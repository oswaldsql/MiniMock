namespace MiniMock.Builders;

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

internal static class PropertyBuilder
{
    internal static void BuildProperty(CodeBuilder builder, IPropertySymbol property)
    {
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

        builder.Add($$$"""

                       #region Property : {{{propertyName}}}
                       public {{{overrideString}}}{{{type}}} {{{propertyName}}}
                       {
                           get {
                               return this.Get_{{{propertyName}}}();
                           }
                           {{{setType}}} { 
                               this.Set_{{{propertyName}}}(value);
                           }
                       }

                       internal {{{type.TrimEnd('?')}}}? internal_{{{propertyName}}};
                       internal System.Func<{{{type}}}> Get_{{{propertyName}}} { get; set; } = () => {{{BuildNotMockedException(property)}}}
                       internal System.Action<{{{type}}}> Set_{{{propertyName}}} { get; set; } = s => {{{BuildNotMockedException(property)}}}
                       
                       public partial class Config
                       {
                           public Config {{{propertyName}}}({{{type.Replace("?", "")}}} value)
                           {
                               target.internal_{{{propertyName}}} = value;
                               target.Get_{{{propertyName}}} = () => target.internal_{{{propertyName}}};
                               target.Set_{{{propertyName}}} = s => target.internal_{{{propertyName}}} = s;
                               
                               return this;
                           }
                       
                           public Config {{{propertyName}}}(System.Func<{{{type}}}> get, System.Action<{{{type}}}> set)
                           {
                               target.Get_{{{propertyName}}} = get;
                               target.Set_{{{propertyName}}} = set;
                               return this;
                           }
                       }

                       #endregion
                       """);
    }

    private static string BuildNotMockedException(this IPropertySymbol symbol) => $"throw new System.InvalidOperationException(\"The property '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";

    public static void BuildProperties(CodeBuilder builder, IEnumerable<IPropertySymbol> propertySymbols)
    {
        foreach (var symbol in propertySymbols)
        {
            BuildProperty(builder, symbol);
        }
    }
}
