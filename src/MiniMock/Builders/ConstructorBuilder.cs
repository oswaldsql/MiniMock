namespace MiniMock.Builders;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Util;

/// <summary>
/// Represents a builder for constructing mock constructors.
/// </summary>
internal class ConstructorBuilder : ISymbolBuilder
{
    /// <summary>
    /// Tries to build constructors for the given symbols.
    /// </summary>
    /// <param name="builder">The code builder to add the constructors to.</param>
    /// <param name="symbols">The symbols to build constructors for.</param>
    /// <returns>True if constructors were built; otherwise, false.</returns>
    public bool TryBuild(CodeBuilder builder, IGrouping<string, ISymbol> symbols)
    {
        var first = symbols.First();
        if (first is IMethodSymbol { MethodKind: MethodKind.Constructor })
        {
            return BuildConstructors(builder, first.ContainingSymbol, symbols.OfType<IMethodSymbol>());
        }

        return false;
    }

    /// <summary>
    /// Builds constructors for the specified target and adds them to the code builder.
    /// </summary>
    /// <param name="builder">The code builder to add the constructors to.</param>
    /// <param name="target">The target symbol for which to build constructors.</param>
    /// <param name="constructors">The constructors to build.</param>
    /// <returns>True if constructors were built; otherwise, false.</returns>
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
            var parameterList = constructor.Parameters.ToString(p => $"{p.Type} {p.Name}, ", "");
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

    /// <summary>
    /// Builds an empty constructor for the specified target.
    /// </summary>
    /// <param name="target">The target symbol for which to build an empty constructor.</param>
    /// <returns>A code builder containing the empty constructor.</returns>
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
