namespace MiniMock;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Builders;
using Microsoft.CodeAnalysis;

[Generator]
public sealed class MiniMockGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx => ctx.AddSource(
            "MiniMockAttribute.g.cs", Constants.MockAttributeCode));

        var enums = context.SyntaxProvider.ForAttributeWithMetadataName("MiniMock.Mock`1",
                (syntaxNode, _) => syntaxNode is SyntaxNode, this.GetAttributes)
            .Where(static enumData => enumData is not null)
            .SelectMany((enumerable, _) => enumerable)
            .Collect();

        context.RegisterSourceOutput(enums, this.Build);
    }

    private void Build(SourceProductionContext arg1, ImmutableArray<AttributeData> arg2)
    {
        var implemented = new List<ISymbol>();
        var l = arg2.ToLookup(FirstGenericType, a => a, SymbolEqualityComparer.Default).Where(t => t.Key != null);

        foreach (var l2 in l)
        {
            var symbol = l2.Key;
            try
            {
                var source = ClassBuilder.Build(symbol, arg1);

                var fileName = symbol.ToString().Replace("<", "_").Replace(">", "");

                arg1.AddSource(fileName + ".g.cs", source);
                implemented.Add(symbol);
            }
            catch (RefPropertyNotSupportedException e)
            {
                var t = l2.Select(t => t.ApplicationSyntaxReference?.GetSyntax().GetLocation()).Where(t => t != null);

                arg1.AddRefPropertyNotSupported(t, e.Message);
            }
            catch (RefReturnTypeNotSupportedException e)
            {
                var t = l2.Select(t => t.ApplicationSyntaxReference?.GetSyntax().GetLocation()).Where(t => t != null);

                arg1.AddRefPropertyNotSupported(t, e.Message);
            }
        }

        var mockClassSource = MockClassBuilder.Build(implemented, arg1);
        arg1.AddSource("Mock.g.cs", mockClassSource);
    }

    private static ITypeSymbol? FirstGenericType(AttributeData t) => t.AttributeClass?.TypeArguments.FirstOrDefault();

    private IEnumerable<AttributeData> GetAttributes(GeneratorAttributeSyntaxContext arg1, CancellationToken arg2) =>
        arg1.Attributes;
}

internal class UnsupportedAccessibilityException(Accessibility accessibility)
    : Exception($"Unsupported accessibility type '{accessibility}'");

internal class RefPropertyNotSupportedException(IPropertySymbol propertySymbol, ITypeSymbol typeSymbol) : Exception($"Ref property not supported for '{propertySymbol.Name}' in '{typeSymbol.Name}'" );
internal class RefReturnTypeNotSupportedException(IMethodSymbol methodSymbol, ITypeSymbol typeSymbol) : Exception($"Ref return type not supported for '{methodSymbol.Name}' in '{typeSymbol.Name}'" );
