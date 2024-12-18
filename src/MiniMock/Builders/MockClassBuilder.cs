namespace MiniMock.Builders;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

public static class MockClassBuilder
{
    public static string Build(IEnumerable<ISymbol> typeSymbols, SourceProductionContext context)
    {
        var mocks = typeSymbols.OfType<INamedTypeSymbol>().OrderBy(t => t.Name).ToArray();

        if (!mocks.Any())
        {
            return "//No mocks found.";
        }

        var builder = new CodeBuilder();

        builder.Add($$"""
                      // Generated by MiniMock on {{DateTime.Now}}
                      #nullable enable
                      namespace MiniMock {
                      ->

                      /// <summary>
                      /// Factory for creating mock objects.
                      /// </summary>
                      internal static class Mock {
                      ->
                      """);

        foreach (var symbol in mocks)
        {
            bool AccessibilityFilter(Accessibility accessibility)
            {
                return accessibility is Accessibility.Public or Accessibility.Protected;
            }

            if (!symbol.Constructors.Any(t => !t.IsStatic))
            {
                BuildFactoryMethod(symbol, builder);
            }
            else
            {
                foreach (var constructor in symbol.Constructors.Where(t => AccessibilityFilter(t.DeclaredAccessibility) && !t.IsStatic))
                {
                    BuildFactoryMethod(symbol, builder, constructor);
                }
            }
        }

        builder.Add("""
                    <-
                    }
                    <-
                    }
                    """);

        return builder.ToString();
    }

    private static void BuildFactoryMethod(INamedTypeSymbol symbol, CodeBuilder builder, IMethodSymbol? constructor = null)
    {
        var p = constructor?.Parameters.Select(t => $"{t.Type} {t.Name}, ") ?? ImmutableArray<string>.Empty;
        var parameters = string.Join("", p);

        var n = constructor?.Parameters.Select(t => $"{t.Name}, ") ?? ImmutableArray<string>.Empty;
        var names = string.Join("", n);

        //var doc = constructor?.GetDocumentationCommentXml() ?? "/// DOC";
        //builder.Add(doc);

        var typeArguments = symbol.TypeArguments;
        var containingNamespace = symbol.ContainingNamespace;
        var symbolName = symbol.Name;

        var cref = symbol.ToString().Replace('<', '{').Replace('>', '}');

        builder.Add(
            $"""

             /// <summary>
             ///     Creates a mock object for <see cref="{cref}"/>.
             /// </summary>
             """);

        if (constructor != null)
        {
            foreach (var o in constructor.Parameters)
            {
                builder.Add($"///     <param name=\"{o.Name}\">Base constructor parameter {o.Name}.</param>");
            }
        }

        builder.Add(
            $"""
             ///     <param name="config">Optional configuration for the mock object.</param>
             /// <returns>The mock object for <see cref="{cref}"/>.</returns>
             """);

        if (typeArguments.Length > 0)
        {
            var types = string.Join(", ", typeArguments.Select(t => t.Name));
            var name = $"MockOf_{symbolName}<{types}>";
            var constraints = typeArguments.ToConstraints();

            builder.Add($"""
                         internal static {symbol} {symbolName}<{types}>
                             ({parameters}System.Action<{containingNamespace}.{name}.Config>? config = null)
                                 {constraints}
                             => {containingNamespace}.{name}.Create({names}config);
                         """);
        }
        else
        {
            var name = "MockOf_" + symbolName;
            builder.Add($"""
                         internal static {symbol} {symbolName}
                             ({parameters}System.Action<{containingNamespace}.{name}.Config>? config = null)
                             => {containingNamespace}.{name}.Create({names}config);
                         """);
        }
    }
}
