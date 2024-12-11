namespace MiniMock.Builders;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal interface ISymbolBuilder
{
    bool TryBuild(CodeBuilder builder, IGrouping<string, ISymbol> symbols);
}
