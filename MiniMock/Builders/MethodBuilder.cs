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
        for (var i = 0; i < enumerable.Length; i++)
        {
            var symbol = enumerable[i];
            helpers.AddRange(Build(builder, symbol, i));
        }

        var signatures = helpers.ToLookup(t => t.Signature);

        builder.Add("public partial class Config {").Indent();

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

    private static IEnumerable<MethodSignature> Build(CodeBuilder builder, IMethodSymbol method, int index)
    {
        var parameterList = method.ToString(p => $"{p.Type} {p.Name}");
        var typeList = method.ToString(p => $"{p.Type}");
        if (method.Parameters.Length > 0 && !method.ReturnsVoid)
        {
            typeList += ", ";
        }

        var nameList = method.ToString(p => $"{p.Name}");
        var lambdaList = method.ToString(p => $"{p.Type} _");

        var overrideString = method.OverrideString();

        var methodName = method.Name;
        var methodReturnType = (INamedTypeSymbol)method.ReturnType;

        var functionPointer = index == 0 ? "On_" + methodName : "On_" + methodName + "_" + index;

        if (method.ReturnsVoid)
        {
            if (method.HasParameters())
            {
                 return BuildVoidMethodsWithParamerets(builder, method, overrideString, methodReturnType, methodName, parameterList, functionPointer, nameList, typeList, lambdaList).ToArray();
                
            }
            else
            {
                 return BuildVoidMethodsWithoutParamerets(builder, method, overrideString, methodReturnType, methodName, parameterList, functionPointer, nameList, lambdaList).ToArray();
                
            }
        }
        else
        {
            return BuildNoneVoidMethods(builder, method, overrideString, methodReturnType, methodName, parameterList, functionPointer, nameList, typeList, lambdaList).ToArray();
            ;
            
        }
    }

    private static IEnumerable<MethodSignature> BuildNoneVoidMethods(CodeBuilder builder, IMethodSymbol method, string overrideString,
        INamedTypeSymbol methodReturnType, string methodName, string parameterList, string functionPointer, string nameList,
        string typeList, string lambdaList)
    {
        builder.Add($$$"""
                       
                       #region Method : {{{(ITypeSymbol)methodReturnType}}} {{{methodName}}}({{{parameterList}}})
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

        yield return new($"System.Func<{typeList}{methodReturnType}> call", $"this._{methodName}(call);");
        yield return new("System.Exception throws", $"this._{method.Name}(({lambdaList}) => throw throws);");
        yield return new(methodReturnType + " returns", $"this._{method.Name}(({lambdaList}) => returns);");

        if (methodReturnType.IsTask())
        {
            if (method.HasParameters())
            {
                var typeList2 = method.ToString(p => $"{p.Type}");
                yield return new($"System.Action<{typeList2}> call",$$"""this._{{methodName}}(({{parameterList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""");
                yield return new("",$$"""this._{{methodName}}(({{nameList}}) => {return System.Threading.Tasks.Task.CompletedTask;});""");
            }
            else
            {
                yield return new("System.Action call", $$"""this._{{methodName}}(({{nameList}}) => {call({{nameList}});return System.Threading.Tasks.Task.CompletedTask;});""");
                yield return new("", $$"""this._{{methodName}}(({{nameList}}) => {return System.Threading.Tasks.Task.CompletedTask;});""");
            }
        }

        if (methodReturnType.IsGenericTask())
        {
            var genericType = ((INamedTypeSymbol)method.ReturnType).TypeArguments.First();
            yield return new($$"""{{genericType}} returns""",$$"""this._{{methodName}}(({{lambdaList}}) => System.Threading.Tasks.Task.FromResult(returns));""");
            yield return new($$"""System.Func<{{typeList}}{{genericType}}> call""",$$"""this._{{methodName}}(({{nameList}}) => System.Threading.Tasks.Task.FromResult(call({{nameList}})));""");
        }
    }

    private static IEnumerable<MethodSignature> BuildVoidMethodsWithoutParamerets(CodeBuilder builder, IMethodSymbol method,
        string overrideString, INamedTypeSymbol methodReturnType, string methodName, string parameterList,
        string functionPointer, string nameList, string lambdaList)
    {
        builder.Add($$"""
                      
                      #region Method : {{(ITypeSymbol)methodReturnType}}{{methodName}}({{parameterList}})
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
                      }
                      #endregion
                      """);

        yield return new($"System.Action call", $"this._{methodName}(call);");
        yield return new("System.Exception throws", $$"""this._{{method.Name}}(({{lambdaList}}) => throw throws);""");
        yield return new("", $$"""this._{{method.Name}}(({{lambdaList}}) => {});""");
    }

    private static IEnumerable<MethodSignature> BuildVoidMethodsWithParamerets(CodeBuilder builder, IMethodSymbol method, string overrideString,
        INamedTypeSymbol methodReturnType, string methodName, string parameterList, string functionPointer, string nameList,
        string typeList, string lambdaList)
    {
        builder.Add($$"""
                      
                      #region Method : {{(ITypeSymbol)methodReturnType}}{{methodName}}({{parameterList}})
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
                      }
                      #endregion
                      """);
        yield return new($"System.Action<{typeList}> call", $"this._{methodName}(call);");
        yield return new("System.Exception throws", $$"""this._{{method.Name}}(({{lambdaList}}) => throw throws);""");
        yield return new("", $$"""this._{{method.Name}}(({{lambdaList}}) => {});""");
    }

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
