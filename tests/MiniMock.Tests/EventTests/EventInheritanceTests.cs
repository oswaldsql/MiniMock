// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable MemberCanBePrivate.Global

namespace MiniMock.Tests.EventTests;

public class EventInheritanceTests
{
    [Fact]
    [Mock<IDerivedWithEvent>]
    public void BothDerivedAndBaseEventsAreTriggered()
    {
        // Arrange
        var eventTriggered = false;
        var baseEventTriggered = false;
        Action<string> trigger = _ => { };
        var sut = Mock.IDerivedWithEvent(config => config.Event1(out trigger));

        sut.Event1 += (_, _) => { eventTriggered = true; };
        ((IBaseWithEvent)sut).Event1 += (_, _) => baseEventTriggered = true;

        // Act
        trigger("EventArgs.Empty");

        // Assert
        Assert.True(eventTriggered);
        Assert.True(baseEventTriggered);
    }

    public interface IBaseWithEvent
    {
        event EventHandler<string> Event1;
    }

    public interface IDerivedWithEvent : IBaseWithEvent
    {
        new event EventHandler<string> Event1;
    }
}
