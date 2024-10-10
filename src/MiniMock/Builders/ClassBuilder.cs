namespace MiniMock.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal class ClassBuilder(ISymbol target)
{
    public static string Build(ISymbol symbol) =>
        new ClassBuilder(symbol).BuildClass();

    private string BuildClass()
    {
        if (target.IsSealed && target is INamedTypeSymbol symbol)
        {
            throw new CanNotMockASealedClassException(symbol);
        }

        var builder = new CodeBuilder();

        var fullName = target.ToString();
        var interfaceNamespace = target.ContainingNamespace;
        var name = "MockOf_" + target.Name;

        var constraints = "";

        var typeArguments = ((INamedTypeSymbol)target).TypeArguments;
        if (typeArguments.Length > 0)
        {
            constraints = typeArguments.ToConstraints();
            var types = string.Join(", ", typeArguments.Select(t => t.Name));
            name = $"MockOf_{target.Name}<{types}>";
        }

        var documentationName = fullName.Replace("<", "{").Replace(">", "}");

        builder.Add($$"""
                      // Generated by MiniMock on {{DateTime.Now}}
                      #nullable enable
                      namespace {{interfaceNamespace}}
                      {
                      ->
                      /// <summary>
                      /// Mock implementation of <see cref="{{documentationName}}"/>. Should only be used for testing purposes.
                      /// </summary>
                      internal class {{name}} : {{fullName}} {{constraints}}
                      {
                      ->
                      internal protected MockOf_{{target.Name}}(System.Action<Config>? config = null) {
                          var result = new Config(this);
                          config = config ?? new System.Action<Config>(t => { });
                          config.Invoke(result);
                          _config = result;
                      }

                      public static {{fullName}} Create(System.Action<Config>? config = null) => new {{name}}(config);

                      private Config _config { get; }
                      internal void GetConfig(out Config config) => config = _config;

                      internal partial class Config
                      {
                          private readonly {{name}} target;

                          public Config({{name}} target)
                          {
                              this.target = target;
                          }
                      }
                      """);

        this.BuildMembers(builder);

        builder.Add("""
                    <-
                    }
                    <-
                    }
                    """);

        return builder.ToString();
    }

    private void BuildMembers(CodeBuilder builder)
    {
        var memberCandidates = new List<ISymbol>(((INamedTypeSymbol)target).GetMembers());

        if (((INamedTypeSymbol)target).TypeKind == TypeKind.Interface)
        {
            this.AddInheritedInterfaces(memberCandidates, (INamedTypeSymbol)target);
        }

        var memberGroups = memberCandidates.Distinct(SymbolEqualityComparer.IncludeNullability).ToLookup(t => t.Name);

        foreach (var members in memberGroups)
        {
            var symbol = members.First();
            switch (symbol)
            {
                case IEventSymbol:
                    EventBuilder.BuildEvents(builder, members.OfType<IEventSymbol>());
                    break;
                case IMethodSymbol { MethodKind: MethodKind.Ordinary }:
                    MethodBuilder.BuildMethods(builder, members.OfType<IMethodSymbol>());
                    break;
                case IPropertySymbol { IsIndexer: false }:
                    PropertyBuilder.BuildProperties(builder, members.OfType<IPropertySymbol>());
                    break;
                case IPropertySymbol { IsIndexer: true }:
                    IndexBuilder.BuildIndexes(builder, members.OfType<IPropertySymbol>());
                    break;
            }
        }
    }

    private void AddInheritedInterfaces(List<ISymbol> memberCandidates, INamedTypeSymbol namedTypeSymbol)
    {
        var allInterfaces = namedTypeSymbol.AllInterfaces;
        foreach (var interface2 in allInterfaces)
        {
            memberCandidates.AddRange(interface2.GetMembers());
            this.AddInheritedInterfaces(memberCandidates, interface2);
        }
    }
}
