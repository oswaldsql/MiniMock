namespace MiniMock.Builders;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal static class IndexBuilder
{
    internal static void BuildIndex(CodeBuilder builder, IPropertySymbol indx)
    {
        var returnType = indx.Type.ToString();
        var indexType = indx.Parameters[0].Type.ToString();
        var methodDifferenter = indexType.Replace(".", "_");
        if (indx.Parameters[0].Type.IsTupleType)
        {
            methodDifferenter =
                string.Join("", ((INamedTypeSymbol)indx.Parameters[0].Type).TypeArguments.Select(t => t.Name));
        }

        var exception = BuildNotMockedException(indx);

        builder.Add($$"""
                      #region Indexer : {{returnType}} this[{{indexType}}]
                          public {{returnType}} this[{{indexType}} index]
                          {
                              get => this.On_{{methodDifferenter}}IndexGet(index);
                              set => this.On_{{methodDifferenter}}IndexSet(index, value);
                          }
                      
                          internal System.Func<{{indexType}}, {{returnType}}> On_{{methodDifferenter}}IndexGet { get; set; } = (_) => {{exception}}
                          internal System.Action<{{indexType}}, {{returnType}}> On_{{methodDifferenter}}IndexSet { get; set; } = (_, _) => {{exception}}
                          
                          public partial class Config{
                              public Config Indexer(System.Collections.Generic.Dictionary<{{indexType}}, {{returnType}}> values){
                                  target.On_{{methodDifferenter}}IndexGet = s => values[s];
                                  target.On_{{methodDifferenter}}IndexSet = (s, v) => values[s] = v;
                                  return this;
                              }
                      
                              public Config Indexer(System.Func<{{indexType}}, {{returnType}}> get, System.Action<{{indexType}}, {{returnType}}> set){
                                  target.On_{{methodDifferenter}}IndexGet = get;
                                  target.On_{{methodDifferenter}}IndexSet = set;
                                  return this;
                              }
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
