namespace MiniMock.Builders;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal static class Documentation
{
    internal const string callBackDocumentation = "Configures the mock to execute the specified action when the method matching the signature is called.";
    internal const string acceptAnyDocumentation = "Configures the mock to accept any call to the method.";
    internal const string throwDocumentation = "Configures the mock to throw the specified exception when the method is called.";
    internal const string specificValueDocumentation = "Configures the mock to return the specific value.";
    internal const string genericTaskObjectDocumentation = "Configures the mock to return the specific value in a task object.";
    internal const string genericTaskFunctionDocumentation = "Configures the mock to call the specified function and return the value wrapped in a task object when the method matching the signature is called.";

}

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
                          private Config {{functionPointer}}({{functionPointer}}_Delegate call){
                              target.{{functionPointer}} = call;
                              return this;
                          }
                      }

                      """);

        helpers.Add(new("System.Exception throws", $"this.{functionPointer}(({parameterList}) => throw throws);", Documentation.throwDocumentation));

        switch (symbol.ReturnsVoid)
        {
            case true when symbol.Parameters.Length == 0:
                helpers.Add(new("System.Action call", $"this.{functionPointer}(() => call());", Documentation.callBackDocumentation));
                helpers.Add(new("", $"this.{functionPointer}(() => {{}});", Documentation.acceptAnyDocumentation));
                break;
            case true when !HasOutOrRef(symbol):
                helpers.Add(new($"System.Action<{typeList}> call", $"this.{functionPointer}(({parameterList}) => call({nameList}));", Documentation.callBackDocumentation));
                helpers.Add(new("", $"this.{functionPointer}(({parameterList}) => {{}});", Documentation.acceptAnyDocumentation));
                break;
            case false when !HasOutOrRef(symbol) && symbol.Parameters.Length == 0:
                helpers.Add(new($"System.Func<{methodReturnType}> call", $"this.{functionPointer}(() => call());", Documentation.callBackDocumentation));
                break;
            case false when !HasOutOrRef(symbol) && symbol.Parameters.Length > 0:
                helpers.Add(new($"System.Func<{typeList},{methodReturnType}> call", $"this.{functionPointer}(({parameterList}) => call({nameList}));", Documentation.callBackDocumentation));
                break;
            default:
                helpers.Add(new($"{functionPointer}_Delegate call", $"this.{functionPointer}(call);", Documentation.callBackDocumentation));
                break;
        }

        if (!HasOutOrRef(symbol) && !symbol.ReturnsVoid)
        {
            helpers.Add(new($"{methodReturnType} returns", $"this.{functionPointer}(({parameterList}) => returns);", Documentation.specificValueDocumentation));
        }

        if (symbol.IsReturningTask())
        {
            if (symbol.HasParameters())
            {
                helpers.Add(new($"System.Action<{typeList}> call", $$"""this.{{functionPointer}}(({{parameterList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""", Documentation.callBackDocumentation));
            }
            else
            {
                helpers.Add(new("System.Action call", $$"""this.{{functionPointer}}(({{nameList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""", Documentation.callBackDocumentation));
            }

            helpers.Add(new("", $$"""this.{{functionPointer}}(({{nameList}}) => {return System.Threading.Tasks.Task.CompletedTask;});""", Documentation.acceptAnyDocumentation));
        }

        if (symbol.IsReturningGenericTask())
        {
            var genericType = ((INamedTypeSymbol)symbol.ReturnType).TypeArguments.First();
            helpers.Add(new($"{genericType} returns", $"this.{functionPointer}(({parameterList}) => System.Threading.Tasks.Task.FromResult(returns));", Documentation.genericTaskObjectDocumentation));
            if (symbol.HasParameters())
            {
                helpers.Add(new($"System.Func<{typeList},{genericType}> call", $"this.{functionPointer}(({nameList}) => System.Threading.Tasks.Task.FromResult(call({nameList})));", Documentation.genericTaskFunctionDocumentation));
            }
            else
            {
                helpers.Add(new($"System.Func<{genericType}> call", $"this.{functionPointer}(({nameList}) => System.Threading.Tasks.Task.FromResult(call({nameList})));", Documentation.genericTaskFunctionDocumentation));
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
