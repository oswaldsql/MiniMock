namespace MiniMock.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

using guard = System.Func<CodeBuilder, Microsoft.CodeAnalysis.IMethodSymbol, System.Action<string, string>, bool>;

internal static class MethodBuilder
{
    private static readonly guard[] Strategies =
    [
        IgnoreIrrelevantMethods,
        TryBuildMethodHavingOutParameters,
        TryBuildVoidMethodsHavingParameters,
        TryBuildVoidMethodsWithoutParameters,
        TryBuildNoneVoidMethods
    ];

    private static bool IgnoreIrrelevantMethods(CodeBuilder arg1, IMethodSymbol arg2, Action<string, string> arg3)
    {
        if (arg2.IsAbstract || arg2.IsVirtual)
        {
            return false;
        }

        arg1.Add().Add("// Ignoring " + arg2);

        return true;
    }

    private static int methodCount = 0;

    public static void BuildMethods(CodeBuilder builder, IEnumerable<IMethodSymbol> methodSymbols)
    {
        var enumerable = methodSymbols as IMethodSymbol[] ?? methodSymbols.ToArray();
        var name = enumerable.First().Name;

        var helpers = new List<MethodSignature>();
        void AddHelper(string signature, string code) => helpers.Add(new(signature, code));

        foreach (var symbol in enumerable)
        {
            Build(builder, symbol, AddHelper);
        }

        BuildHelpers(builder, helpers, name);
    }

    private static void BuildHelpers(CodeBuilder builder, List<MethodSignature> helpers, string name)
    {
        if(helpers.Count == 0)
        {
            return;
        }

        var signatures = helpers.ToLookup(t => t.Signature);

        builder.Add("public partial class Config {").Indent();

        foreach (var grouping in signatures)
        {
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

    private static bool Build(CodeBuilder builder, IMethodSymbol method, Action<string, string> addHelper)
    {
        foreach (var strategy in Strategies)
        {
            if(strategy(builder, method, addHelper)) { return true; }
        }

        return false;
    }

    private static bool TryBuildMethodHavingOutParameters(CodeBuilder builder, IMethodSymbol method, Action<string,string> addHelper)
    {
        if(!(method.HasParameters() && method.Parameters.Any(p => p.RefKind == RefKind.Out || p.RefKind == RefKind.Ref)))
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
        var containingSymbol = method.ContainingSymbol;

        builder.Add($$$"""

                       #region Method : {{{methodReturnType}}} {{{methodName}}}({{{parameterList}}})
                       public delegate {{{methodReturnType}}} {{{methodName}}}_Delegate({{{parameterList}}});

                       {{{method.AccessibilityString()}}} {{{overrideString}}}{{{methodReturnType}}} {{{methodName}}}({{{parameterList}}})
                       {
                           {{{returnString}}}this.{{{functionPointer}}}.Invoke({{{nameList}}});
                       }
                       internal {{{methodName}}}_Delegate {{{functionPointer}}} {get;set;} = ({{{parameterList}}}) => {{{BuildNotMockedException(method)}}}

                       public partial class Config{
                           private Config _{{{methodName}}}({{{methodName}}}_Delegate call){
                               target.{{{functionPointer}}} = call;
                               return this;
                           }
                       }

                       #endregion

                       """);

        addHelper($"{methodName}_Delegate call", $"this._{methodName}(call);");
        addHelper("System.Exception throws", $"this._{method.Name}(({parameterList}) => throw throws);");

        return true;
    }

    private static bool TryBuildNoneVoidMethods(CodeBuilder builder, IMethodSymbol method, Action<string, string> addHelper)
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

        builder.Add($$$"""
                       
                       #region Method : {{{methodReturnType}}} {{{methodName}}}({{{parameterList}}})
                       {{{method.AccessibilityString()}}} {{{overrideString}}}{{{methodReturnType}}} {{{methodName}}}({{{parameterList}}})
                       {
                           return this.{{{functionPointer}}}.Invoke({{{nameList}}});
                       }
                       internal System.Func<{{{typeList}}}{{{methodReturnType}}}> {{{functionPointer}}} {get;set;} = ({{{lambdaList}}}) => {{{BuildNotMockedException(method)}}}

                       public partial class Config{
                           private Config _{{{methodName}}}(System.Func<{{{typeList}}}{{{methodReturnType}}}> call){
                               target.{{{functionPointer}}} = call;
                               return this;
                           }
                       }
                       
                       #endregion
                       """);

        addHelper($"System.Func<{typeList}{methodReturnType}> call", $"this._{methodName}(call);");
        addHelper("System.Exception throws", $"this._{methodName}(({lambdaList}) => throw throws);");
        addHelper(methodReturnType + " returns", $"this._{methodName}(({lambdaList}) => returns);");

        if (methodReturnType.IsTask())
        {
            if (method.HasParameters())
            {
                var typeList2 = method.ToString(p => $"{p.Type}");
                addHelper($"System.Action<{typeList2}> call",$$"""this._{{methodName}}(({{parameterList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""");
                addHelper("",$$"""this._{{methodName}}(({{nameList}}) => {return System.Threading.Tasks.Task.CompletedTask;});""");
            }
            else
            {
                addHelper("System.Action call", $$"""this._{{methodName}}(({{nameList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""");
                addHelper("", $$"""this._{{methodName}}(({{nameList}}) => {return System.Threading.Tasks.Task.CompletedTask;});""");
            }
        }

        if (methodReturnType.IsGenericTask())
        {
            var genericType = ((INamedTypeSymbol)method.ReturnType).TypeArguments.First();
            addHelper($"{genericType} returns",$"this._{methodName}(({lambdaList}) => System.Threading.Tasks.Task.FromResult(returns));");
            addHelper($"System.Func<{typeList}{genericType}> call",$"this._{methodName}(({nameList}) => System.Threading.Tasks.Task.FromResult(call({nameList})));");
        }

        return true;
    }

    private static bool TryBuildVoidMethodsWithoutParameters(CodeBuilder builder, IMethodSymbol method, Action<string, string> addHelper)
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

        builder.Add($$"""
                      
                      #region Method : void {{methodName}}({{parameterList}})
                      {{method.AccessibilityString()}} {{overrideString}} void {{methodName}}({{parameterList}})
                      {
                          this.{{functionPointer}}.Invoke({{nameList}});
                      }
                      internal System.Action {{functionPointer}} {get;set;} = ({{lambdaList}}) => {{BuildNotMockedException(method)}}

                      public partial class Config{
                          private Config _{{methodName}}(System.Action mock){
                                target.{{functionPointer}} = mock;
                                return this;
                          }
                      }
                      #endregion
                      """);

        addHelper($"System.Action call", $"this._{methodName}(call);");
        addHelper("System.Exception throws", $$"""this._{{methodName}}(({{lambdaList}}) => throw throws);""");
        addHelper("", $$"""this._{{methodName}}(({{lambdaList}}) => {});""");

        return true;
    }

    private static bool TryBuildVoidMethodsHavingParameters(CodeBuilder builder, IMethodSymbol method, Action<string, string> addHelper)
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

        builder.Add($$"""
                      
                      #region Method : void {{methodName}}({{parameterList}})
                      {{method.AccessibilityString()}} {{overrideString}} void {{methodName}}({{parameterList}})
                      {
                          this.{{functionPointer}}.Invoke({{nameList}});
                      }
                      internal System.Action<{{typeList}}> {{functionPointer}} {get;set;} = ({{parameterList}}) => {{BuildNotMockedException(method)}}

                      public partial class Config{
                          private Config _{{methodName}}(System.Action<{{typeList}}> mock){
                              target.{{functionPointer}} = mock;
                              return this;
                          }
                      }
                      #endregion
                      """);
        addHelper($"System.Action<{typeList}> call", $"this._{methodName}(call);");
        addHelper("System.Exception throws", $$"""this._{{methodName}}(({{parameterList}}) => throw throws);""");
        addHelper("", $$"""this._{{methodName}}(({{parameterList}}) => {});""");

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

    private static bool HasParameters(this IMethodSymbol method) => method.Parameters.Length >0;

    private static string BuildNotMockedException(this IMethodSymbol symbol) => $"throw new System.InvalidOperationException(\"The method '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";

    private static bool IsTask(this INamedTypeSymbol methodReturnType) => methodReturnType.ToString().Equals("System.Threading.Tasks.Task");

    private static bool IsGenericTask(this INamedTypeSymbol methodReturnType) => methodReturnType.ToString().StartsWith("System.Threading.Tasks.Task<") && methodReturnType.TypeArguments.Length > 0;
}

public class MethodSignature
{
    public string Signature { get; }
    public string Code { get; }

    public MethodSignature(string signature, string code)
    {
        this.Signature = signature;
        this.Code = code;
    }
}
