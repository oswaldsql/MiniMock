namespace MiniMock.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Util;

internal class ClassBuilder(ISymbol target)
{
    private static readonly ISymbolBuilder[] Builders = [new ConstructorBuilder2(),  new EventBuilder(), new MethodBuilder(), new PropertyBuilder(), new IndexBuilder()];

    private readonly Func<Accessibility, bool> accessibilityFilter = accessibility => accessibility == Accessibility.Public || accessibility == Accessibility.Protected;

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

        new ConstructorBuilder(target).Build(builder, fullName, name);

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
        var memberCandidates = new List<ISymbol>(((INamedTypeSymbol)target).GetMembers().Where(t => this.accessibilityFilter(t.DeclaredAccessibility)));

        if (((INamedTypeSymbol)target).TypeKind == TypeKind.Interface)
        {
            this.AddInheritedInterfaces(memberCandidates, (INamedTypeSymbol)target);
        }

        var memberGroups = memberCandidates.Distinct(SymbolEqualityComparer.IncludeNullability).ToLookup(t => t.Name);

        foreach (var members in memberGroups)
        {
            var symbol = members.First();
            var wasBuild = Builders.FirstOrDefault(b => b.TryBuild(builder, members));
            if (wasBuild == null)
            {
                builder.Add("// Ignored " + symbol.Kind + " " + symbol);
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
