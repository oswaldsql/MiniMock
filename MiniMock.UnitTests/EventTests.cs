namespace MiniMock.UnitTests;

using Microsoft.CodeAnalysis;

public class EventTests(ITestOutputHelper testOutputHelper)
{
    public interface IEventRepository
    {
        delegate void SampleEventHandler(object sender, string pe);

        event EventHandler SimpleEvent;
        event EventHandler<string> EventWithArgs;
        event SampleEventHandler CustomEvent;
    }

    [Fact]
    public void EventRepositoryTests()
    {
        var source = Build.TestClass<IEventRepository>();

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.Empty(result.diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error));
    }
}
