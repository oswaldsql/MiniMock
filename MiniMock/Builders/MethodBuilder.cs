namespace MiniMock.Builders;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal static class MethodBuilder
{
    public static void BuildMethods(CodeBuilder builder, IEnumerable<IMethodSymbol> methodSymbols)
    {
        var enumerable = methodSymbols as IMethodSymbol[] ?? methodSymbols.ToArray();

        var isOverloaded = enumerable.Count() > 1;

        var index = 0;
        foreach (var symbol in enumerable)
        {
            index++;
            Build(builder, symbol, isOverloaded, index);
        }

        if (isOverloaded)
        {
            BuildOverloadHelpers(builder, enumerable);
        }
    }

    private static void Build(CodeBuilder builder, IMethodSymbol method, bool isOverloaded, int index)
    {
        var parameterList = method.ToString(p => $"{p.Type} {p.Name}");
        var typeList = method.ToString(p => $"{p.Type}");
        if (method.Parameters.Length > 0 && !method.ReturnsVoid)
        {
            typeList += ", ";
        }

        var nameList = method.ToString(p => $"{p.Name}");
        var lambdaList = method.ToString(_ => "_");

        var overrideString = method.OverrideString();

        var methodName = method.Name;
        var methodReturnType = (INamedTypeSymbol)method.ReturnType;

        var functionPointer = isOverloaded ? "On_" + methodName + "_" + index : "On_" + methodName;

        builder.Add($$"""

                      #region Method : {{(ITypeSymbol)methodReturnType}} {{methodName}}({{parameterList}})
                      """);

        if (method.ReturnsVoid)
        {
            if (method.HasParameters())
            {
                builder.Add($$"""
                              {{method.AccessibilityString()}} {{overrideString}}{{methodReturnType}} {{methodName}}({{parameterList}})
                              {
                                  this.{{functionPointer}}.Invoke({{nameList}});
                              }
                              internal System.Action<{{typeList}}> {{functionPointer}} {get;set;} = ({{lambdaList}}) => {{BuildNotMockedException(method)}}

                              public partial class Config{
                                  private Config _{{methodName}}(System.Action<{{typeList}}> mock){
                                      target.{{functionPointer}} = mock;
                                      return this;
                                  }
                                  
                                  public Config {{methodName}}(System.Action<{{typeList}}> call) => this._{{methodName}}(call);
                              }
                              """);
            }
            else
            {
                builder.Add($$"""
                              {{method.AccessibilityString()}} {{overrideString}}{{methodReturnType}} {{methodName}}({{parameterList}})
                              {
                                  this.{{functionPointer}}.Invoke({{nameList}});
                              }
                              internal System.Action {{functionPointer}} {get;set;} = ({{lambdaList}}) => {{BuildNotMockedException(method)}}

                              public partial class Config{
                                  private Config _{{methodName}}(System.Action mock){
                                        target.{{functionPointer}} = mock;
                                        return this;
                                  }
                              
                                  public Config {{methodName}}(System.Action call) => this._{{methodName}}(call);
                              }
                              """);
            }
        }
        else
        {
            builder.Add($$"""
                          {{method.AccessibilityString()}} {{overrideString}}{{methodReturnType}} {{methodName}}({{parameterList}})
                          {
                              return this.{{functionPointer}}.Invoke({{nameList}});
                          }
                          internal System.Func<{{typeList}}{{methodReturnType}}> {{functionPointer}} {get;set;} = ({{lambdaList}}) => {{BuildNotMockedException(method)}}
                          
                          public partial class Config{
                              private Config _{{methodName}}(System.Func<{{typeList}}{{methodReturnType}}> call){
                                  target.{{functionPointer}} = call;
                                  return this;
                              }
                              
                              public Config {{methodName}}(System.Func<{{typeList}}{{methodReturnType}}> call) => this._{{methodName}}(call);
                          }
                          """);
        }

        if (!isOverloaded)
        {
            BuildHelpers(builder, method);
        }

        builder.Add("#endregion").Add();
    }

    private static bool HasParameters(this IMethodSymbol method) => method.Parameters.Length >0;

    private static string BuildNotMockedException(this IMethodSymbol symbol) => $"throw new System.InvalidOperationException(\"The method '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";

    private static bool IsTask(this INamedTypeSymbol methodReturnType) => methodReturnType.ToString().Equals("System.Threading.Tasks.Task");

    private static bool IsGenericTask(this INamedTypeSymbol methodReturnType) => methodReturnType.ToString().StartsWith("System.Threading.Tasks.Task<") && methodReturnType.TypeArguments.Length > 0;

    private static void BuildHelpers(CodeBuilder builder, IMethodSymbol method)
    {
        var typeList = method.ToString(p => $"{p.Type}");
        if (method.Parameters.Length > 0 && !method.ReturnsVoid)
        {
            typeList += ", ";
        }

        var nameList = method.ToString(p => $"{p.Name}");
        var lambdaList = method.ToString(_ => "_");

        var methodName = method.Name;
        var methodReturnType = (INamedTypeSymbol)method.ReturnType;

        builder.Add("public partial class Config{").Indent();

        builder.BuildTrowHelpers(methodName, method);

        if (!method.ReturnsVoid)
        {
            builder.BuildRawValueReturn(method);

            if (methodReturnType.IsTask())
            {
                if (method.HasParameters())
                {
                    var typeList2 = method.ToString(p => $"{p.Type}");
                    builder.Add($$"""public Config {{methodName}}(System.Action<{{typeList2}}> call) => this._{{methodName}}(({{nameList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""");
                    builder.Add($$"""public Config {{methodName}}() => this._{{methodName}}(({{nameList}}) => {return System.Threading.Tasks.Task.CompletedTask;});""");
                }
                else
                {
                    builder.Add($$"""public Config {{methodName}}(System.Action call) => this._{{methodName}}(({{nameList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""");
                    builder.Add($$"""public Config {{methodName}}() => this._{{methodName}}(({{nameList}}) => {return System.Threading.Tasks.Task.CompletedTask;});""");
                }
            }

            if (methodReturnType.IsGenericTask())
            {
                var genericType = ((INamedTypeSymbol)method.ReturnType).TypeArguments.First();
                builder.Add($$"""public Config {{methodName}}({{genericType}} returns) => this._{{methodName}}(({{lambdaList}}) => System.Threading.Tasks.Task.FromResult(returns));""");
                builder.Add($$"""public Config {{methodName}}(System.Func<{{typeList}}{{genericType}}> call) => this._{{methodName}}(({{nameList}}) => System.Threading.Tasks.Task.FromResult(call({{nameList}})));""");
            }
        }

        builder.Unindent().Add("}").Add();
    }

    private static void BuildOverloadHelpers(CodeBuilder builder, IEnumerable<IMethodSymbol> methodSymbols)
    {
        var enumerable = methodSymbols as IMethodSymbol[] ?? methodSymbols.ToArray();

        var name = enumerable.First().Name;

        builder.Add("public partial class Config {").Indent();

//        builder.Add("//------------------------- HERE ------------------");
        var signatures = BuildMethodSignatures(enumerable).ToLookup(t => t.Signature);

        foreach (var grouping in signatures)
        {
            builder.Add($"public Config {name}({grouping.Key}) {{");
            foreach (var mse in grouping)
            {
                builder.Add("    " + mse.Code);
            }

            builder.Add("    return this;");
            builder.Add(" }");
            builder.Add();
        }

        builder.Unindent().Add("}");
    }

    private static IEnumerable<MethodSignature> BuildMethodSignatures(IEnumerable<IMethodSymbol> methodSymbols)
    {
        foreach (var method in methodSymbols)
        {
            var methodName = method.Name;
            var methodReturnType = (INamedTypeSymbol)method.ReturnType;

            var lambdaList = method.ToString(p => $"{p.Type} _");
            yield return new("System.Exception throws", $$"""this.{{method.Name}}(({{lambdaList}}) => throw throws);""");

            if (method.ReturnsVoid)
            {
                yield return new("", $$"""this._{{method.Name}}(({{lambdaList}}) => {});""");
            }
            else
            {
                yield return new(methodReturnType + " returns", $$"""this._{{method.Name}}(({{lambdaList}}) => returns);""");
            }

            if (methodReturnType.IsTask())
            {
                var nameList = method.ToString(p => $"{p.Name}");
                var PList = method.ToString(p => $"{p.Type} {p.Name}");

                if (method.HasParameters())
                {
                    var typeList2 = method.ToString(p => $"{p.Type}");
                    yield return new ($"System.Action<{typeList2}> call", $$"""this._{{methodName}}(({{PList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""");
                    yield return new ("", $$"""this._{{methodName}}(({{lambdaList}}) => {return System.Threading.Tasks.Task.CompletedTask;});""");
                }
                else
                {
                    yield return new ("System.Action call", $$"""this._{{methodName}}(({{lambdaList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""");
                    yield return new ("", $$"""this._{{methodName}}(({{lambdaList}}) => {return System.Threading.Tasks.Task.CompletedTask;});""");
                }
            }

            if (methodReturnType.IsGenericTask())
            {
                var genericType = ((INamedTypeSymbol)method.ReturnType).TypeArguments.First();
                yield return new(genericType + " returns", $$"""this._{{methodName}}(({{lambdaList}}) => System.Threading.Tasks.Task.FromResult(returns));""");

                //var typeList = method.ToString(p => $"{p.Type}");
                //if (method.Parameters.Length > 0 && !method.ReturnsVoid)
                //{
                //    typeList += ", ";
                //}
                //var nameList = method.ToString(p => $"{p.Name}");

                //yield return new MS($"System.Func<{typeList}{genericType}> call", $$"""this._{{methodName}}(({{lambdaList}}) => System.Threading.Tasks.Task.FromResult(call({{nameList}}))); //c""");
            }
        }
    }

    private static void BuildRawValueReturn(this CodeBuilder builder, params IMethodSymbol[] enumerable)
    {
        var returnTypes = enumerable.ToLookup(t => t.ReturnType, comparer: SymbolEqualityComparer.Default).Where(t => t.Key?.ToString() != "void");

        foreach (var method in returnTypes)
        {
            var name = method.First().Name;
            builder.Add($$"""public Config {{name}}({{method.Key}} returns) {""").Indent();

            foreach (var symbol in method)
            {
                var lambdaList = symbol.ToString(p => $"{p.Type} _");

                builder.Add($"this.{name}(({lambdaList}) => returns);");
            }

            builder.Add("return this;").Unindent().Add("}");
        }
    }

    private static void BuildTrowHelpers(this CodeBuilder builder, string name, params IMethodSymbol[] enumerable)
    {
        builder.Add($$"""public Config {{name}}(System.Exception throws) {""").Indent();

        foreach (var method in enumerable)
        {
            var lambdaList = method.ToString(p => $"{p.Type} _");

            builder.Add($$"""this._{{name}}(({{lambdaList}}) => throw throws);""");
        }

        builder.Add("return this;").Unindent().Add("}");
    }
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
