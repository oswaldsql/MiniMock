namespace MiniMock.Tests.IndexerTests;

public class StaticInterfaceTests
{
    public interface ISupportedStaticInterfaceMembers
    {
        static ISupportedStaticInterfaceMembers(){}

        static int StaticProperty { get; set; }
        static string StaticMethod() => "value";
        static event EventHandler StaticEvent;

        static void DoStaticEvent()
        {
            StaticEvent?.Invoke(null, EventArgs.Empty);
        }

        static virtual string Bar => "value";  // with implementation
    }

    public interface IUnSipportedStaticAbstractInterfaceMembers
    {
        static abstract string AbstractProperty { get; set; }
        static abstract string AbstractMethod();
        static abstract event EventHandler StaticEvent;
    }

    [Fact]
    [Mock<ISupportedStaticInterfaceMembers>]
    public void StaticInterfaceCanBeMocked()
    {
        // Arrange
        var sut = Mock.ISupportedStaticInterfaceMembers();

        // ACT

        // Assert
        Assert.NotNull(sut);
    }
}
