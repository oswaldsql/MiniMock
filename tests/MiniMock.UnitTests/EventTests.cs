namespace MiniMock.UnitTests;

using System.ComponentModel;

public class EventTests(ITestOutputHelper testOutputHelper)
{
    public interface IEventRepository
    {
        delegate void SampleEventHandler(object sender, string pe);

        event EventHandler SimpleEvent;
        event EventHandler<string> EventWithArgs;
        event SampleEventHandler CustomEvent;
    }

    public class TestNotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    [Fact]
    public void EventRepositoryTests()
    {
        var source = Build.TestClass<IEventRepository>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }

    [Fact]
    public void NotifyPropertyChangedTests()
    {
        var source = Build.TestClass<TestNotifyPropertyChanged>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }
}
