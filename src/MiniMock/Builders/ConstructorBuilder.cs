namespace MiniMock.Builders;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Util;

internal class ConstructorBuilder : ISymbolBuilder
{
    public bool TryBuild(CodeBuilder builder, IGrouping<string, ISymbol> symbols)
    {
        var first = symbols.First();
        if (first is IMethodSymbol { MethodKind: MethodKind.Constructor })
        {
            return BuildConstructors(builder, first.ContainingSymbol, symbols.OfType<IMethodSymbol>());
        }

        return false;
    }

    private static bool BuildConstructors(CodeBuilder builder, ISymbol target, IEnumerable<IMethodSymbol> constructors)
    {
        var fullName = target.ToString();
        var name = "MockOf_" + target.Name;

        var typeArguments = ((INamedTypeSymbol)target).TypeArguments;
        if (typeArguments.Length > 0)
        {
            var types = string.Join(", ", typeArguments.Select(t => t.Name));
            name = $"MockOf_{target.Name}<{types}>";
        }

        builder.Add("#region Constructors");

        foreach (var constructor in constructors)
        {
            var parameterList = constructor.Parameters.ToString(p => $"{p.Type} {p.Name}, " , "");
            var argumentList = constructor.Parameters.ToString(p => p.Name);

            var parameterNames = constructor.Parameters.ToString(p => p.Name + ", ", "");

            builder.Add($$"""
                          internal protected MockOf_{{target.Name}}({{parameterList}}System.Action<Config>? config = null) : base({{argumentList}}) {
                              var result = new Config(this);
                              config = config ?? new System.Action<Config>(t => { });
                              config.Invoke(result);
                              _config = result;
                          }

                          public static {{fullName}} Create({{parameterList}}System.Action<Config>? config = null) => new {{name}}({{parameterNames}}config);
                          """);
        }

        builder.Add("#endregion");

        return true;
    }

    public static CodeBuilder BuildEmptyConstructor(ISymbol target)
    {
        var fullName = target.ToString();
        var name = "MockOf_" + target.Name;

        var typeArguments = ((INamedTypeSymbol)target).TypeArguments;
        if (typeArguments.Length > 0)
        {
            var types = string.Join(", ", typeArguments.Select(t => t.Name));
            name = $"MockOf_{target.Name}<{types}>";
        }

        CodeBuilder builder = new();

        builder.Add($$"""
                      #region Constructor

                      internal protected MockOf_{{target.Name}}(System.Action<Config>? config = null) {
                          var result = new Config(this);
                          config = config ?? new System.Action<Config>(t => { });
                          config.Invoke(result);
                          _config = result;
                      }

                      public static {{fullName}} Create(System.Action<Config>? config = null) => new {{name}}(config);

                      #endregion
                      """);

        return builder;
    }
}
