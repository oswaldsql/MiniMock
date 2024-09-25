namespace MiniMock.Builders;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal static class IndexBuilder
{
    public static void BuildIndexes(CodeBuilder builder, IEnumerable<IPropertySymbol> indexerSymbols)
    {
        var helpers = new List<MethodSignature>();
        var symbols = indexerSymbols as IPropertySymbol[] ?? indexerSymbols.ToArray();
        var indexType = symbols.First().Parameters[0].Type.ToString();

        builder.Add($"#region Indexer : this[{indexType}]");

        var indexerCount = 0;
        foreach (var symbol in symbols)
        {
            indexerCount++;
            BuildIndex(builder, symbol, helpers, indexerCount);
        }

        helpers.BuildHelpers(builder, "Indexer");

        builder.Add("#endregion");
    }

    private static void BuildIndex(CodeBuilder builder, IPropertySymbol symbol, List<MethodSignature> helpers, int indexerCount)
    {
        var returnType = symbol.Type.ToString();
        var indexType = symbol.Parameters[0].Type.ToString();
        var exception = BuildNotMockedException(symbol);

        var (containingSymbol, accessibilityString, _) = symbol.Overwrites();

        var hasGet = symbol.GetMethod != null;
        var hasSet = symbol.SetMethod != null;

        builder.Add($$"""{{accessibilityString}} {{returnType}} {{containingSymbol}}this[{{indexType}} index] {""").Indent();
        builder.Add(hasGet, () => $"get => this.On_IndexGet_{indexerCount}(index);");
        builder.Add(hasSet, () => $"set => this.On_IndexSet_{indexerCount}(index, value);").Unindent();
        builder.Add($$"""
                      }

                      private System.Func<{{indexType}}, {{returnType}}> On_IndexGet_{{indexerCount}} { get; set; } = (_) => {{exception}}
                      private System.Action<{{indexType}}, {{returnType}}> On_IndexSet_{{indexerCount}} { get; set; } = (_, _) => {{exception}}

                      """);

        var dictionarySource = $"""
                                target.On_IndexGet_{indexerCount} = s => values[s];
                                target.On_IndexSet_{indexerCount} = (s, v) => values[s] = v;
                                """;

        var seeCref = symbol.ToString();
        helpers.Add(new($"System.Collections.Generic.Dictionary<{indexType}, {returnType}> values",
            dictionarySource,
            "Gets and sets values in the dictionary when the indexer is called.", seeCref));

        if (hasSet && !hasGet)
        {
            helpers.Add(new($"System.Action<{indexType}, {returnType}> set",
                $"target.On_IndexSet_{indexerCount} = set;",
                $"Specifies a setter method to call when the indexer for <see cref=\"{indexType}\"/> is called.", seeCref));
        }
        else if(!hasSet && hasGet)
        {
            helpers.Add(new($"System.Func<{indexType}, {returnType}> get",
                $"target.On_IndexGet_{indexerCount} = get;",
                $"Specifies a getter method to call when the indexer for <see cref=\"{indexType}\"/> is called.", seeCref));
        }
        else
        {
            helpers.Add(new($"System.Func<{indexType}, {returnType}> get, System.Action<{indexType}, {returnType}> set",
                $"target.On_IndexGet_{indexerCount} = get;target.On_IndexSet_{indexerCount} = set;",
                $"Specifies a getter and setter method to call when the indexer for <see cref=\"{indexType}\"/> is called.", seeCref));
        }
    }

    private static string BuildNotMockedException(this IPropertySymbol symbol) =>
        $"throw new System.InvalidOperationException(\"The indexer '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";
}
