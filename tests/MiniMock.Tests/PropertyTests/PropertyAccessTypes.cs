// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable MemberCanBePrivate.Global

namespace MiniMock.Tests.PropertyTests;

public class PropertyAccessTypes
{
    [Fact]
    [Mock<IPropertyAccessTypes>]
    public void PropertyInheritanceTests()
    {
        var dummy = "";
        var sut = Mock.IPropertyAccessTypes();

        Assert.Throws<InvalidOperationException>(() => sut.Name1 = "test");
        Assert.Throws<InvalidOperationException>(() => dummy = sut.Name1);
        Assert.Throws<InvalidOperationException>(() => sut.Name2 = "test");
        Assert.Throws<InvalidOperationException>(() => dummy = sut.Name3);
    }

    public interface IPropertyAccessTypes
    {
        string Name1 { get; set; }
        string Name2 { set; }
        string Name3 { get; }
    }
}
