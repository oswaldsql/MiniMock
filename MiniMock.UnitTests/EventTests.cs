#nullable enable
namespace MiniMock.UnitTests;

using Microsoft.CodeAnalysis;

public class EventTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void IEventRepositoryTests()
    {
        var source = @"namespace Demo;
using MiniMock.UnitTests;
using MiniMock;
using System;

[Mock<IEventRepository>]
public class TestClass{
}";

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.Empty(result.diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error));
    }
}

public interface IEventRepository
{
    delegate void SampleEventHandler(object sender, string pe);

    event EventHandler SimpleEvent;
    event EventHandler<string> EventWithArgs;
    event SampleEventHandler CustomEvent;
}


