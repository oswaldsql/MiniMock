namespace MiniMock.Builders;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Util;

/// <summary>
/// Represents a builder for indexers, implementing the ISymbolBuilder interface.
/// </summary>
internal class IndexBuilder : ISymbolBuilder
{
    /// <inheritdoc />
    public bool TryBuild(CodeBuilder builder, IGrouping<string, ISymbol> symbols)
    {
        var first = symbols.First();

        if (first is not IPropertySymbol { IsIndexer: true })
        {
            return false;
        }

        return BuildIndexes(builder, symbols.OfType<IPropertySymbol>().Where(t => t.IsIndexer));
    }

    /// <summary>
    ///     Builds the indexers and adds them to the code builder.
    /// </summary>
    /// <param name="builder">The code builder to add the indexers to.</param>
    /// <param name="indexerSymbols">The collection of indexer symbols to build.</param>
    /// <returns>True if any indexers were built; otherwise, false.</returns>
    private static bool BuildIndexes(CodeBuilder builder, IEnumerable<IPropertySymbol> indexerSymbols)
    {
        var helpers = new List<HelperMethod>();
        var symbols = indexerSymbols as IPropertySymbol[] ?? indexerSymbols.ToArray();
        var indexType = symbols.First().Parameters[0].Type.ToString();

        builder.Add($"#region Indexer : this[{indexType}]");

        var indexerCount = 0;
        foreach (var symbol in symbols)
        {
            indexerCount++;
            BuildIndex(builder, symbol, helpers, indexerCount);
        }

        builder.Add(helpers.BuildHelpers("Indexer"));

        builder.Add("#endregion");

        return indexerCount > 0;
    }

    /// <summary>
    ///     Builds a single indexer and adds it to the code builder.
    /// </summary>
    /// <param name="builder">The code builder to add the indexer to.</param>
    /// <param name="symbol">The property symbol representing the indexer.</param>
    /// <param name="helpers">The list of helper methods to add to.</param>
    /// <param name="indexerCount">The count of indexers built so far.</param>
    private static void BuildIndex(CodeBuilder builder, IPropertySymbol symbol, List<HelperMethod> helpers, int indexerCount)
    {
        var returnType = symbol.Type.ToString();
        var indexType = symbol.Parameters[0].Type.ToString();
        var exception = symbol.BuildNotMockedExceptionForIndexer();

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

        helpers.AddRange(BuildHelpers(symbol, indexerCount));
    }

    /// <summary>
    ///     Builds helper methods for the indexer.
    /// </summary>
    /// <param name="symbol">The property symbol representing the indexer.</param>
    /// <param name="indexerCount">The count of indexers built so far.</param>
    /// <returns>A collection of helper methods for the indexer.</returns>
    private static IEnumerable<HelperMethod> BuildHelpers(IPropertySymbol symbol, int indexerCount)
    {
        var hasGet = symbol.GetMethod != null;
        var hasSet = symbol.SetMethod != null;
        var returnType = symbol.Type.ToString();
        var indexType = symbol.Parameters[0].Type.ToString();
        var seeCref = symbol.ToString();

        yield return new HelperMethod($"System.Collections.Generic.Dictionary<{indexType}, {returnType}> values",
            $"""
             target.On_IndexGet_{indexerCount} = s => values[s];
             target.On_IndexSet_{indexerCount} = (s, v) => values[s] = v;
             """,
            "Gets and sets values in the dictionary when the indexer is called.", seeCref);

        switch (hasSet, hasGet)
        {
            case (true, true):
                yield return new HelperMethod($"System.Func<{indexType}, {returnType}> get, System.Action<{indexType}, {returnType}> set",
                    $"target.On_IndexGet_{indexerCount} = get;target.On_IndexSet_{indexerCount} = set;",
                    $"Specifies a getter and setter method to call when the indexer for <see cref=\"{indexType}\"/> is called.", seeCref);
                break;
            case (true, false):
                yield return new HelperMethod($"System.Action<{indexType}, {returnType}> set",
                    $"target.On_IndexSet_{indexerCount} = set;",
                    $"Specifies a setter method to call when the indexer for <see cref=\"{indexType}\"/> is called.", seeCref);
                break;
            case (false, true):
                yield return new HelperMethod($"System.Func<{indexType}, {returnType}> get",
                    $"target.On_IndexGet_{indexerCount} = get;",
                    $"Specifies a getter method to call when the indexer for <see cref=\"{indexType}\"/> is called.", seeCref);
                break;
        }
    }
}
