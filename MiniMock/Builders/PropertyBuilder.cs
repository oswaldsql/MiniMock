namespace MiniMock.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal static class PropertyBuilder
{
    private static int propertyCount;

    public static void BuildProperties(CodeBuilder builder, IEnumerable<IPropertySymbol> propertySymbols)
    {
        var enumerable = propertySymbols as IPropertySymbol[] ?? propertySymbols.ToArray();
        var name = enumerable.First().Name;
        var helpers = new List<MethodSignature>();

        void AddHelper(string signature, string code, string documentation)
        {
            helpers.Add(new(signature, code, documentation));
        }

        foreach (var symbol in enumerable)
        {
            BuildProperty(builder, symbol, AddHelper);
        }

        helpers.BuildHelpers(builder, name);
    }

    internal static void BuildProperty(CodeBuilder builder, IPropertySymbol property, Action<string, string, string> addHelper)
    {
        propertyCount++;

        var propertyName = property.Name;
        var type = property.Type.ToString();
        var setType = property.SetMethod?.IsInitOnly == true ? "init" : "set";

        var overrideString = "";
        if (property.ContainingType.TypeKind == TypeKind.Class)
        {
            if (property.IsAbstract)
            {
                overrideString = "override ";
            }
            else if (property.IsVirtual)
            {
                overrideString = "override ";
            }
        }

        var containingSymbol = "";
        var accessibilityString = property.AccessibilityString();
        if (property.ContainingType.TypeKind == TypeKind.Interface)
        {
            containingSymbol = property.ContainingSymbol + ".";
            accessibilityString = "";
        }

        builder.Add($$"""

                      #region Property : {{propertyName}}
                      {{accessibilityString}} {{overrideString}}{{type}} {{containingSymbol}}{{propertyName}}
                      {
                      """);

        if (property.GetMethod != null)
        {
            builder.Add($$"""
                              get {
                                  return this.Get_{{propertyName}}_{{propertyCount}}();
                              }
                          """);
        }

        if (property.SetMethod != null)
        {
            builder.Add($$"""
                              {{setType}} { 
                                  this.Set_{{propertyName}}_{{propertyCount}}(value);
                              }
                          """);
        }

        builder.Add("""}""");


        builder.Add($$"""
                      internal {{type.TrimEnd('?')}}? internal_{{propertyName}}_{{propertyCount}};
                      internal System.Func<{{type}}> Get_{{propertyName}}_{{propertyCount}} { get; set; } = () => {{BuildNotMockedException(property)}}
                      internal System.Action<{{type}}> Set_{{propertyName}}_{{propertyCount}} { get; set; } = s => {{BuildNotMockedException(property)}}

                      #endregion
                      """);

        addHelper($"{type.Replace("?", "")} value", $"target.internal_{propertyName}_{propertyCount} = value;", $"Sets a initial value for {propertyName}.");
        addHelper($"{type.Replace("?", "")} value", $"target.Get_{propertyName}_{propertyCount} = () => target.internal_{propertyName}_{propertyCount};","");
        addHelper($"{type.Replace("?", "")} value", $"target.Set_{propertyName}_{propertyCount} = s => target.internal_{propertyName}_{propertyCount} = s;","");
        addHelper($"System.Func<{type}> get, System.Action<{type}> set", $"target.Get_{propertyName}_{propertyCount} = get;", $"Specifies a getter and setter method to call when the property {propertyName} is called.");
        addHelper($"System.Func<{type}> get, System.Action<{type}> set", $"target.Set_{propertyName}_{propertyCount} = set;","");
    }

    private static string BuildNotMockedException(this IPropertySymbol symbol) =>
        $"throw new System.InvalidOperationException(\"The property '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";
}
