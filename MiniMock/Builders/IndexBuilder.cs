namespace MiniMock.Builders;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal static class IndexBuilder
{
    internal static void BuildIndex(CodeBuilder builder, IPropertySymbol indx)
    {
        var hasGetter = indx.GetMethod != null;
        var hasSetter = indx.SetMethod != null;

        var returnType = indx.Type.ToString();
        var indexType = indx.Parameters[0].Type.ToString();
        var methodDifferenter = indexType.Replace(".", "_");
        if (indx.Parameters[0].Type.IsTupleType)
        {
            methodDifferenter =
                string.Join("", ((INamedTypeSymbol)indx.Parameters[0].Type).TypeArguments.Select(t => t.Name));
        }

        builder.Add($$"""

                      #region {{returnType}} this[{{indexType}}]
                      """);

            //builder.Add($$"""
            //              public partial class Config{
            //                  public Config On_{{methodDifferenter}}Index_Get(System.Func<{{indexType}}, {{returnType}}> mock){
            //                      this.On_{{methodDifferenter}}IndexGet = mock;
            //                      return this;
            //                  }
            //                  internal System.Func<{{indexType}}, {{returnType}}> On_{{methodDifferenter}}IndexGet { get; set; } = (_) => {{BuildNotMockedException(indx)}}
            //              }

            //              """);
            //builder.Add($$"""
            //              public partial class Config{
            //                  public Config On_{{methodDifferenter}}Index_Set(System.Action<{{indexType}}, {{returnType}}> mock){
            //                      this.On_{{methodDifferenter}}IndexSet = mock;
            //                      return this;
            //                  }
            //                  internal System.Action<{{indexType}}, {{returnType}}> On_{{methodDifferenter}}IndexSet { get; set; } = (_, _) => {{BuildNotMockedException(indx)}}
            //              }

            //              """);

            builder.Add($$"""
                      public partial class Config{
                          internal System.Func<{{indexType}}, {{returnType}}> On_{{methodDifferenter}}IndexGet { get; set; } = (_) => {{BuildNotMockedException(indx)}}
                          internal System.Action<{{indexType}}, {{returnType}}> On_{{methodDifferenter}}IndexSet { get; set; } = (_, _) => {{BuildNotMockedException(indx)}}
                      }

                      """);

        builder.Add($$"""
                      public partial class Config{
                          public Config Indexer(System.Collections.Generic.Dictionary<{{indexType}}, {{returnType}}> values){
                              this.On_{{methodDifferenter}}IndexGet = s => values[s];
                              this.On_{{methodDifferenter}}IndexSet = (s, v) => values[s] = v;
                              return this;
                          }
                      }

                      """);

        builder.Add($$"""
                      public partial class Config{
                          public Config Indexer(System.Func<{{indexType}}, {{returnType}}> get, System.Action<{{indexType}}, {{returnType}}> set){
                              this.On_{{methodDifferenter}}IndexGet = get;
                              this.On_{{methodDifferenter}}IndexSet = set;
                              return this;
                          }
                      }

                      """);

        builder.Add($$"""
                      public {{returnType}} this[{{indexType}} index]
                      {
                      ->
                      """)
            .Add(hasGetter, () => $"get => _MockConfig.On_{methodDifferenter}IndexGet(index);")
            .Add(hasSetter, () => $"set => _MockConfig.On_{methodDifferenter}IndexSet(index, value);")
            .Add("""
                 <-
                 }
                 #endregion
                 """);
    }

    private static string BuildNotMockedException(this IPropertySymbol symbol) => $"throw new System.InvalidOperationException(\"The indexer '{symbol.Name}' in '{symbol.ContainingType.Name}' is not explicitly mocked.\") {{Source = \"{symbol}\"}};";

    public static void BuildIndexes(CodeBuilder builder, IEnumerable<IPropertySymbol> indexerSymbols)
    {
        foreach (var symbol in indexerSymbols)
        {
            BuildIndex(builder, symbol);
        }
    }
}
