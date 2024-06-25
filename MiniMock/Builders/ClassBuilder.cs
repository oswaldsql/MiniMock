namespace MiniMock.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal class ClassBuilder(ISymbol target, SourceProductionContext context)
{
    private string fullName = target.ToString();
    private INamespaceSymbol interfaceNamespace = target.ContainingNamespace;
    private string name = target.Name + "Mock";

    public static string Build(ISymbol symbol, SourceProductionContext context) =>
        new ClassBuilder(symbol, context).BuildClass();

    internal string BuildClass()
    {
        var builder = new CodeBuilder();

        var typeArguments = ((INamedTypeSymbol)target).TypeArguments;
        if (typeArguments.Length > 0)
        {
            var types = string.Join("_", typeArguments.Select(t => t.Name));
            this.name = $"{target.Name}Mock_{types}";
        }

        builder.Add($$"""
                      // Generated by MiniMock on {{DateTime.Now}}
                      #nullable enable
                      namespace {{this.interfaceNamespace}}
                      {
                      ->
                      /// <summary>
                      /// Mock implementation of <see cref="{{this.fullName.Replace("<", "{").Replace(">", "}")}}"/>. Should only be used for testing purposes.
                      /// </summary>
                      internal class {{this.name}} : {{this.fullName}}
                      {
                      ->
                      private {{this.name}}(System.Action<Config>? config = null) {
                          var result = new Config(this);
                          config = config ?? new System.Action<Config>(t => { });
                          config.Invoke(result);
                          _MockConfig = result;
                      }
                      
                      public static {{this.fullName}} Create(System.Action<Config>? config = null) => new {{this.name}}(config);
                      
                      internal Config _MockConfig { get; set; }

                      public partial class Config
                      {
                          private readonly {{this.name}} target;
                      
                          public Config({{this.name}} target)
                          {
                              this.target = target;
                          }
                      }
                      """);

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

        builder.Add("""
                    <-
                    }
                    <-
                    }
                    """);

        return builder.ToString();
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
