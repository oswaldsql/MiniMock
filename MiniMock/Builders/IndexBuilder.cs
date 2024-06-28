namespace MiniMock.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal static class IndexBuilder
{
    private static int indexerCount;

    public static void BuildIndexes(CodeBuilder builder, IEnumerable<IPropertySymbol> indexerSymbols)
    {
        var helpers = new List<MethodSignature>();

        void AddHelper(string signature, string code, string documentation)
        {
            helpers.Add(new(signature, code, documentation));
        }

        foreach (var symbol in indexerSymbols)
        {
            BuildIndex(builder, symbol, AddHelper);
        }

        BuildHelpers(builder, helpers, "Indexer");
    }

    internal static void BuildIndex(CodeBuilder builder, IPropertySymbol indx, Action<string, string, string> addHelper)
    {
        indexerCount++;

        var returnType = indx.Type.ToString();
        var indexType = indx.Parameters[0].Type.ToString();
        var exception = BuildNotMockedException(indx);

        var containingSymbol = "";
        var accessibilityString = indx.AccessibilityString();
        if (indx.ContainingType.TypeKind == TypeKind.Interface)
        {
            containingSymbol = indx.ContainingSymbol + ".";
            accessibilityString = "";
        }

        var hasGet = indx.GetMethod != null;
        var hasSet = indx.SetMethod != null;

        builder.Add($$"""
                      #region Indexer : {{returnType}} this[{{indexType}}]
                      {{accessibilityString}} {{returnType}} {{containingSymbol}}this[{{indexType}} index]
                      {
                      """);
        builder.Add(hasGet, () => $"get => this.On_IndexGet_{indexerCount}(index);");
        builder.Add(hasSet, () => $"set => this.On_IndexSet_{indexerCount}(index, value);");
        builder.Add($$"""
                          }
                      
                          internal System.Func<{{indexType}}, {{returnType}}> On_IndexGet_{{indexerCount}} { get; set; } = (_) => {{exception}}
                          internal System.Action<{{indexType}}, {{returnType}}> On_IndexSet_{{indexerCount}} { get; set; } = (_, _) => {{exception}}

                      #endregion
                      """);

        addHelper($"System.Collections.Generic.Dictionary<{indexType}, {returnType}> values", $"target.On_IndexGet_{indexerCount} = s => values[s];", "Gets and sets values in the dictionary when the indexer is called.");
        addHelper($"System.Collections.Generic.Dictionary<{indexType}, {returnType}> values", $"target.On_IndexSet_{indexerCount} = (s, v) => values[s] = v;","");
        addHelper($"System.Func<{indexType}, {returnType}> get, System.Action<{indexType}, {returnType}> set", $"target.On_IndexGet_{indexerCount} = get;", $"Specifies a getter and setter method to call when the indexer for <see cref=\"{indexType}\"/> is called.");
        addHelper($"System.Func<{indexType}, {returnType}> get, System.Action<{indexType}, {returnType}> set", $"target.On_IndexSet_{indexerCount} = set;","");
    }

    private static string BuildNotMockedException(this IPropertySymbol symbol) =>
        $"throw new System.InvalidOperationException(\"The indexer '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";

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
