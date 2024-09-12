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

        if(symbol.IsGenericMethod)
        {
            throw new GenericMethodNotSupportedException(symbol, symbol.ContainingType);
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

        var (parameterList, typeList, nameList) = symbol.ParameterStrings();

        var (methodName, methodReturnType, returnString) = MethodName(symbol);

        var (containingSymbol, accessibilityString, overrideString) = symbol.Overwrites();

        var functionPointer = methodCount == 1 ? $"_{methodName}" : "_" + methodName + "_" + methodCount;

        builder.Add($$"""
                      public delegate {{methodReturnType}} {{functionPointer}}_Delegate({{parameterList}});

                      {{accessibilityString}} {{overrideString}}{{methodReturnType}} {{containingSymbol}}{{methodName}}({{parameterList}})
                      {
                          {{returnString}}this.{{functionPointer}}.Invoke({{nameList}});
                      }
                      private {{functionPointer}}_Delegate {{functionPointer}} {get;set;} = ({{parameterList}}) => {{symbol.BuildNotMockedException()}}

                      internal partial class Config{
                          private Config {{functionPointer}}({{functionPointer}}_Delegate call){
                              target.{{functionPointer}} = call;
                              return this;
                          }
                      }

                      """);

        helpers.Add(new("System.Exception throws", $"this.{functionPointer}(({parameterList}) => throw throws);", Documentation.ThrowsException));

        switch (symbol.ReturnsVoid)
        {
            case true when symbol.Parameters.Length == 0:
                helpers.Add(new("System.Action call", $"this.{functionPointer}(() => call());", Documentation.CallBack));
                helpers.Add(new("", $"this.{functionPointer}(() => {{}});", Documentation.AcceptAny));
                break;
            case true when !HasOutOrRef(symbol):
                helpers.Add(new($"System.Action<{typeList}> call", $"this.{functionPointer}(({parameterList}) => call({nameList}));", Documentation.CallBack));
                helpers.Add(new("", $"this.{functionPointer}(({parameterList}) => {{}});", Documentation.AcceptAny));
                break;
            case false when !HasOutOrRef(symbol) && symbol.Parameters.Length == 0:
                helpers.Add(new($"System.Func<{methodReturnType}> call", $"this.{functionPointer}(() => call());", Documentation.CallBack));
                break;
            case false when !HasOutOrRef(symbol) && symbol.Parameters.Length > 0:
                helpers.Add(new($"System.Func<{typeList},{methodReturnType}> call", $"this.{functionPointer}(({parameterList}) => call({nameList}));", Documentation.CallBack));
                break;
            default:
                helpers.Add(new($"{functionPointer}_Delegate call", $"this.{functionPointer}(call);", Documentation.CallBack));
                break;
        }

        if (!HasOutOrRef(symbol) && !symbol.ReturnsVoid)
        {
            helpers.Add(new($"{methodReturnType} returns", $"this.{functionPointer}(({parameterList}) => returns);", Documentation.SpecificValue));

            var code = $$"""
                            var {{functionPointer}}_Values = returns.GetEnumerator();
                            this.{{functionPointer}}(({{parameterList}}) =>
                            {
                                if ({{functionPointer}}_Values.MoveNext())
                                {
                                    return {{functionPointer}}_Values.Current;
                                }

                                {{symbol.BuildNotMockedException()}}
                                });
                            """;
            helpers.Add(new($"System.Collections.Generic.IEnumerable<{methodReturnType}> returns", code, Documentation.SpecificValueList));
        }

        if (symbol.IsReturningTask())
        {
            if (symbol.HasParameters())
            {
                helpers.Add(new($"System.Action<{typeList}> call", $$"""this.{{functionPointer}}(({{parameterList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""", Documentation.CallBack));
            }
            else
            {
                helpers.Add(new("System.Action call", $$"""this.{{functionPointer}}(({{nameList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""", Documentation.CallBack));
            }

            helpers.Add(new("", $$"""this.{{functionPointer}}(({{nameList}}) => {return System.Threading.Tasks.Task.CompletedTask;});""", Documentation.AcceptAny));
        }

        if (symbol.IsReturningGenericTask())
        {
            var genericType = ((INamedTypeSymbol)symbol.ReturnType).TypeArguments.First();
            helpers.Add(new($"{genericType} returns", $"this.{functionPointer}(({parameterList}) => System.Threading.Tasks.Task.FromResult(returns));", Documentation.GenericTaskObject));

            var code = $$"""
                         var {{functionPointer}}_Values = returns.GetEnumerator();
                         this.{{functionPointer}}(({{parameterList}}) =>
                         {
                             if ({{functionPointer}}_Values.MoveNext())
                             {
                                 return System.Threading.Tasks.Task.FromResult({{functionPointer}}_Values.Current);
                             }

                             {{symbol.BuildNotMockedException()}}
                             });
                         """;
            helpers.Add(new($"System.Collections.Generic.IEnumerable<{genericType}> returns", code, Documentation.SpecificValueList));


            if (symbol.HasParameters())
            {
                helpers.Add(new($"System.Func<{typeList},{genericType}> call", $"this.{functionPointer}(({nameList}) => System.Threading.Tasks.Task.FromResult(call({nameList})));", Documentation.GenericTaskFunction));
            }
            else
            {
                helpers.Add(new($"System.Func<{genericType}> call", $"this.{functionPointer}(({nameList}) => System.Threading.Tasks.Task.FromResult(call({nameList})));", Documentation.GenericTaskFunction));
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
}
