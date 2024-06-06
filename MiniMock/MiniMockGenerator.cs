namespace MiniMock;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MiniMock.Builders;

[Generator]
public sealed class MiniMockGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx => ctx.AddSource(
            "MiniMockAttribute.g.cs", Constants.MockAttributeCode));

        var enums = context.SyntaxProvider.ForAttributeWithMetadataName("MiniMock.Mock`1",
                (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax, this.GetAttributes)
            .Where(static enumData => enumData is not null)
            .SelectMany((enumerable, token) => enumerable)
            .Collect();

        context.RegisterSourceOutput(enums, this.Build);
    }

    private void Build(SourceProductionContext arg1, ImmutableArray<AttributeData> arg2)
    {
        var typeSymbols = arg2
            .Select(FirstGenericType)
            .Where(t => t != null)
            .Distinct(SymbolEqualityComparer.Default)
            .ToArray();

        foreach (var d in typeSymbols)
        {
            var source = ClassBuilder.Build(d, arg1);

            var fileName = d.ToString().Replace("<", "_").Replace(">", "");

            arg1.AddSource(fileName + ".g.cs", source);
        }

        var mockClassSource = MockClassBuilder.Build(typeSymbols, arg1);
        arg1.AddSource("Mock.g.cs", mockClassSource);
    }

    private static ITypeSymbol? FirstGenericType(AttributeData t) => t.AttributeClass?.TypeArguments.FirstOrDefault();

    private IEnumerable<AttributeData> GetAttributes(GeneratorAttributeSyntaxContext arg1, CancellationToken arg2) =>
        arg1.Attributes;
}

public static class Constants
{
    public const string MockAttributeCode = """
                                            namespace MiniMock { 
                                                [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true)]
                                                public class Mock<T> : System.Attribute{}
                                                
                                                internal class CallEvents //: System.Collections.Generic.IEnumerable<CallEvent>
                                                {
                                                    private readonly System.Collections.Generic.List<CallEvent> store = new();
                                                    private int index = 0;
                                                    public void Add(string source, string method, CallEventType type) => this.store.Add(new CallEvent(this.index++, source, method,     type));
                                                    public System.Collections.Generic.IEnumerator<CallEvent> GetEnumerator() => this.store.GetEnumerator();
                                                
                                                    //System.Collections.Generic.IEnumerator<CallEvent> IEnumerable.GetEnumerator() => this.GetEnumerator();
                                                }
                                                
                                                internal class CallEvent
                                                {
                                                    public CallEvent(int Index, string Source, string Method, CallEventType Type)
                                                    {
                                                        this.Index = Index;
                                                        this.Source = Source;
                                                        this.Method = Method;
                                                        this.Type = Type;
                                                    }
                                                
                                                    public int Index { get; }
                                                    public string Source { get; }
                                                    public string Method { get; }
                                                    public CallEventType Type { get; }
                                                
                                                    public void Deconstruct(out int Index, out string Source, out string Method, out CallEventType Type)
                                                    {
                                                        Index = this.Index;
                                                        Source = this.Source;
                                                        Method = this.Method;
                                                        Type = this.Type;
                                                    }
                                                }
                                            
                                                internal enum CallEventType
                                                {
                                                    Setup = 0,
                                                    Call = 1,
                                                    Get = 2,
                                                    Set = 3,
                                                    Raise = 4,
                                                }
                                            }
                                            """;
}


//public class CallEvents2 : System.Collections.Generic.IEnumerable<CallEvent>
//{
//    private readonly System.Collections.Generic.List<CallEvent> store = new();
//    private int index = 0;
//    public void Add(string source, string method, CallEventType type) => this.store.Add(new CallEvent(this.index++, source, method, type));
//    public System.Collections.Generic.IEnumerator<CallEvent> GetEnumerator() => this.store.GetEnumerator();

//    System.Collections.Generic.IEnumerator<CallEvent> System.Collections.Generic.IEnumerable<CallEvent>.GetEnumerator() => this.GetEnumerator();
//}

//public class CallEvents : System.Collections.Generic.IEnumerable<CallEvent>
//{
//    private readonly List<CallEvent> store = new();
//    private int index = 0;
//    public void Add(string source, string method, CallEventType type) => this.store.Add(new CallEvent(this.index++, source, method, type));
//    public System.Collections.Generic.IEnumerator<CallEvent> GetEnumerator() => this.store.GetEnumerator();

//    //System.Collections.Generic.IEnumerator System.Collections.Generic.IEnumerable.GetEnumerator() => this.GetEnumerator();
//    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
//}
//public class CallEvent
//{
//    public CallEvent(int Index, string Source, string Method, CallEventType Type)
//    {
//        this.Index = Index;
//        this.Source = Source;
//        this.Method = Method;
//        this.Type = Type;
//    }

//    public int Index { get; }
//    public string Source { get; }
//    public string Method { get; }
//    public CallEventType Type { get; }

//    public void Deconstruct(out int Index, out string Source, out string Method, out CallEventType Type)
//    {
//        Index = this.Index;
//        Source = this.Source;
//        Method = this.Method;
//        Type = this.Type;
//    }
//}

//public enum CallEventType
//{
//    Setup = 0,
//    Call = 1,
//    Get = 2,
//    Set = 3,
//    Raise = 4,
//}

internal class UnsupportedAccessibilityException(Accessibility accessibility)
    : Exception($"Unsupported accessibility type '{accessibility}'");

