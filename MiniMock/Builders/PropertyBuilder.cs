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

                       #region {{{propertyName}}}
                       public partial class Config
                       {
                           public Config {{{propertyName}}}({{{type.Replace("?", "")}}}? value = null, System.Func<{{{type}}}>? onGet = null, System.Action<{{{type}}}>? onSet = null)
                           {
                             if (value != null)
                             {
                               this.internal_{{{propertyName}}} = value;
                               this.Get_{{{propertyName}}} = () => this.internal_{{{propertyName}}};
                               this.Set_{{{propertyName}}} = s => this.internal_{{{propertyName}}} = s;
                             }
                             
                             if(onGet != null)
                             {
                                 this.Get_{{{propertyName}}} = onGet;
                             }
                         
                             if(onSet != null)
                             {
                                 this.Set_{{{propertyName}}} = onSet;
                             }
                               
                               return this;
                           }
                       
                           private {{{type.TrimEnd('?')}}}? internal_{{{propertyName}}};
                           internal System.Func<{{{type}}}> Get_{{{propertyName}}} { get; set; } = () => {{{BuildNotMockedException(property)}}}
                           internal System.Action<{{{type}}}> Set_{{{propertyName}}} { get; set; } = s => {{{BuildNotMockedException(property)}}}
                       }

                       public {{{overrideString}}}{{{type}}} {{{propertyName}}}
                       {
                           get {
                             _MockConfig._CallEvents.Add(this.GetType().Name, "{{{propertyName}}}", MiniMock.CallEventType.Get);
                               return _MockConfig.Get_{{{propertyName}}}();
                           }
                           {{{setType}}} { 
                               _MockConfig._CallEvents.Add(this.GetType().Name, "{{{propertyName}}}", MiniMock.CallEventType.Set);
                               _MockConfig.Set_{{{propertyName}}}(value);
                           }
                       }
                       #endregion
                       """);
    }

    private static string BuildNotMockedException(this IPropertySymbol symbol) => $"throw new System.InvalidOperationException(\"The indexer '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";

    public static void BuildProperties(CodeBuilder builder, IEnumerable<IPropertySymbol> propertySymbols)
    {
        foreach (var symbol in propertySymbols)
        {
            BuildProperty(builder, symbol);
        }
    }

    //public string tfdasfads
    //{
    //    get
    //    {

    //        return this.tfdas1;
    //    }
    //    set
    //    {
    //        this.tfdas1 = value;
    //    }
    //}
}
