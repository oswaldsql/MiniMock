namespace MiniMock.Builders;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Util;

/// <summary>
/// Represents a builder for properties, implementing the ISymbolBuilder interface.
/// </summary>
internal class PropertyBuilder : ISymbolBuilder
{
    /// <inheritdoc />
    public bool TryBuild(CodeBuilder builder, IGrouping<string, ISymbol> symbols)
    {
        var first = symbols.First();

        if (first is IMethodSymbol { MethodKind: MethodKind.PropertyGet or MethodKind.PropertySet })
        {
            return true;
        }

        if (first is not IPropertySymbol symbol || symbol.IsIndexer)
        {
            return false;
        }

        if (!(first.IsAbstract || first.IsVirtual))
        {
            return true;
        }

        var propertySymbols = symbols.OfType<IPropertySymbol>().Where(t => !t.IsIndexer).ToArray();
        if (propertySymbols.Length == 0)
        {
            return false;
        }

        return BuildProperties(builder, propertySymbols);
    }

    private static bool BuildProperties(CodeBuilder builder, IPropertySymbol[] symbols)
    {
        var name = symbols.First().Name;
        var helpers = new List<HelperMethod>();

        builder.Add($"#region Property : {name}");

        var index = 0;
        foreach (var symbol in symbols)
        {
            if (symbol.IsStatic)
            {
                builder.Add($"// Ignoring Static property {symbol}.");
            }
            else if (!symbol.IsAbstract && !symbol.IsVirtual)
            {
                builder.Add($"// Ignoring property {symbol}.");
            }
            else
            {
                index++;
                BuildProperty(builder, symbol, helpers, index);
            }
        }

        builder.Add(helpers.BuildHelpers(name));

        builder.Add("#endregion");

        return index > 0;
    }

    private static void BuildProperty(CodeBuilder builder, IPropertySymbol symbol, List<HelperMethod> helpers, int index)
    {
        if (symbol.ReturnsByRef || symbol.ReturnsByRefReadonly)
        {
            throw new RefPropertyNotSupportedException(symbol, symbol.ContainingType);
        }

        var propertyName = symbol.Name;
        var internalName = index == 1 ? propertyName : $"{propertyName}_{index}";
        var type = symbol.Type.ToString();
        var setType = symbol.SetMethod?.IsInitOnly == true ? "init" : "set";

        var (containingSymbol, accessibilityString, overrideString) = symbol.Overwrites();

        var hasGet = symbol.GetMethod != null;
        var hasSet = symbol.SetMethod != null;

        builder.Add($$"""

                      {{accessibilityString}} {{overrideString}}{{type}} {{containingSymbol}}{{propertyName}}
                      {
                      """).Indent();
        builder.Add(hasGet, () => $"get => this._{internalName}_get();");
        builder.Add(hasSet, () => $"{setType} => this._{internalName}_set(value);");
        builder.Unindent().Add("}");

        builder.Add($$"""
                      private {{type.TrimEnd('?')}}? _{{internalName}};
                      private System.Func<{{type}}> _{{internalName}}_get { get; set; } = () => {{symbol.BuildNotMockedException()}}
                      private System.Action<{{type}}> _{{internalName}}_set { get; set; } = s => {{symbol.BuildNotMockedException()}}

                      """);

        helpers.AddRange(BuildHelpers(symbol, type, internalName, propertyName));
    }

    /// <summary>
    /// Builds helper methods for the property.
    /// </summary>
    /// <param name="symbol">The property symbol representing the property.</param>
    /// <param name="type">The type of the property.</param>
    /// <param name="internalName">The internal name of the property.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>A collection of helper methods for the property.</returns>
    private static IEnumerable<HelperMethod> BuildHelpers(IPropertySymbol symbol, string type, string internalName, string propertyName)
    {
        var seeCref = symbol.ToString();

        var hasGet = symbol.GetMethod != null;
        var hasSet = symbol.SetMethod != null;

        var isNullable = symbol.NullableAnnotation == NullableAnnotation.Annotated;
        if (isNullable)
        {
            yield return new HelperMethod(
                $"{type.Replace("?", "")} value"
                , $"""
                   target._{internalName} = value;
                   target._{internalName}_get = () => target._{internalName};
                   target._{internalName}_set = s => target._{internalName} = s;
                   """,
                $"Sets an initial value for {propertyName}.", seeCref);
        }
        else
        {
            yield return new HelperMethod(
                $"{type.Replace("?", "")} value"
                , $"""
                   target._{internalName} = value;
                   target._{internalName}_get = () => target._{internalName} ?? {symbol.BuildNotMockedException()};
                   target._{internalName}_set = s => target._{internalName} = s;
                   """,
                $"Sets an initial value for {propertyName}.", seeCref);
        }

        switch (hasSet, hasGet)
        {
            case (true, true):
                yield return new HelperMethod(
                    $"System.Func<{type}> get, System.Action<{type}> set",
                    $"""
                     target._{internalName}_get = get;
                     target._{internalName}_set = set;
                     """,
                    $"Specifies a getter and setter method to call when the property {propertyName} is called.", seeCref);
                break;
            case (false, true):
                yield return new HelperMethod(
                    $"System.Func<{type}> get",
                    $"target._{internalName}_get = get;",
                    $"Specifies a getter method to call when the property {propertyName} is called.", seeCref);
                break;
            case (true, false):
                yield return new HelperMethod(
                    $"System.Action<{type}> set",
                    $"target._{internalName}_set = set;",
                    $"Specifies a setter method to call when the property {propertyName} is set.", seeCref);
                break;
        }
    }
}
