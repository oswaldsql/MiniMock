namespace MiniMock.Tests.MethodTests;

using Xunit;

[Mock<IOverloadedMethods>]
public class OverloadedMethodTests
{
    public interface IOverloadedMethods
    {
        int OverloadedMethod();
        string OverloadedMethod(string name);
        string OverloadedMethod(string name, int value);
        string OverloadedMethod(int value, string name);
    }

    [Fact]
    public void OverloadedMethod_WhenMockNotInitialized_ShouldThrowException()
    {
        // Arrange
        var sut = Mock.IOverloadedMethods();

        Mock.IOverloadedMethods(m => m.OverloadedMethod((string _,int _) => "test"));

        // Act
        var actual = Assert.Throws<InvalidOperationException>(() => sut.OverloadedMethod());

        // Assert
        Assert.NotNull(actual);
        Assert.Contains("OverloadedMethod", actual.Message);
        Assert.Contains("OverloadedMethod", actual.Source);
    }

    [Fact]
    public void OverloadedMethod_WhenInitializedWithException_AllOverloadsShouldThrowException()
    {
        // Arrange
        var sut = Mock.IOverloadedMethods(mock => mock.OverloadedMethod(new ArgumentException()));

        // Act

        // Assert
        Assert.Throws<ArgumentException>(() => sut.OverloadedMethod());
        Assert.Throws<ArgumentException>(() => sut.OverloadedMethod("name"));
        Assert.Throws<ArgumentException>(() => sut.OverloadedMethod("name", 10));
    }

    [Fact]
    public void OverloadedMethod_WhenMockInitializedWithNoParameters_ShouldCallMethod()
    {
        // Arrange
        var isCalled = false;
        var sut = Mock.IOverloadedMethods(mock => mock.OverloadedMethod(() =>
        {
            isCalled = true;
            return 10;
        }));

        // Act
        var actual = sut.OverloadedMethod();

        // Assert
        Assert.True(isCalled, "Should be true when the mock is called");
        Assert.Equal(10, actual);
    }

    [Fact]
    public void OverloadedMethod_WhenMockInitializedWithOneParameter_ShouldCallMethod()
    {
        // Arrange
        var actual = "";
        var sut = Mock.IOverloadedMethods(mock => mock.OverloadedMethod(value => actual = value));

        // Act
        sut.OverloadedMethod("Whats in a name");

        // Assert
        Assert.Equal("Whats in a name", actual);
    }

    [Fact]
    public void OverloadedMethod_WhenMockInitializedWithTwoParameters_ShouldCallMethod()
    {
        // Arrange
        var sut = Mock.IOverloadedMethods(mock => mock.OverloadedMethod((string name, int value) => $"{name} {value}"));

        // Act
        var actual = sut.OverloadedMethod("Whats in a name", 10);

        // Assert
        Assert.Equal("Whats in a name 10", actual);
    }
}
