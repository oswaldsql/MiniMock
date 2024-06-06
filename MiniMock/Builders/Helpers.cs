namespace MiniMock.Builders;

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using MiniMock;

public static class Helpers
{
    public static string OverrideString(this ISymbol method)
    {
        var overrideString = "";
        if (method.ContainingType.TypeKind == TypeKind.Class)
        {
            if (method.IsAbstract)
            {
                overrideString = "override ";
            }
            else if (method.IsVirtual)
            {
                overrideString = "override ";
            }
        }

        return overrideString;
    }

    public static string ToParameterList(this IMethodSymbol m, Func<IParameterSymbol, string> selector)
    {
        var Parameters = m.Parameters.Select(selector);
        var parameterList = string.Join(", ", Parameters);
        return parameterList;
    }

    public static string AccessibilityString(this ISymbol method) =>
        method.DeclaredAccessibility.AccessibilityString();

    public static string AccessibilityString(this Accessibility accessibility) =>
        accessibility switch
        {
            Accessibility.Internal => "internal",
            Accessibility.NotApplicable => throw new UnsupportedAccessibilityException(accessibility),
            Accessibility.Private => "private",
            Accessibility.ProtectedAndInternal => "protected internal",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.Public => "public",
            _ => throw new UnsupportedAccessibilityException(accessibility)
        };
}
