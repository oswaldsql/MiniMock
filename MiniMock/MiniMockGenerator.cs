namespace MiniMock;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MiniMock.Builders;

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
            .SelectMany((enumerable, token) => enumerable)
            .Collect();

        context.RegisterSourceOutput(enums, this.Build);
    }

    private void Build(SourceProductionContext arg1, ImmutableArray<AttributeData> arg2)
    {
        var typeSymbols = arg2
            .Select(FirstGenericType)
            .Where(t => t != null)
            .Distinct(SymbolEqualityComparer.Default)
            .ToArray();

        foreach (var d in typeSymbols)
        {
            var source = ClassBuilder.Build(d, arg1);

            var fileName = d.ToString().Replace("<", "_").Replace(">", "");

            arg1.AddSource(fileName + ".g.cs", source);
        }

        var mockClassSource = MockClassBuilder.Build(typeSymbols, arg1);
        arg1.AddSource("Mock.g.cs", mockClassSource);
    }

    private static ITypeSymbol? FirstGenericType(AttributeData t) => t.AttributeClass?.TypeArguments.FirstOrDefault();

    private IEnumerable<AttributeData> GetAttributes(GeneratorAttributeSyntaxContext arg1, CancellationToken arg2) =>
        arg1.Attributes;
}

internal class UnsupportedAccessibilityException(Accessibility accessibility)
    : Exception($"Unsupported accessibility type '{accessibility}'");

