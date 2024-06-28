namespace MiniMock.Builders;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal static class MethodBuilder
{
//    private static int methodCount;

    public static void BuildMethods(CodeBuilder builder, IEnumerable<IMethodSymbol> methodSymbols)
    {
        var enumerable = methodSymbols as IMethodSymbol[] ?? methodSymbols.ToArray();
        var name = enumerable.First().Name;

        var helpers = new List<MethodSignature>();

        builder.Add($"#region Method : {name}");

        int methodCount = 0;
        foreach (var symbol in enumerable)
        {
            methodCount++;
            Build(builder, symbol, helpers, methodCount);
        }

        builder.Add("#endregion");

        helpers.BuildHelpers(builder, name);
    }

    private static void Build(CodeBuilder builder, IMethodSymbol symbol, List<MethodSignature> helpers, int methodCount)
    {
        if (!(symbol.IsAbstract || symbol.IsVirtual))
        {
            builder.Add().Add("// Ignoring " + symbol);
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

                      public partial class Config{
                          private Config _{{functionPointer}}({{functionPointer}}_Delegate call){
                              target.{{functionPointer}} = call;
                              return this;
                          }
                      }

                      """);

        helpers.Add(new($"{functionPointer}_Delegate call",
            $"this._{functionPointer}(call);",
            "Configures the mock to execute the specified action when the method matching the signature is called."));

        helpers.Add(new("System.Exception throws",
            $"this._{functionPointer}(({parameterList}) => throw throws);",
            "Configures the mock to throw the specified exception when the method is called."));

        if (!HasOutOrRef(symbol) && !symbol.ReturnsVoid)
        {
            helpers.Add(new($"{methodReturnType} returns",
                $"this._{functionPointer}(({parameterList}) => returns);",
                $"Configures the mock to return the specific value when returning <see cref=\"{Helpers.EscapeToHtml(methodReturnType)}\"/>"));
        }

        if (symbol.IsReturningTask())
        {
            if (symbol.HasParameters())
            {
                helpers.Add(new($"System.Action<{typeList}> call",
                    $$"""this._{{functionPointer}}(({{parameterList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""",
                    "Configures the mock to execute the specified action when the method matching the signature is called."));
            }
            else
            {
                helpers.Add(new("System.Action call",
                    $$"""this._{{functionPointer}}(({{nameList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""",
                    "Configures the mock to execute the specified action when the method matching the signature is called."));
            }

            helpers.Add(new("",
                $$"""this._{{functionPointer}}(({{nameList}}) => {return System.Threading.Tasks.Task.CompletedTask;});""",
                "Configures the mock to accept any call to the method."));
        }

        if (symbol.IsReturningGenericTask())
        {
            var genericType = ((INamedTypeSymbol)symbol.ReturnType).TypeArguments.First();
            helpers.Add(new($"{genericType} returns",
                $"this._{functionPointer}(({parameterList}) => System.Threading.Tasks.Task.FromResult(returns));",
                $"Configures the mock to return the specific value when returning a generic task containing <see cref=\"{Helpers.EscapeToHtml(methodReturnType)}\"/>"));
            if (symbol.HasParameters())
            {
                helpers.Add(new($"System.Func<{typeList},{genericType}> call",
                    $"this._{functionPointer}(({nameList}) => System.Threading.Tasks.Task.FromResult(call({nameList})));",
                    "Configures the mock to call the specified function and return the value wrapped in a task object when the method matching the signature is called."));
            }
            else
            {
                helpers.Add(new($"System.Func<{genericType}> call",
                    $"this._{functionPointer}(({nameList}) => System.Threading.Tasks.Task.FromResult(call({nameList})));",
                    "Configures the mock to call the specified function and return the value wrapped in a task object when the method matching the signature is called."));
            }
        }
    }

    private static bool HasOutOrRef(IMethodSymbol method) =>
        method.Parameters.Any(p => p.RefKind == RefKind.Out || p.RefKind == RefKind.Ref);

    private static (string methodName, string methodReturnType, string returnString) MethodName(
        IMethodSymbol method)
    {
        var methodName = method.Name;
        var methodReturnType = ((INamedTypeSymbol)method.ReturnType).ToString();
        var returnString = method.ReturnsVoid ? "" : "return ";
        return (methodName, methodReturnType, returnString);
    }
}
