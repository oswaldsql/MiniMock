namespace MiniMock.Tests.MethodTests;

using Xunit.Abstractions;

public class InheritanceTests(ITestOutputHelper testOutputHelper)
{
    public interface IBaseWithMethods
    {
        bool method1();
        void method2();
        bool method3(string name);
        bool method4(out string name);
        void method5(string name);
    }

    public interface IDerivedWithMethods : IBaseWithMethods
    {
        bool method1();
        void method2();
        bool method3(string name);
        bool method4(out string name);
        void method5(string name);
    }

    [Fact]
    [Mock<IDerivedWithMethods>]
    public void MethodInheritanceTests()
    {

    }

    public interface IBaseWithProperties
    {
        string Name1 { get; set; }
        string Name2 { set; }
        string Name3 { get; }
    }

    public interface IDerivedWithProperties : IBaseWithProperties
    {
        string Name1 { get; set; }
        string Name2 { set; }
        string Name3 { get; }
    }


    [Fact]
    [Mock<IDerivedWithProperties>]
    public void PropertyInheritanceTests()
    {

    }


    public interface IBaseWithEvent
    {
        event EventHandler<string> Event1;
    }

    public interface IDerivedWithEvent : IBaseWithEvent
    {
        event EventHandler<string> Event1;
    }


    [Fact]
    [Mock<IDerivedWithEvent>]
    public void EventInheritanceTests()
    {
        // Arrange
        var eventTriggered = false;
        var baseEventTriggered = false;
        Action<string> trigger = null;
        var sut = Mock.IDerivedWithEvent(config => config.Event1(out trigger));

        sut.Event1 += (_, _) => { eventTriggered = true; };
        ((IBaseWithEvent)sut).Event1 += (_, _) => baseEventTriggered = true;

        // Act
        trigger("EventArgs.Empty");

        // Assert
        Assert.True(eventTriggered);
        Assert.True(baseEventTriggered);
    }
}
