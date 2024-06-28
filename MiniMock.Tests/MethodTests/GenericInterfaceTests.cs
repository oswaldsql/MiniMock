namespace MiniMock.Tests.MethodTests;

[Mock<IGeneric<string>>]
[Mock<IGeneric<int>>]
public class GenericInterfaceTests
{
    public interface IGeneric<T>
    {
        public T ReturnGenericType();
        public void GenericParameter(T source);
    }

    [Fact]
    public void GenericStringClass_ShouldReturnGenericType()
    {
        // Arrange
        var sut = Mock.IGeneric_String(mock => mock.ReturnGenericType("Result"));

        // Act
        var actual = sut.ReturnGenericType();

        // Assert
        Assert.Equal("Result", actual);
    }

    [Fact]
    public void GenericIntClass_ShouldReturnGenericType()
    {
        // Arrange
        var sut = Mock.IGeneric_Int32(mock => mock.ReturnGenericType(10));

        // Act
        var actual = sut.ReturnGenericType();

        // Assert
        Assert.Equal(10, actual);
    }

    [Fact]
    public void GenericStringClass_ShouldBeAbleToSerGenericType()
    {
        // Arrange
        var actual = "";
        var sut = Mock.IGeneric_String(mock => mock.GenericParameter(value => actual = value));

        // Act
        sut.GenericParameter("New value");

        // Assert
        Assert.Equal("New value", actual);
    }

    [Fact]
    public void GenericIntClass_ShouldBeAbleToSerGenericType()
    {
        // Arrange
        var actual = 0;
        var sut = Mock.IGeneric_Int32(mock => mock.GenericParameter(value => actual = value));

        // Act
        sut.GenericParameter(10);

        // Assert
        Assert.Equal(10, actual);
    }
}
