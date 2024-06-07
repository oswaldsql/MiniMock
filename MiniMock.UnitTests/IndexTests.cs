namespace MiniMock.UnitTests;

using Microsoft.CodeAnalysis;

public interface IIndexRepository
{
    int this[uint index] { set; }
    int this[int index] { get; }
    int this[string index] { get; set; }
    (string name, int age) this[Guid index] { get; set; }
    string this[(string name, int age) index] { get; set; }
}

public class IndexTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void IIndexRepositoryTests()
    {
        var source = @"namespace Demo;
using MiniMock.UnitTests;
using MiniMock;
using System;

public class TestClass{
    [Mock<IIndexRepository>]
    public void Test(){}
}";

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.Empty(result.diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error));
    }
}
