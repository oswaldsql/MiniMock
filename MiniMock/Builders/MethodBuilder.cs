namespace MiniMock.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Strategy = System.Func<CodeBuilder, Microsoft.CodeAnalysis.IMethodSymbol, System.Action<string, string, string>, bool>;

internal static class MethodBuilder
{
    private static readonly Strategy[] Strategies =
    [
        IgnoreIrrelevantMethods,
        TryBuildMethodHavingOutParameters,
        TryBuildVoidMethodsHavingParameters,
        TryBuildVoidMethodsWithoutParameters,
        TryBuildNoneVoidMethods
    ];

    private static int methodCount;

    public static void BuildMethods(CodeBuilder builder, IEnumerable<IMethodSymbol> methodSymbols)
    {
        var enumerable = methodSymbols as IMethodSymbol[] ?? methodSymbols.ToArray();
        var name = enumerable.First().Name;

        var helpers = new List<MethodSignature>();

        void AddHelper(string signature, string code, string documentation)
        {
            helpers.Add(new(signature, code, documentation));
        }

        foreach (var symbol in enumerable)
        {
            Build(builder, symbol, AddHelper);
        }

        BuildHelpers(builder, helpers, name);
    }

    private static void BuildHelpers(CodeBuilder builder, List<MethodSignature> helpers, string name)
    {
        if (helpers.Count == 0)
        {
            return;
        }

        var signatures = helpers.ToLookup(t => t.Signature);

        builder.Add("public partial class Config {").Indent();

        foreach (var grouping in signatures)
        {
            builder.Add($"""
                         
                         /// <summary>
                         """);
            grouping.Select(t => t.Documentation).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToList().ForEach(t => builder.Add("///     " + t));
            builder.Add($"""
                         /// </summary>
                         /// <returns>The updated configuration.</returns>
                         """);

            builder.Add($"public Config {name}({grouping.Key}) {{").Indent();
            foreach (var mse in grouping)
            {
                builder.Add(mse.Code);
            }

            builder.Unindent().Add("    return this;");
            builder.Add("}");
            builder.Add();
        }

        builder.Unindent().Add("}");
    }

    private static bool Build(CodeBuilder builder, IMethodSymbol method, Action<string, string, string> addHelper)
    {
        foreach (var strategy in Strategies)
        {
            if (strategy(builder, method, addHelper)) { return true; }
        }

        return false;
    }

    private static bool IgnoreIrrelevantMethods(CodeBuilder builder, IMethodSymbol method,
        Action<string, string, string> addHelper)
    {
        if (method.IsAbstract || method.IsVirtual)
        {
            return false;
        }

        builder.Add().Add("// Ignoring " + method);

        return true;
    }

    private static bool TryBuildMethodHavingOutParameters(CodeBuilder builder, IMethodSymbol method,
        Action<string, string, string> addHelper)
    {
        if (!(method.HasParameters() &&
              method.Parameters.Any(p => p.RefKind == RefKind.Out || p.RefKind == RefKind.Ref)))
        {
            return false;
        }

        var parameterList = method.ToString(p => $"{p.OutString()}{p.Type} {p.Name}");
        var methodReturnType = (INamedTypeSymbol)method.ReturnType;

        var nameList = method.ToString(p => $"{p.OutString()}{p.Name}");

        var overrideString = method.OverrideString();

        var methodName = method.Name;
        var functionPointer = "On_" + methodName + "_" + methodCount++;

        var returnString = method.ReturnsVoid ? "" : "return ";

        var containingSymbol = "";
        var accessibilityString = method.AccessibilityString();
        if (method.ContainingType.TypeKind == TypeKind.Interface)
        {
            containingSymbol = method.ContainingSymbol + ".";
            accessibilityString = "";
        }

        builder.Add($$"""

                      #region Method : {{methodReturnType}} {{methodName}}({{parameterList}})
                      public delegate {{methodReturnType}} {{methodName}}_{{methodCount}}_Delegate({{parameterList}});

                      {{accessibilityString}} {{overrideString}}{{methodReturnType}} {{containingSymbol}}{{methodName}}({{parameterList}})
                      {
                          {{returnString}}this.{{functionPointer}}.Invoke({{nameList}});
                      }
                      internal {{methodName}}_{{methodCount}}_Delegate {{functionPointer}} {get;set;} = ({{parameterList}}) => {{BuildNotMockedException(method)}}

                      public partial class Config{
                          private Config _{{methodName}}_{{methodCount}}({{methodName}}_{{methodCount}}_Delegate call){
                              target.{{functionPointer}} = call;
                              return this;
                          }
                      }

                      #endregion

                      """);

        addHelper($"{methodName}_{methodCount}_Delegate call", $"this._{methodName}_{methodCount}(call);", "Configures the mock to execute the specified action when the method matching the signature is called.");
        addHelper("System.Exception throws", $"this._{method.Name}_{methodCount}(({parameterList}) => throw throws);", "Configures the mock to throw the specified exception when the method is called.");

        return true;
    }

    private static bool TryBuildNoneVoidMethods(CodeBuilder builder, IMethodSymbol method,
        Action<string, string, string> addHelper)
    {
        if (method.ReturnsVoid)
        {
            return false;
        }

        var parameterList = method.ToString(p => $"{p.Type} {p.Name}");
        var typeList = method.ToString(p => $"{p.Type}");
        var methodReturnType = (INamedTypeSymbol)method.ReturnType;
        if (method.Parameters.Length > 0)
        {
            typeList += ", ";
        }

        var nameList = method.ToString(p => $"{p.Name}");
        var lambdaList = method.ToString(p => $"{p.Type} _");

        var overrideString = method.OverrideString();

        var methodName = method.Name;
        var functionPointer = "On_" + methodName + "_" + methodCount++;

        var containingSymbol = "";
        var accessibilityString = method.AccessibilityString();
        if (method.ContainingType.TypeKind == TypeKind.Interface)
        {
            containingSymbol = method.ContainingSymbol + ".";
            accessibilityString = "";
        }

        builder.Add($$$"""

                       #region Method : {{{methodReturnType}}} {{{methodName}}}({{{parameterList}}})
                       {{{accessibilityString}}} {{{overrideString}}}{{{methodReturnType}}} {{{containingSymbol}}}{{{methodName}}}({{{parameterList}}})
                       {
                           return this.{{{functionPointer}}}.Invoke({{{nameList}}});
                       }
                       internal System.Func<{{{typeList}}}{{{methodReturnType}}}> {{{functionPointer}}} {get;set;} = ({{{lambdaList}}}) => {{{BuildNotMockedException(method)}}}

                       public partial class Config{
                           private Config _{{{methodName}}}_{{{methodCount}}}(System.Func<{{{typeList}}}{{{methodReturnType}}}> call){
                               target.{{{functionPointer}}} = call;
                               return this;
                           }
                       }

                       #endregion
                       """);

        addHelper($"System.Func<{typeList}{methodReturnType}> call", $"this._{methodName}_{methodCount}(call);", "Configures the mock to call the specified function and return the value when the method matching the signature is called.");
        addHelper("System.Exception throws", $"this._{methodName}_{methodCount}(({lambdaList}) => throw throws);", "Configures the mock to throw the specified exception when the method is called.");
        addHelper(methodReturnType + " returns", $"this._{methodName}_{methodCount}(({lambdaList}) => returns);", $"Configures the mock to return the specific value when returning <see cref=\"{EscapeToHtml(methodReturnType.ToString())}\"/>");

        if (methodReturnType.IsTask())
        {
            if (method.HasParameters())
            {
                var typeList2 = method.ToString(p => $"{p.Type}");
                addHelper($"System.Action<{typeList2}> call", $$"""this._{{methodName}}_{{methodCount}}(({{parameterList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""", "Configures the mock to execute the specified action when the method matching the signature is called.");
            }
            else
            {
                addHelper("System.Action call", $$"""this._{{methodName}}_{{methodCount}}(({{nameList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""", "Configures the mock to execute the specified action when the method matching the signature is called.");
            }

            addHelper("", $$"""this._{{methodName}}_{{methodCount}}(({{nameList}}) => {return System.Threading.Tasks.Task.CompletedTask;});""", "Configures the mock to ignore but accept any call to the method.");
        }

        if (methodReturnType.IsGenericTask())
        {
            var genericType = ((INamedTypeSymbol)method.ReturnType).TypeArguments.First();
            addHelper($"{genericType} returns", $"this._{methodName}_{methodCount}(({lambdaList}) => System.Threading.Tasks.Task.FromResult(returns));", $"Configures the mock to return the specific value when returning a generic task containing <see cref=\"{EscapeToHtml(methodReturnType.ToString())}\"/>");
            addHelper($"System.Func<{typeList}{genericType}> call", $"this._{methodName}_{methodCount}(({nameList}) => System.Threading.Tasks.Task.FromResult(call({nameList})));", "Configures the mock to call the specified function and return the value wrapped in a task object when the method matching the signature is called.");
        }

        return true;
    }

    private static bool TryBuildVoidMethodsWithoutParameters(CodeBuilder builder, IMethodSymbol method,
        Action<string, string, string> addHelper)
    {
        if (!(method.ReturnsVoid && !method.HasParameters()))
        {
            return false;
        }

        var parameterList = method.ToString(p => $"{p.Type} {p.Name}");

        var nameList = method.ToString(p => $"{p.Name}");
        var lambdaList = method.ToString(p => $"{p.Type} _");

        var overrideString = method.OverrideString();

        var methodName = method.Name;
        var functionPointer = "On_" + methodName + "_" + methodCount++;

        var containingSymbol = "";
        var accessibilityString = method.AccessibilityString();
        if (method.ContainingType.TypeKind == TypeKind.Interface)
        {
            containingSymbol = method.ContainingSymbol + ".";
            accessibilityString = "";
        }

        builder.Add($$"""

                      #region Method : void {{methodName}}({{parameterList}})
                      {{accessibilityString}} {{overrideString}} void {{containingSymbol}}{{methodName}}({{parameterList}})
                      {
                          this.{{functionPointer}}.Invoke({{nameList}});
                      }
                      internal System.Action {{functionPointer}} {get;set;} = ({{lambdaList}}) => {{BuildNotMockedException(method)}}

                      public partial class Config{
                          private Config _{{methodName}}_{{methodCount}}(System.Action mock){
                                target.{{functionPointer}} = mock;
                                return this;
                          }
                      }
                      #endregion
                      """);

        addHelper("System.Action call", $"this._{methodName}_{methodCount}(call);", "Configures the mock to execute the specified action when the method matching the signature is called.");
        addHelper("System.Exception throws", $$"""this._{{methodName}}_{{methodCount}}(({{lambdaList}}) => throw throws);""", "Configures the mock to throw the specified exception when the method is called.");
        addHelper("", $$"""this._{{methodName}}_{{methodCount}}(({{lambdaList}}) => {});""", "Configures the mock to ignore but accept any call to the method.");

        return true;
    }

    private static bool TryBuildVoidMethodsHavingParameters(CodeBuilder builder, IMethodSymbol method,
        Action<string, string, string> addHelper)
    {
        if (!(method.ReturnsVoid && method.HasParameters()))
        {
            return false;
        }

        var parameterList = method.ToString(p => $"{p.Type} {p.Name}");
        var typeList = method.ToString(p => $"{p.Type}");

        var nameList = method.ToString(p => $"{p.Name}");
        var overrideString = method.OverrideString();

        var methodName = method.Name;
        var functionPointer = "On_" + methodName + "_" + methodCount++;

        var containingSymbol = "";
        var accessibilityString = method.AccessibilityString();
        if (method.ContainingType.TypeKind == TypeKind.Interface)
        {
            containingSymbol = method.ContainingSymbol + ".";
            accessibilityString = "";
        }

        builder.Add($$"""

                      #region Method : void {{methodName}}({{parameterList}})
                      {{accessibilityString}} {{overrideString}} void {{containingSymbol}}{{methodName}}({{parameterList}})
                      {
                          this.{{functionPointer}}.Invoke({{nameList}});
                      }
                      internal System.Action<{{typeList}}> {{functionPointer}} {get;set;} = ({{parameterList}}) => {{BuildNotMockedException(method)}}

                      public partial class Config{
                          private Config _{{methodName}}_{{methodCount}}(System.Action<{{typeList}}> mock){
                              target.{{functionPointer}} = mock;
                              return this;
                          }
                      }
                      #endregion
                      """);
        addHelper($"System.Action<{typeList}> call", $"this._{methodName}_{methodCount}(call);", "Configures the mock to execute the specified action when the method matching the signature is called.");
        addHelper("System.Exception throws", $$"""this._{{methodName}}_{{methodCount}}(({{parameterList}}) => throw throws);""", "Configures the mock to throw the specified exception when the method is called.");
        addHelper("", $$"""this._{{methodName}}_{{methodCount}}(({{parameterList}}) => {});""", "Configures the mock to ignore but accept any call to the method.");

        return true;
    }

    private static string OutString(this IParameterSymbol parameterSymbol) =>
        parameterSymbol.RefKind switch
        {
            RefKind.Out => "out ",
            RefKind.Ref => "ref ",
            RefKind.In => "in ",
            RefKind.RefReadOnlyParameter => "ref readonly ",
            _ => ""
        };

    private static bool HasParameters(this IMethodSymbol method) => method.Parameters.Length > 0;

    private static string BuildNotMockedException(this IMethodSymbol symbol) =>
        $"throw new System.InvalidOperationException(\"The method '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";

    private static bool IsTask(this INamedTypeSymbol methodReturnType) =>
        methodReturnType.ToString().Equals("System.Threading.Tasks.Task");

    private static bool IsGenericTask(this INamedTypeSymbol methodReturnType) =>
        methodReturnType.ToString().StartsWith("System.Threading.Tasks.Task<") &&
        methodReturnType.TypeArguments.Length > 0;

    private static string EscapeToHtml(string text) => text.Replace("<", "&lt;").Replace(">", "&gt;");
}

public class MethodSignature
{
    public MethodSignature(string signature, string code, string documentation)
    {
        this.Signature = signature;
        this.Code = code;
        this.Documentation = documentation;
    }

    public string Signature { get; }
    public string Code { get; }
    public string Documentation { get; }
}
