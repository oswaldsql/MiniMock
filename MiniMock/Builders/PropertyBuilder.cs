namespace MiniMock.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal static class PropertyBuilder
{
    private static int propertyCount = 0;

    public static void BuildProperties(CodeBuilder builder, IEnumerable<IPropertySymbol> propertySymbols)
    {
        var enumerable = propertySymbols as IPropertySymbol[] ?? propertySymbols.ToArray();
        var name = enumerable.First().Name;
        var helpers = new List<MethodSignature>();

        void AddHelper(string signature, string code) => helpers.Add(new(signature, code));

        foreach (var symbol in propertySymbols)
        {
            BuildProperty(builder, symbol, AddHelper);
        }

        BuildHelpers(builder, helpers, name);
    }

    internal static void BuildProperty(CodeBuilder builder, IPropertySymbol property, Action<string, string> addHelper)
    {
        propertyCount++;

        var propertyName = property.Name;
        var type = property.Type.ToString();
        var setType = property.SetMethod?.IsInitOnly == true ? "init" : "set";

        var overrideString = "";
        if (property.ContainingType.TypeKind == TypeKind.Class)
        {
            if (property.IsAbstract)
            {
                overrideString = "override ";
            }
            else if (property.IsVirtual)
            {
                overrideString = "override ";
            }
        }

        var containingSymbol = "";
        var accessibilityString = property.AccessibilityString();
        if (property.ContainingType.TypeKind == TypeKind.Interface)
        {
            containingSymbol = property.ContainingSymbol.ToString() + ".";
            accessibilityString = "";
        }
        else
        {

        }

        builder.Add($$"""

                      #region Property : {{propertyName}}
                      {{accessibilityString}} {{overrideString}}{{type}} {{containingSymbol}}{{propertyName}}
                      {
                      """);

        if (property.GetMethod != null)
        {
            builder.Add($$"""
                              get {
                                  return this.Get_{{propertyName}}_{{propertyCount}}();
                              }
                          """);
        }

        if (property.SetMethod != null)
        {
            builder.Add($$"""
                              {{setType}} { 
                                  this.Set_{{propertyName}}_{{propertyCount}}(value);
                              }
                          """);
        }

        builder.Add("""}""");

        
        builder.Add($$"""
                      internal {{type.TrimEnd('?')}}? internal_{{propertyName}}_{{propertyCount}};
                      internal System.Func<{{type}}> Get_{{propertyName}}_{{propertyCount}} { get; set; } = () => {{BuildNotMockedException(property)}}
                      internal System.Action<{{type}}> Set_{{propertyName}}_{{propertyCount}} { get; set; } = s => {{BuildNotMockedException(property)}}

                      #endregion
                      """);

        addHelper($"{type.Replace("?", "")} value", $"target.internal_{propertyName}_{propertyCount} = value;");
        addHelper($"{type.Replace("?", "")} value", $"target.Get_{propertyName}_{propertyCount} = () => target.internal_{propertyName}_{propertyCount};");
        addHelper($"{type.Replace("?", "")} value", $"target.Set_{propertyName}_{propertyCount} = s => target.internal_{propertyName}_{propertyCount} = s;");
        addHelper($"System.Func<{type}> get, System.Action<{type}> set", $"target.Get_{propertyName}_{propertyCount} = get;");
        addHelper($"System.Func<{type}> get, System.Action<{type}> set", $"target.Set_{propertyName}_{propertyCount} = set;");
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

    private static string BuildNotMockedException(this IPropertySymbol symbol) => $"throw new System.InvalidOperationException(\"The property '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";
}
