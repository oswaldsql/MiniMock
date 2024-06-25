namespace MiniMock.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal static class IndexBuilder
{
    private static int IndexerCount = 0;
    
    public static void BuildIndexes(CodeBuilder builder, IEnumerable<IPropertySymbol> indexerSymbols)
    {
        var helpers = new List<MethodSignature>();

        void AddHelper(string signature, string code) => helpers.Add(new(signature, code));

        foreach (var symbol in indexerSymbols)
        {
            BuildIndex(builder, symbol, AddHelper);
        }

        BuildHelpers(builder, helpers, "Indexer");
    }

    internal static void BuildIndex(CodeBuilder builder, IPropertySymbol indx, Action<string, string> addHelper)
    {
        IndexerCount++;

        var returnType = indx.Type.ToString();
        var indexType = indx.Parameters[0].Type.ToString();
        var exception = BuildNotMockedException(indx);

        var containingSymbol = "";
        var accessibilityString = indx.AccessibilityString();
        if (indx.ContainingType.TypeKind == TypeKind.Interface)
        {
            containingSymbol = indx.ContainingSymbol.ToString() + ".";
            accessibilityString = "";
        }

        var hasGet = indx.GetMethod != null;
        var hasSet = indx.SetMethod != null;

        builder.Add($$"""
                      #region Indexer : {{returnType}} this[{{indexType}}]
                      {{accessibilityString}} {{returnType}} {{containingSymbol}}this[{{indexType}} index]
                      {
                      """);
        builder.Add(hasGet, () => $"get => this.On_IndexGet_{IndexerCount}(index);");
        builder.Add(hasSet, () => $"set => this.On_IndexSet_{IndexerCount}(index, value);");
        builder.Add($$"""
                          }
                      
                          internal System.Func<{{indexType}}, {{returnType}}> On_IndexGet_{{IndexerCount}} { get; set; } = (_) => {{exception}}
                          internal System.Action<{{indexType}}, {{returnType}}> On_IndexSet_{{IndexerCount}} { get; set; } = (_, _) => {{exception}}

                      #endregion
                      """);

        addHelper($"System.Collections.Generic.Dictionary<{indexType}, {returnType}> values", $"target.On_IndexGet_{IndexerCount} = s => values[s];");
        addHelper($"System.Collections.Generic.Dictionary<{indexType}, {returnType}> values", $"target.On_IndexSet_{IndexerCount} = (s, v) => values[s] = v;");
        addHelper($"System.Func<{indexType}, {returnType}> get, System.Action<{indexType}, {returnType}> set", $"target.On_IndexGet_{IndexerCount} = get;");
        addHelper($"System.Func<{indexType}, {returnType}> get, System.Action<{indexType}, {returnType}> set", $"target.On_IndexSet_{IndexerCount} = set;");
    }

    private static string BuildNotMockedException(this IPropertySymbol symbol) => $"throw new System.InvalidOperationException(\"The indexer '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";

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
            foreach (var code in grouping.Select(t => t.Code).Distinct())
            {
                builder.Add(code);
            }

            builder.Unindent().Add("    return this;");
            builder.Add("}");
            builder.Add();
        }

        builder.Unindent().Add("}");
    }

}
