namespace MiniMock.Builders;

using System;
using System.Collections.Generic;
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

    internal static void BuildHelpers(this List<MethodSignature> helpers, CodeBuilder builder, string name)
    {
        if (helpers.Count == 0)
        {
            return;
        }

        var signatures = helpers.ToLookup(t => t.Signature);

        builder.Add("public partial class Config {").Indent();

        foreach (var grouping in signatures)
        {
            builder.Add($"""

                         /// <summary>
                         """);
            grouping.Select(t => t.Documentation).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToList().ForEach(t => builder.Add("///     " + t));
            builder.Add($"""
                         /// </summary>
                         /// <returns>The updated configuration.</returns>
                         """);

            builder.Add($"public Config {name}({grouping.Key}) {{").Indent();
            foreach (var mse in grouping)
            {
                builder.Add(mse.Code);
            }

            builder.Unindent().Add("    return this;");
            builder.Add("}");
            builder.Add();
        }

        builder.Unindent().Add("}");
    }
}
