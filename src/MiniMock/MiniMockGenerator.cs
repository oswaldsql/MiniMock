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
            "MiniMock.MockAttribute.g.cs", Constants.MockAttributeCode));

        var enums = context.SyntaxProvider.ForAttributeWithMetadataName("MiniMock.MockAttribute`1",
                (syntaxNode, _) => syntaxNode != null, this.GetAttributes)
            .Where(static enumData => enumData is not null)
            .SelectMany((enumerable, _) => enumerable)
            .Collect();

        context.RegisterSourceOutput(enums, this.Build);
    }

    private void Build(SourceProductionContext context, ImmutableArray<AttributeData> attributes)
    {
        var implemented = new List<ISymbol>();
        var sources = attributes.ToLookup(FirstGenericType, a => a, SymbolEqualityComparer.Default).Where(t => t.Key != null);

        foreach (var source in sources)
        {
            if(source.Key != null)
            {
                try
                {
                    var code = ClassBuilder.Build(source.Key);

                    var fileName = source.Key.ToString().Replace("<", "_").Replace(">", "").Replace(", ","_");

                    context.AddSource(fileName + ".g.cs", code);
                    implemented.Add(source.Key);
                }
                catch (RefPropertyNotSupportedException e)
                {
                    context.AddRefPropertyNotSupported(GetSourceLocations(source), e.Message);
                }
                catch (RefReturnTypeNotSupportedException e)
                {
                    context.AddRefReturnTypeNotSupported(GetSourceLocations(source), e.Message);
                }
                catch (GenericMethodNotSupportedException e)
                {
                    context.AddGenericMethodNotSupported(GetSourceLocations(source), e.Message);
                }
                catch (StaticAbstractMembersNotSupportedException e)
                {
                    context.AddStaticAbstractMembersNotSupported(GetSourceLocations(source), e.Message);
                }

            }
        }

        var mockClassSource = MockClassBuilder.Build(implemented, context);
        context.AddSource("Mock.g.cs", mockClassSource);
    }

    private static IEnumerable<Location> GetSourceLocations(IGrouping<ISymbol?, AttributeData> source) => source.Select(t => t.ApplicationSyntaxReference?.GetSyntax().GetLocation()!).Where(t => t != null);

    private static ITypeSymbol? FirstGenericType(AttributeData t)
    {
        var firstGenericType = t.AttributeClass?.TypeArguments.OfType<INamedTypeSymbol>().FirstOrDefault();
        return firstGenericType?.IsGenericType == true ? firstGenericType.OriginalDefinition : firstGenericType;
    }

    private IEnumerable<AttributeData> GetAttributes(GeneratorAttributeSyntaxContext arg1, CancellationToken arg2) =>
        arg1.Attributes;
}

internal class UnsupportedAccessibilityException(Accessibility accessibility) : Exception($"Unsupported accessibility type '{accessibility}'");
internal class RefPropertyNotSupportedException(IPropertySymbol propertySymbol, ITypeSymbol typeSymbol) : Exception($"Ref property not supported for '{propertySymbol.Name}' in '{typeSymbol.Name}'" );
internal class RefReturnTypeNotSupportedException(IMethodSymbol methodSymbol, ITypeSymbol typeSymbol) : Exception($"Ref return type not supported for '{methodSymbol.Name}' in '{typeSymbol.Name}'" );
internal class GenericMethodNotSupportedException(IMethodSymbol methodSymbol, ITypeSymbol typeSymbol) : Exception($"Generic methods in non generic interfaces or classes is not currently supported for '{methodSymbol.Name}' in '{typeSymbol.Name}'" );
internal class StaticAbstractMembersNotSupportedException(string name, ITypeSymbol typeSymbol) : Exception($"Static abstract members in interfaces or classes is not supported for '{name}' in '{typeSymbol.Name}'" );
