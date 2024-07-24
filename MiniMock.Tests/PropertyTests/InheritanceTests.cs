namespace MiniMock.Tests.PropertyTests;

using Xunit.Abstractions;

public class InheritanceTests()
{
    public interface IBaseWithProperties
    {
        string Name1 { get; set; }
        string Name2 { set; }
        string Name3 { get; }
    }

    public interface IDerivedWithProperties2 : IBaseWithProperties
    {
        new string Name1 { get; set; }
        new string Name2 { set; }
        new string Name3 { get; }
    }

    [Fact]
    [Mock<IDerivedWithProperties2>]
    public void PropertyInheritanceTests()
    {
        var dummy = "";
        var sut = Mock.IDerivedWithProperties2(config => {}) as IBaseWithProperties;

        Assert.Throws<InvalidOperationException>(() => sut.Name1 = "test");
        Assert.Throws<InvalidOperationException>(() => dummy = sut.Name1);
        Assert.Throws<InvalidOperationException>(() => sut.Name2 = "test");
        Assert.Throws<InvalidOperationException>(() => dummy = sut.Name3);
    }
}
