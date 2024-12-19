namespace MiniMock.Util;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

public static class MergeHelpers
{
    internal static string ToString(this IEnumerable<ParameterInfo> m, Func<ParameterInfo, string> selector,
        string separator = ", ")
    {
        var Parameters = m.Select(selector);
        var parameterList = string.Join(separator, Parameters);
        return parameterList;
    }

    internal static string ToString(this IEnumerable<IParameterSymbol>? m, Func<IParameterSymbol, string> selector,
        string separator = ", ")
    {
        if (m == null)
        {
            return "";
        }

        var Parameters = m.Select(selector);
        var parameterList = string.Join(separator, Parameters);
        return parameterList;
    }

    internal static string ToString(this IEnumerable<ITypeSymbol> m, Func<ITypeSymbol, string> selector,
        string separator = ", ")
    {
        var Parameters = m.Select(selector);
        var parameterList = string.Join(separator, Parameters);
        return parameterList;
    }
}
