namespace MiniMock.Builders;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal static class MethodBuilder
{
    public static void BuildMethods(CodeBuilder builder, IEnumerable<IMethodSymbol> methodSymbols)
    {
        var enumerable = methodSymbols as IMethodSymbol[] ?? methodSymbols.ToArray();
        var name = enumerable.First().Name;

        var helpers = new List<MethodSignature>();

        builder.Add($"#region Method : {name}");

        var methodCount = 0;
        foreach (var symbol in enumerable)
        {
            methodCount++;
            Build(builder, symbol, helpers, methodCount);
        }

        helpers.BuildHelpers(builder, name);

        builder.Add("#endregion");
    }

    private static void Build(CodeBuilder builder, IMethodSymbol symbol, List<MethodSignature> helpers, int methodCount)
    {
        if (!(symbol.IsAbstract || symbol.IsVirtual))
        {
            builder.Add().Add("// Ignoring " + symbol);
            return;
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
            return;
        }

        var (methodParameters, parameterList, typeList, nameList) = symbol.ParameterStrings();

        var (methodName, methodReturnType, returnString) = MethodName(symbol);

        var (containingSymbol, accessibilityString, overrideString) = symbol.Overwrites();

        var functionPointer = methodCount == 1 ? $"_{methodName}" : "_" + methodName + "_" + methodCount;

        var genericString = GenericString(symbol);
        var delegateType = symbol.IsGenericMethod && !symbol.ReturnsVoid ? "object" : methodReturnType;
        var castString = symbol.IsGenericMethod && !symbol.ReturnsVoid ? " (" + methodReturnType + ") " : "";

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

        var seeCref = symbol.ToString();

        helpers.Add(new($"{functionPointer}_Delegate call", $"this.{functionPointer}(call);", Documentation.CallBack, seeCref));

        helpers.Add(new("System.Exception throws", $"this.{functionPointer}(({parameterList}) => throw throws);", Documentation.ThrowsException, seeCref));

        if (symbol.ReturnsVoid)
        {
            if (symbol.Parameters.Length == 0)
            {
                helpers.Add(new("", $"this.{functionPointer}(() => {{}});", Documentation.AcceptAny, seeCref));
            }
            else if (!HasOutOrRef(symbol))
            {
                helpers.Add(new("", $"this.{functionPointer}(({parameterList}) => {{}});", Documentation.AcceptAny, seeCref));
            }
        }

        if (!HasOutOrRef(symbol) && !symbol.ReturnsVoid)
        {
            helpers.Add(new($"{delegateType} returns", $"this.{functionPointer}(({parameterList}) => returns);", Documentation.SpecificValue, seeCref));

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
            helpers.Add(new($"System.Collections.Generic.IEnumerable<{delegateType}> returnValues", code, Documentation.SpecificValueList, seeCref));
        }

        if (symbol.IsReturningTask())
        {
            if (symbol.HasParameters())
            {
                helpers.Add(new($"System.Action<{typeList}> call", $$"""this.{{functionPointer}}(({{parameterList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""", Documentation.CallBack, seeCref));
            }
            else
            {
                helpers.Add(new("System.Action call", $$"""this.{{functionPointer}}(({{nameList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""", Documentation.CallBack, seeCref));
            }

            helpers.Add(new("", $$"""this.{{functionPointer}}(({{nameList}}) => {return System.Threading.Tasks.Task.CompletedTask;});""", Documentation.AcceptAny, seeCref));
        }

        if (symbol.IsReturningGenericTask())
        {
            var genericType = ((INamedTypeSymbol)symbol.ReturnType).TypeArguments.First();
            helpers.Add(new($"{genericType} returns", $"this.{functionPointer}(({parameterList}) => System.Threading.Tasks.Task.FromResult(returns));", Documentation.GenericTaskObject, seeCref));

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
            helpers.Add(new($"System.Collections.Generic.IEnumerable<{genericType}> returnValues", code, Documentation.SpecificValueList, seeCref));

            if (symbol.HasParameters())
            {
                helpers.Add(new($"System.Func<{typeList},{genericType}> call", $"this.{functionPointer}(({nameList}) => System.Threading.Tasks.Task.FromResult(call({nameList})));", Documentation.GenericTaskFunction, seeCref));
            }
            else
            {
                helpers.Add(new($"System.Func<{genericType}> call", $"this.{functionPointer}(({nameList}) => System.Threading.Tasks.Task.FromResult(call({nameList})));", Documentation.GenericTaskFunction, seeCref));
            }
        }
    }

    private static bool HasOutOrRef(IMethodSymbol method) =>
        method.Parameters.Any(p => p.RefKind == RefKind.Out || p.RefKind == RefKind.Ref);

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
