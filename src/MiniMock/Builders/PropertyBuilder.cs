namespace MiniMock.Builders;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal static class PropertyBuilder
{
    public static void BuildProperties(CodeBuilder builder, IEnumerable<IPropertySymbol> propertySymbols)
    {
        var enumerable = propertySymbols as IPropertySymbol[] ?? propertySymbols.ToArray();
        var name = enumerable.First().Name;
        var helpers = new List<MethodSignature>();

        builder.Add($"#region Property : {name}");

        var index = 0;
        foreach (var symbol in enumerable)
        {
            if (symbol.IsStatic)
            {
                builder.Add($"// Ignoring Static property {symbol}.");
            }else if (!symbol.IsAbstract && !symbol.IsVirtual)
            {
                builder.Add($"// Ignoring property {symbol}.");
            }else
            {
                index++;
                BuildProperty(builder, symbol, helpers, index);
            }
        }

        helpers.BuildHelpers(builder, name);

        builder.Add("#endregion");
    }

    internal static void BuildProperty(CodeBuilder builder, IPropertySymbol symbol, List<MethodSignature> helpers,
        int index)
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

        var seeCref = symbol.ToString();
        if (symbol.NullableAnnotation == NullableAnnotation.Annotated)
        {
            helpers.Add(new MethodSignature(
                $"{type.Replace("?", "")} value"
                , $"""
                   target._{internalName} = value;
                   target._{internalName}_get = () => target._{internalName};
                   target._{internalName}_set = s => target._{internalName} = s;
                   """,
                $"Sets an initial value for {propertyName}.", seeCref));
        }
        else
        {
            helpers.Add(new MethodSignature(
                $"{type.Replace("?", "")} value"
                , $"""
                   target._{internalName} = value;
                   target._{internalName}_get = () => target._{internalName} ?? {symbol.BuildNotMockedException()};
                   target._{internalName}_set = s => target._{internalName} = s;
                   """,
                $"Sets an initial value for {propertyName}.", seeCref));
        }

        if (hasSet || !hasGet)
        {
            helpers.Add(new MethodSignature(
                $"System.Action<{type}> set",
                $"target._{internalName}_set = set;",
                $"Specifies a setter method to call when the property {propertyName} is set.", seeCref));
        }

        if (!hasSet || hasGet)
        {
            helpers.Add(new MethodSignature(
                $"System.Func<{type}> get",
                $"target._{internalName}_get = get;",
                $"Specifies a getter method to call when the property {propertyName} is called.", seeCref));
        }

        if (hasGet && hasSet)
        {
            helpers.Add(new MethodSignature(
                $"System.Func<{type}> get, System.Action<{type}> set",
                $"""
                 target._{internalName}_get = get;
                 target._{internalName}_set = set;
                 """,
                $"Specifies a getter and setter method to call when the property {propertyName} is called.", seeCref));
        }
    }
}
