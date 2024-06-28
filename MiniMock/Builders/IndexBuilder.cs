namespace MiniMock.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal static class IndexBuilder
{
//    private static int indexerCount;

    public static void BuildIndexes(CodeBuilder builder, IEnumerable<IPropertySymbol> indexerSymbols)
    {
        var helpers = new List<MethodSignature>();
        var symbols = indexerSymbols as IPropertySymbol[] ?? indexerSymbols.ToArray();
        var indexType = symbols.First().Parameters[0].Type.ToString();

        builder.Add($"#region Indexer : this[{indexType}]");

        int indexerCount = 0;
        foreach (var symbol in symbols)
        {
            indexerCount++;
            BuildIndex(builder, symbol, helpers, indexerCount);
        }

        helpers.BuildHelpers(builder, "Indexer");

        builder.Add("#endregion");
    }


    internal static void BuildIndex(CodeBuilder builder, IPropertySymbol symbol, List<MethodSignature> helpers, int indexerCount)
    {
        var returnType = symbol.Type.ToString();
        var indexType = symbol.Parameters[0].Type.ToString();
        var exception = BuildNotMockedException(symbol);

        var (containingSymbol, accessibilityString, overrideString) = symbol.Overwrites();

        var hasGet = symbol.GetMethod != null;
        var hasSet = symbol.SetMethod != null;

        builder.Add($$"""
                      {{accessibilityString}} {{returnType}} {{containingSymbol}}this[{{indexType}} index]
                      {
                      """).Indent();
        builder.Add(hasGet, () => $"get => this.On_IndexGet_{indexerCount}(index);");
        builder.Add(hasSet, () => $"set => this.On_IndexSet_{indexerCount}(index, value);");
        builder.Unindent().Add($$"""
                          }
                      
                          private System.Func<{{indexType}}, {{returnType}}> On_IndexGet_{{indexerCount}} { get; set; } = (_) => {{exception}}
                          private System.Action<{{indexType}}, {{returnType}}> On_IndexSet_{{indexerCount}} { get; set; } = (_, _) => {{exception}}

                      """);

        var dictionarySource = $"""
                                target.On_IndexGet_{indexerCount} = s => values[s];
                                target.On_IndexSet_{indexerCount} = (s, v) => values[s] = v;
                                """;
        helpers.Add(new($"System.Collections.Generic.Dictionary<{indexType}, {returnType}> values",
            dictionarySource,
            "Gets and sets values in the dictionary when the indexer is called."));

        var getSetFunctions = $"""
                               target.On_IndexGet_{indexerCount} = get;
                               target.On_IndexSet_{indexerCount} = set;
                               """;
        helpers.Add(new($"System.Func<{indexType}, {returnType}> get, System.Action<{indexType}, {returnType}> set",
            getSetFunctions,
            $"Specifies a getter and setter method to call when the indexer for <see cref=\"{indexType}\"/> is called."));
    }

    private static string BuildNotMockedException(this IPropertySymbol symbol) =>
        $"throw new System.InvalidOperationException(\"The indexer '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";
}
