namespace MiniMock.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

internal static class MethodBuilder
{
    public static void Build(CodeBuilder builder, IMethodSymbol method, bool isOverloaded, int index)
    {
        var parameterList = method.ToParameterList(p => $"{p.Type} {p.Name}");
        var typeList = method.ToParameterList(p => $"{p.Type}");
        if (method.Parameters.Length > 0 && !method.ReturnsVoid)
        {
            typeList += ", ";
        }

        var nameList = method.ToParameterList(p => $"{p.Name}");
        var lambdaList = method.ToParameterList(_ => "_");

        var overrideString = method.OverrideString();

        var methodName = method.Name;
        var methodReturnType = (INamedTypeSymbol)method.ReturnType;

        var functionPointer = "On_" + methodName;
        if (isOverloaded)
            functionPointer = functionPointer + "_" + index;

        builder.AddStartRegion(methodReturnType, methodName, parameterList);

        if (method.ReturnsVoid)
        {
            if (method.Parameters.Length == 0)
            {
                builder.AddConfigStart();

                builder.Add($$"""
                              internal System.Action {{functionPointer}} {get;set;} = ({{lambdaList}}) => {{BuildNotMockedException(method)}}
                              private Config _{{methodName}}(System.Action mock){
                                    this.{{functionPointer}} = mock;
                                    return this;
                              }
                              
                              public Config {{methodName}}(System.Action call) => this._{{methodName}}(call);
                              """);

                if (!isOverloaded)
                {
                    builder.AddThrow(methodName, lambdaList);
                }

                builder.AddConfigEnd();

                builder.Add($$"""
                              {{method.AccessibilityString()}} {{overrideString}}{{methodReturnType}} {{methodName}}({{parameterList}})
                              {
                                  _MockConfig._CallEvents.Add(this.GetType().Name, "{{methodName}}", MiniMock.CallEventType.Call);
                                  _MockConfig.{{functionPointer}}.Invoke({{nameList}});
                              }
                              """);
            }
            else
            {
                builder.AddConfigStart();

                builder.Add($$"""
                              internal System.Action<{{typeList}}> {{functionPointer}} {get;set;} = ({{lambdaList}}) => {{BuildNotMockedException(method)}}
                              private Config _{{methodName}}(System.Action<{{typeList}}> mock){
                                  this.{{functionPointer}} = mock;
                                  return this;
                              }
                              
                              public Config {{methodName}}(System.Action<{{typeList}}> call) => this._{{methodName}}(call);
                              """);

                if (!isOverloaded)
                {
                    builder.AddThrow(methodName, lambdaList);
                }

                builder.AddConfigEnd();

                builder.Add($$"""
                              {{method.AccessibilityString()}} {{overrideString}}{{methodReturnType}} {{methodName}}({{parameterList}})
                              {
                                  _MockConfig._CallEvents.Add(this.GetType().Name, "{{methodName}}", MiniMock.CallEventType.Call);
                                  _MockConfig.{{functionPointer}}.Invoke({{nameList}});
                              }
                              """);
            }
        }
        else
        {
            builder.AddConfigStart();

            builder.Add($$"""
                          internal System.Func<{{typeList}}{{methodReturnType}}> {{functionPointer}} {get;set;} = ({{lambdaList}}) => {{BuildNotMockedException(method)}}
                          private Config _{{methodName}}(System.Func<{{typeList}}{{methodReturnType}}> call){
                              this.{{functionPointer}} = call;
                              return this;
                          }
                          
                          public Config {{methodName}}(System.Func<{{typeList}}{{methodReturnType}}> call) => this._{{methodName}}(call);
                          """);

            if (!isOverloaded)
            {
                builder.AddThrow(methodName, lambdaList);

                builder.Add($$"""public Config {{methodName}}({{methodReturnType}} returns) => this._{{methodName}}(({{lambdaList}}) => returns);""");

                if (methodReturnType.IsTask())
                {
                    var typeList2 = method.ToParameterList(p => $"{p.Type}");
                    builder.Add($$"""public Config {{methodName}}(System.Action<{{typeList2}}> call) => this._{{methodName}}(({{nameList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""");
                }

                if (methodReturnType.IsGenericTask())
                {
                    var genericType = ((INamedTypeSymbol)method.ReturnType).TypeArguments.First();
                    builder.Add($$"""public Config {{methodName}}({{genericType}} returns) => this._{{methodName}}(({{lambdaList}}) => System.Threading.Tasks.Task.FromResult(returns));""");
                    builder.Add($$"""public Config {{methodName}}(System.Func<{{typeList}}{{genericType}}> call) => this._{{methodName}}(({{nameList}}) => System.Threading.Tasks.Task.FromResult(call({{nameList}})));""");
                }
            }

            builder.AddConfigEnd();

            builder.Add($$"""

                          {{method.AccessibilityString()}} {{overrideString}}{{methodReturnType}} {{methodName}}({{parameterList}})
                          {
                              _MockConfig._CallEvents.Add(this.GetType().Name, "{{methodName}}", MiniMock.CallEventType.Call);
                              return _MockConfig.{{functionPointer}}.Invoke({{nameList}});
                          }
                          """);
        }

        builder.AddEndRegion();
    }

    private static string BuildNotMockedException(this IMethodSymbol symbol) => $"throw new System.InvalidOperationException(\"The method '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";

    private static bool IsTask(this INamedTypeSymbol methodReturnType) => methodReturnType.ToString().Equals("System.Threading.Tasks.Task");

    private static bool IsGenericTask(this INamedTypeSymbol methodReturnType) => methodReturnType.ToString().StartsWith("System.Threading.Tasks.Task<") && methodReturnType.TypeArguments.Length > 0;

    private static void AddThrow(this CodeBuilder builder, string methodName, string lambdaList) => builder.Add($$"""public Config {{methodName}}(System.Exception throws) => this._{{methodName}}(({{lambdaList}}) => throw throws);""");

    private static void AddConfigStart(this CodeBuilder builder) => builder.Add("public partial class Config{").Indent();

    private static void AddConfigEnd(this CodeBuilder builder) => builder.Unindent().Add("}").Add();

    private static void AddStartRegion(this CodeBuilder builder, ITypeSymbol methodReturnType, string methodName,
        string parameterList) =>
        builder.Add().Add($$"""#region {{methodReturnType}} {{methodName}}({{parameterList}})""");

    private static void AddEndRegion(this CodeBuilder builder) => builder.Add("#endregion").Add();

    public static void BuildMethods(CodeBuilder builder, IEnumerable<IMethodSymbol> methodSymbols)
    {
        var isOverloaded = methodSymbols.Count() > 1;

        int index = 0;
        foreach (var symbol in methodSymbols)
        {
            index++;
            Build(builder, symbol, isOverloaded, index);
        }

        if (isOverloaded)
        {
            BuildOverloadHelpers(builder, methodSymbols);
        }

    }

    private static void BuildOverloadHelpers(CodeBuilder builder, IEnumerable<IMethodSymbol> methodSymbols)
    {
        var name = methodSymbols.First().Name;

        builder.Add("public partial class Config {").Indent();



        builder.Add($$"""public Config {{name}}(System.Exception throws) {""").Indent();

        var index = 0;
        foreach (var symbol in methodSymbols)
        {
            index++;
            var lambdaList = symbol.ToParameterList(p => $"{p.Type} _");

            builder.Add($$"""
                          this.On_{{name}}_{{index}} = (({{lambdaList}}) => throw throws);
                          """);
        }

        builder.Add("return this;").Unindent().Add("}");


        var rt = methodSymbols.ToLookup(t => t.ReturnType).Where(t => t.Key.ToString() != "void");
        foreach (var t in rt)
        {
            builder.Add($$"""public Config {{name}}({{t.Key}} value) {""").Indent();

            foreach (var symbol in t)
            {
                var lambdaList = symbol.ToParameterList(p => $"{p.Type} _");

                builder.Add($"this.{name}(({lambdaList}) => value);");
            }

            builder.Add("return this;").Unindent().Add("}");

        }


        builder.Unindent().Add("}");
    }
}

