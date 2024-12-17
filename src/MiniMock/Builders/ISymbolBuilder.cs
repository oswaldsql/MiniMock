namespace MiniMock.Builders;

using System.Linq;
using Microsoft.CodeAnalysis;

/// <summary>
///     Interface for building code based on a grouping of symbols.
/// </summary>
internal interface ISymbolBuilder
{
    /// <summary>
    ///     Attempts to build a symbol using the provided CodeBuilder and a grouping of symbols.
    /// </summary>
    /// <param name="builder">The CodeBuilder instance to append the result to.</param>
    /// <param name="symbols">A grouping of symbols to be used in the building process.</param>
    /// <returns>True if the symbol was successfully built; otherwise, false.</returns>
    public bool TryBuild(CodeBuilder builder, IGrouping<string, ISymbol> symbols);
}
