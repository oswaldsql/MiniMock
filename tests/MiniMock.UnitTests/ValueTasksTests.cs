namespace MiniMock.UnitTests;

public class ValueTasksTests(ITestOutputHelper testOutputHelper)
{
    public interface IValueTasks
    {
        ValueTask SimpleTask();

        ValueTask SimpleTaskWithArgs(string name);

        ValueTask<string> TaskWithResult();

        ValueTask<string> TaskWithResultWithArgs(string name, CancellationToken ct = default);
    }

    [Fact]
    public void DefaultImplementationTests()
    {
        var source = Build.TestClass<IValueTasks>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }
}
