namespace MiniMock.Builders;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Util;

internal class MethodBuilder : ISymbolBuilder
{
    public bool TryBuild(CodeBuilder builder, IGrouping<string, ISymbol> symbols)
    {
        var first = symbols.First();

        if (first is IMethodSymbol { MethodKind: MethodKind.Constructor })
        {
            return false;
        }

        if (first is not IMethodSymbol { MethodKind: MethodKind.Ordinary })
        {
            return false;
        }

        var methodSymbols = symbols.OfType<IMethodSymbol>().Where(t => t.MethodKind == MethodKind.Ordinary);
        return this.BuildMethods(builder, methodSymbols);
    }

    private bool BuildMethods(CodeBuilder builder, IEnumerable<IMethodSymbol> methodSymbols)
    {
        var enumerable = methodSymbols as IMethodSymbol[] ?? methodSymbols.ToArray();
        if (enumerable.Length == 0)
        {
            return false;
        }

        var name = enumerable.First().Name;

        var helpers = new List<HelperMethod>();

        builder.Add($"#region Method : {name}");

        var methodCount = 1;
        foreach (var symbol in enumerable)
        {
            if (Build(builder, symbol, helpers, methodCount))
            {
                methodCount++;
            }
        }

        builder.Add(helpers.BuildHelpers(name));

        builder.Add("#endregion");

        return methodCount > 1;
    }

    private static bool Build(CodeBuilder builder, IMethodSymbol symbol, List<HelperMethod> helpers, int methodCount)
    {
        if (!(symbol.IsAbstract || symbol.IsVirtual))
        {
            builder.Add().Add("// Ignoring " + symbol);
            return false;
        }

        if (symbol.ReturnsByRef || symbol.ReturnsByRefReadonly)
        {
            throw new RefReturnTypeNotSupportedException(symbol, symbol.ContainingType);
        }

        if (symbol.IsStatic)
        {
            if (symbol.IsAbstract)
            {
                throw new StaticAbstractMembersNotSupportedException(symbol.Name, symbol.ContainingType);
            }

            builder.Add($"// Ignoring Static method {symbol}.");
            return false;
        }

        var (methodParameters, parameterList, typeList, nameList) = symbol.ParameterStrings();

        var (methodName, methodReturnType, returnString) = MethodName(symbol);

        var (containingSymbol, accessibilityString, overrideString) = symbol.Overwrites();

        var functionPointer = methodCount == 1 ? $"_{methodName}" : "_" + methodName + "_" + methodCount;

        var genericString = GenericString(symbol);
        var delegateType = symbol is { IsGenericMethod: true, ReturnsVoid: false } ? "object" : methodReturnType;
        var castString = symbol is { IsGenericMethod: true, ReturnsVoid: false } ? " (" + methodReturnType + ") " : "";

        builder.Add($$"""
                      public delegate {{delegateType}} {{functionPointer}}_Delegate({{parameterList}});

                      {{accessibilityString}} {{overrideString}}{{methodReturnType}} {{containingSymbol}}{{methodName}}{{genericString}}({{methodParameters}})
                      {
                          {{returnString}}{{castString}}this.{{functionPointer}}.Invoke({{nameList}});
                      }
                      private {{functionPointer}}_Delegate {{functionPointer}} {get;set;} = ({{parameterList}}) => {{symbol.BuildNotMockedException()}}

                      internal partial class Config{
                          private Config {{functionPointer}}({{functionPointer}}_Delegate call){
                              target.{{functionPointer}} = call;
                              return this;
                          }
                      }

                      """);

        helpers.AddRange(AddHelpers(symbol, functionPointer, parameterList, delegateType, typeList, nameList));

        return true;
    }

    private static IEnumerable<HelperMethod> AddHelpers(IMethodSymbol symbol, string functionPointer, string parameterList, string delegateType, string typeList, string nameList)
    {
        var seeCref = symbol.ToString();

        yield return new HelperMethod($"{functionPointer}_Delegate call", $"this.{functionPointer}(call);", Documentation.CallBack, seeCref);

        yield return new HelperMethod("System.Exception throws", $"this.{functionPointer}(({parameterList}) => throw throws);", Documentation.ThrowsException, seeCref);

        if (symbol.ReturnsVoid)
        {
            if (symbol.Parameters.Length == 0)
            {
                yield return new HelperMethod("", $"this.{functionPointer}(() => {{}});", Documentation.AcceptAny, seeCref);
            }
            else if (!HasOutOrRef(symbol))
            {
                yield return new HelperMethod("", $"this.{functionPointer}(({parameterList}) => {{}});", Documentation.AcceptAny, seeCref);
            }
        }

        if (!HasOutOrRef(symbol) && !symbol.ReturnsVoid)
        {
            yield return new HelperMethod($"{delegateType} returns", $"this.{functionPointer}(({parameterList}) => returns);", Documentation.SpecificValue, seeCref);

            var code = $$"""
                         var {{functionPointer}}_Values = returnValues.GetEnumerator();
                         this.{{functionPointer}}(({{parameterList}}) =>
                         {
                             if ({{functionPointer}}_Values.MoveNext())
                             {
                                 return {{functionPointer}}_Values.Current;
                             }

                             {{symbol.BuildNotMockedException()}}
                             });
                         """;
            yield return new HelperMethod($"System.Collections.Generic.IEnumerable<{delegateType}> returnValues", code, Documentation.SpecificValueList, seeCref);
        }

        if (symbol.IsReturningTask())
        {
            if (symbol.HasParameters())
            {
                yield return new HelperMethod($"System.Action<{typeList}> call", $$"""this.{{functionPointer}}(({{parameterList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""", Documentation.CallBack, seeCref);
            }
            else
            {
                yield return new HelperMethod("System.Action call", $$"""this.{{functionPointer}}(({{nameList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""", Documentation.CallBack, seeCref);
            }

            yield return new HelperMethod("", $$"""this.{{functionPointer}}(({{nameList}}) => {return System.Threading.Tasks.Task.CompletedTask;});""", Documentation.AcceptAny, seeCref);
        }

        if (symbol.IsReturningGenericTask())
        {
            var genericType = ((INamedTypeSymbol)symbol.ReturnType).TypeArguments.First();
            yield return new HelperMethod($"{genericType} returns", $"this.{functionPointer}(({parameterList}) => System.Threading.Tasks.Task.FromResult(returns));", Documentation.GenericTaskObject, seeCref);

            var code = $$"""
                         var {{functionPointer}}_Values = returnValues.GetEnumerator();
                         this.{{functionPointer}}(({{parameterList}}) =>
                         {
                             if ({{functionPointer}}_Values.MoveNext())
                             {
                                 return System.Threading.Tasks.Task.FromResult({{functionPointer}}_Values.Current);
                             }

                             {{symbol.BuildNotMockedException()}}
                             });
                         """;
            yield return new HelperMethod($"System.Collections.Generic.IEnumerable<{genericType}> returnValues", code, Documentation.SpecificValueList, seeCref);

            if (symbol.HasParameters())
            {
                yield return new HelperMethod($"System.Func<{typeList},{genericType}> call", $"this.{functionPointer}(({nameList}) => System.Threading.Tasks.Task.FromResult(call({nameList})));", Documentation.GenericTaskFunction, seeCref);
            }
            else
            {
                yield return new HelperMethod($"System.Func<{genericType}> call", $"this.{functionPointer}(({nameList}) => System.Threading.Tasks.Task.FromResult(call({nameList})));", Documentation.GenericTaskFunction, seeCref);
            }
        }
    }

    private static bool HasOutOrRef(IMethodSymbol method) =>
        method.Parameters.Any(p => p.RefKind is RefKind.Out or RefKind.Ref);

    private static (string methodName, string methodReturnType, string returnString) MethodName(
        IMethodSymbol method)
    {
        var methodName = method.Name;
        var methodReturnType = method.ReturnType.ToString();
        var returnString = method.ReturnsVoid ? "" : "return ";
        return (methodName, methodReturnType, returnString);
    }

    private static string GenericString(IMethodSymbol symbol)
    {
        if (!symbol.IsGenericMethod)
        {
            return "";
        }

        var typeArguments = symbol.TypeArguments;
        var types = string.Join(", ", typeArguments.Select(t => t.Name));
        return $"<{types}>";
    }
}
