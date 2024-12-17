namespace MiniMock.Util;

using System.Linq;
using Microsoft.CodeAnalysis;

public static class Helpers
{
    private static string AccessibilityString(this ISymbol method) =>
        method.DeclaredAccessibility.AccessibilityString();

    private static string AccessibilityString(this Accessibility accessibility) =>
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

    private static string OutAsString(this IParameterSymbol parameterSymbol) =>
        parameterSymbol.RefKind switch
        {
            RefKind.Out => "out ",
            RefKind.Ref => "ref ",
            RefKind.In => "in ",
            RefKind.RefReadOnlyParameter => "ref readonly ",
            _ => ""
        };

    internal static bool HasParameters(this IMethodSymbol method) => method.Parameters.Length > 0;

    internal static string BuildNotMockedException(this IMethodSymbol symbol)
        => $"throw new System.InvalidOperationException(\"The method '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";

    internal static string BuildNotMockedException(this IPropertySymbol symbol)
        => $"throw new System.InvalidOperationException(\"The property '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";

    public static string BuildNotMockedExceptionForIndexer(this IPropertySymbol symbol) =>
        $"throw new System.InvalidOperationException(\"The indexer '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";

    internal static bool IsReturningTask(this IMethodSymbol method) =>
        method.ReturnType.ToString().Equals("System.Threading.Tasks.Task");

    internal static bool IsReturningGenericTask(this IMethodSymbol method) =>
        method.ReturnType.ToString().StartsWith("System.Threading.Tasks.Task<") &&
        ((INamedTypeSymbol)method.ReturnType).TypeArguments.Length > 0;

    internal static string EscapeToHtml(this string text) => text.Replace("<", "&lt;").Replace(">", "&gt;");

    internal static (string containingSymbol, string accessibilityString, string overrideString) Overwrites(
        this ISymbol method)
    {
        if (method.ContainingType.TypeKind == TypeKind.Interface)
        {
            return (method.ContainingSymbol + ".", "", "");
        }

        return ("", method.AccessibilityString(), "override ");
    }

    internal static (string methodParameters, string parameterList, string typeList, string nameList) ParameterStrings(this IMethodSymbol method)
    {
        var parameters = method.Parameters.Select(t => new ParameterInfo(t.Type.ToString(), t.Name, t.OutAsString(), t.Name)).ToList();

        var methodParameters = parameters.ToString(p => $"{p.OutString}{p.Type} {p.Name}");

        if (method.IsGenericMethod)
        {
            parameters.AddRange(method.TypeArguments.Select(typeArgument => new ParameterInfo("System.Type", "typeOf_" + typeArgument.Name, "", "typeof(" + typeArgument.Name + ")")));
        }

        var parameterList = parameters.ToString(p => $"{p.OutString}{p.Type} {p.Name}");
        var typeList = parameters.ToString(p => $"{p.Type}");
        var nameList = parameters.ToString(p => $"{p.OutString}{p.Function}");

        return (methodParameters, parameterList, typeList, nameList);
    }
}
