namespace MiniMock.Builders;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

public static class Helpers
{
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

            builder.Add("return this;").Unindent();
            builder.Add("}");
            builder.Add();
        }

        builder.Unindent().Add("}");
    }

    internal static string OutString(this IParameterSymbol parameterSymbol) =>
        parameterSymbol.RefKind switch
        {
            RefKind.Out => "out ",
            RefKind.Ref => "ref ",
            RefKind.In => "in ",
            RefKind.RefReadOnlyParameter => "ref readonly ",
            _ => ""
        };

    internal static bool HasParameters(this IMethodSymbol method) => method.Parameters.Length > 0;

    internal static string BuildNotMockedException(this IMethodSymbol symbol) => $"throw new System.InvalidOperationException(\"The method '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";

    internal static string BuildNotMockedException(this IPropertySymbol symbol) => $"throw new System.InvalidOperationException(\"The property '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";

    internal static bool IsReturningTask(this IMethodSymbol method) =>
        method.ReturnType.ToString().Equals("System.Threading.Tasks.Task");

    internal static bool IsReturningGenericTask(this IMethodSymbol method) =>
        method.ReturnType.ToString().StartsWith("System.Threading.Tasks.Task<") &&
        ((INamedTypeSymbol)method.ReturnType).TypeArguments.Length > 0;

    internal static string EscapeToHtml(string text) => text.Replace("<", "&lt;").Replace(">", "&gt;");

    internal static (string containingSymbol, string accessibilityString, string overrideString) Overwrites(
        this ISymbol method)
    {
        if (method.ContainingType.TypeKind == TypeKind.Interface)
        {
            return (method.ContainingSymbol + ".", "", "");
        }

        return ("", method.AccessibilityString(), "override ");
    }

    internal static (string parameterList, string typeList, string nameList) ParameterStrings(this IMethodSymbol method)
    {
        var parameterList = method.ToString(p => $"{p.OutString()}{p.Type} {p.Name}");
        var typeList = method.ToString(p => $"{p.Type}");
        var nameList = method.ToString(p => $"{p.OutString()}{p.Name}");
        return (parameterList, typeList, nameList);
    }
}

public static class ConstraintHelpers
{
    public static string ToConstraints(this ImmutableArray<ITypeSymbol> typeArguments)
    {
        var result = new StringBuilder();

        foreach (var s in typeArguments.Select(type => ToConstraintString((ITypeParameterSymbol)type)))
        {
            result.Append(s);
        }

        return result.ToString().Trim();
    }

    private static string ToConstraintString(ITypeParameterSymbol symbol)
    {
        var result = symbol.ConstraintTypes.Select(t => t.ToString()).ToList();

        if (symbol.HasUnmanagedTypeConstraint)
        {
            result.Add("unmanaged");
        }
        else
        {
            if (symbol.HasConstructorConstraint)
            {
                result.Add("new()");
            }

            if (symbol.HasValueTypeConstraint)
            {
                result.Add("struct");
            }
        }

        if (symbol.HasReferenceTypeConstraint)
        {
            result.Add("class");
        }

        if (symbol.HasNotNullConstraint)
        {
            result.Add("notnull");
        }

        if (result.Count == 0)
        {
            return "";
        }

        return " where " + symbol.Name + " : " + string.Join(", ", result);
    }
}
