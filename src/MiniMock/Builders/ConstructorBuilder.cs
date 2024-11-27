namespace MiniMock.Builders;

using System;
using System.Linq;
using Microsoft.CodeAnalysis;

internal class ConstructorBuilder(ISymbol target)
{
    private readonly Func<Accessibility, bool> accessibilityFilter = accessibility => accessibility == Accessibility.Public || accessibility == Accessibility.Protected;

    public void Build(CodeBuilder builder, string fullName, string name)
    {
        var symbol = (INamedTypeSymbol)target;

        var constructors = symbol.Constructors
            .Where(c => this.accessibilityFilter(c.DeclaredAccessibility))
            .ToArray();

        builder.Add("#region Constructors");

        if (constructors.Length == 0 || constructors.Any(t => t.Parameters.Length == 0))
        {
            builder.Add($$"""
                          internal protected MockOf_{{target.Name}}(System.Action<Config>? config = null) {
                              var result = new Config(this);
                              config = config ?? new System.Action<Config>(t => { });
                              config.Invoke(result);
                              _config = result;
                          }

                          public static {{fullName}} Create(System.Action<Config>? config = null) => new {{name}}(config);
                          """);
        }

        foreach (var constructor in constructors.Where(t => t.Parameters.Length > 0))
        {
            var parameters = constructor.Parameters.Select(p => $"{p.Type} {p.Name}").ToArray();
            var parameterList = string.Join(", ", parameters);
            var parameterNames = constructor.Parameters.Select(p => p.Name).ToArray();
            var parameterNamesList = string.Join(", ", parameterNames);

            builder.Add($$"""
                          internal protected MockOf_{{target.Name}}({{parameterList}}, System.Action<Config>? config = null) : base({{parameterNamesList}}) {
                              var result = new Config(this);
                              config = config ?? new System.Action<Config>(t => { });
                              config.Invoke(result);
                              _config = result;
                          }

                          public static {{fullName}} Create({{parameterList}}, System.Action<Config>? config = null) => new {{name}}({{parameterNamesList}}, config);
                          """);
        }

        builder.Add("#endregion");
    }
}
