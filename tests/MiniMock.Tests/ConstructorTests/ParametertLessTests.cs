namespace MiniMock.Tests.ConstructorTests;

public class ParameterLessTests
{
    public class ParameterLessClass
    {
        public ParameterLessClass() => this.CtorIsCalled = true;

        public bool CtorIsCalled;

        public bool ImplicitCtorIsCalled { get; set; } = true;
    }

    public interface IInterfaceWithoutCtor
    {
    }

    [Fact]
    [Mock<ParameterLessClass>]
    public void ParameterlessConstructorCanBeUsed()
    {
        // Arrange

        // ACT
        var sut = Mock.ParameterLessClass();

        // Assert
        Assert.IsAssignableFrom<ParameterLessClass>(sut);
        Assert.True(sut.CtorIsCalled);
        Assert.True(sut.ImplicitCtorIsCalled);
    }

    [Fact]
    [Mock<IInterfaceWithoutCtor>]
    public void InterfaceWithoutCtorCanBeUsed()
    {
        // Arrange

        // ACT
        var sut = Mock.IInterfaceWithoutCtor();

        // Assert
        Assert.IsAssignableFrom<IInterfaceWithoutCtor>(sut);
    }
}
