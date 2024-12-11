namespace MiniMock.Builders;

using System.Linq;
using Microsoft.CodeAnalysis;

internal interface ISymbolBuilder
{
    public bool TryBuild(CodeBuilder builder, IGrouping<string, ISymbol> symbols);
}
