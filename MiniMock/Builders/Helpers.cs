namespace MiniMock.Builders;

using System;
using System.Linq;
using Microsoft.CodeAnalysis;

public static class Helpers
{
    public static string OverrideString(this ISymbol method)
    {
        if (method.ContainingType.TypeKind == TypeKind.Class && (method.IsAbstract || method.IsVirtual))
        {
            return "override ";
        }

        return "";
    }

    public static string ToString(this IMethodSymbol m, Func<IParameterSymbol, string> selector,
        string separator = ", ")
    {
        var Parameters = m.Parameters.Select(selector);
        var parameterList = string.Join(separator, Parameters);
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
