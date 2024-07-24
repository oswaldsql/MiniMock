namespace MiniMock.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal class ClassBuilder(ISymbol target, SourceProductionContext context)
{
    public static string Build(ISymbol symbol, SourceProductionContext context) =>
        new ClassBuilder(symbol, context).BuildClass();

    private string BuildClass()
    {
        var builder = new CodeBuilder();

        var fullName = target.ToString();
        var interfaceNamespace = target.ContainingNamespace;
        var name = target.Name + "Mock";

        var typeArguments = ((INamedTypeSymbol)target).TypeArguments;
        if (typeArguments.Length > 0)
        {
            var types = string.Join("_", typeArguments.Select(t => t.Name));
            name = $"{target.Name}Mock_{types}";
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
                      internal class {{name}} : {{fullName}}
                      {
                      ->
                      private {{name}}(System.Action<Config>? config = null) {
                          var result = new Config(this);
                          config = config ?? new System.Action<Config>(t => { });
                          config.Invoke(result);
                          _MockConfig = result;
                      }

                      public static {{fullName}} Create(System.Action<Config>? config = null) => new {{name}}(config);

                      internal Config _MockConfig { get; set; }

                      public partial class Config
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

        this.AddInheritedInterfaces(memberCandidates, (INamedTypeSymbol)target);

        var memberGroups = memberCandidates.ToLookup(t => t.Name);

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
