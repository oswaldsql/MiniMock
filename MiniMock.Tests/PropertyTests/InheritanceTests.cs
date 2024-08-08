namespace MiniMock.Tests.PropertyTests;

public class InheritanceTests
{
    public interface IBaseWithProperties
    {
        string Name1 { get; set; }
        string Name2 { set; }
        string Name3 { get; }
    }

    public interface IDerivedWithProperties : IBaseWithProperties
    {
        new string Name1 { get; set; }
        new string Name2 { set; }
        new string Name3 { get; }
    }

    [Fact]
    [Mock<IDerivedWithProperties>]
    public void CallsToBaseInterfaceShouldThrowException()
    {
        var dummy = "";
        var sut = Mock.IDerivedWithProperties() as IBaseWithProperties;

        Assert.Throws<InvalidOperationException>(() => sut.Name1 = "test");
        Assert.Throws<InvalidOperationException>(() => dummy = sut.Name1);
        Assert.Throws<InvalidOperationException>(() => sut.Name2 = "test");
        Assert.Throws<InvalidOperationException>(() => dummy = sut.Name3);
    }

    [Fact]
    [Mock<IDerivedWithProperties>]
    public void CallsToDerivedInterfaceShouldThrowException()
    {
        var dummy = "";
        var sut = Mock.IDerivedWithProperties();

        Assert.Throws<InvalidOperationException>(() => sut.Name1 = "test");
        Assert.Throws<InvalidOperationException>(() => dummy = sut.Name1);
        Assert.Throws<InvalidOperationException>(() => sut.Name2 = "test");
        Assert.Throws<InvalidOperationException>(() => dummy = sut.Name3);
    }

}
