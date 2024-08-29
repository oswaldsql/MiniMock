namespace MiniMock.Builders;

using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

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
