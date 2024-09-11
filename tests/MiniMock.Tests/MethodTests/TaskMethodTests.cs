namespace MiniMock.Tests.MethodTests;
using System;
using System.Collections.Frozen;
using System.Threading.Tasks;

[Mock<IAsyncTaskMethods>]
public class TaskMethodTests
{
    public interface IAsyncTaskMethods
    {
        Task SimpleTask(string name);
        Task<string> TaskWithResult(string name);
    }

    [Fact]
    public async Task SimpleTask_WhenMockNotInitialized_ShouldThrowException()
    {
        // Arrange
        var sut = Mock.IAsyncTaskMethods();

        // Act
        var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.SimpleTask("Whats in a name"));

        // Assert
        Assert.NotNull(actual);
        Assert.Contains("SimpleTask", actual.Message);
        Assert.Contains("SimpleTask", actual.Source);
    }

    [Fact]
    public async Task SimpleTask_WhenMockInitializedWithException_ShouldThrowException()
    {
        // Arrange
        var sut = Mock.IAsyncTaskMethods(mock => mock.SimpleTask(new ArgumentException("Test Exception")));

        // Act and Assert
        var actual = await Assert.ThrowsAsync<ArgumentException>(() => sut.SimpleTask("Whats in a name"));

        // Assert
        Assert.NotNull(actual);
        Assert.Equal("Test Exception", actual.Message);
    }

    [Fact]
    public async Task SimpleTask_WhenMockInitializedWithAction_ShouldCallAction()
    {
        // Arrange
        var actual = "";
        var sut = Mock.IAsyncTaskMethods(mock => mock.SimpleTask(value => actual = value));

        // Act and Assert
        await sut.SimpleTask("Whats in a name");

        // Assert
        Assert.Equal("Whats in a name", actual);
    }

    [Fact]
    public async Task SimpleTask_WhenMockInitializedWithTaskFunction_ShouldCallAction()
    {
        // Arrange
        var actual = "";
        var sut = Mock.IAsyncTaskMethods(mock => mock.SimpleTask(value =>
        {
            actual = value;
            return Task.CompletedTask;
        }));

        // Act and Assert
        await sut.SimpleTask("Whats in a name");

        // Assert
        Assert.Equal("Whats in a name", actual);
    }

    [Fact]
    public async Task SimpleTask_WhenMockInitializedWithTaskCompleted_ShouldNotFail()
    {
        // Arrange
        var sut = Mock.IAsyncTaskMethods(mock => mock.SimpleTask(Task.CompletedTask));

        // Act and Assert
        await sut.SimpleTask("Whats in a name");

        // Assert
    }

    [Fact]
    public async Task TaskWithResult_WhenMockNotInitialized_ShouldThrowException()
    {
        // Arrange
        var sut = Mock.IAsyncTaskMethods();

        // Act
        var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.TaskWithResult("Whats in a name"));

        // Assert
        Assert.NotNull(actual);
        Assert.Contains("TaskWithResult", actual.Message);
        Assert.Contains("TaskWithResult", actual.Source);
    }

    [Fact]
    public async Task TaskWithResult_WhenMockInitializedWithException_ShouldThrowException()
    {
        // Arrange
        var sut = Mock.IAsyncTaskMethods(mock => mock.TaskWithResult(new ArgumentException("Test Exception")));

        // Act and Assert
        var actual = await Assert.ThrowsAsync<ArgumentException>(() => sut.TaskWithResult("Whats in a name"));

        // Assert
        Assert.NotNull(actual);
        Assert.Equal("Test Exception", actual.Message);
    }

    [Fact]
    public async Task TaskWithResult_WhenMockInitializedWithAction_ShouldCallAction()
    {
        // Arrange
        var actual = "";
        var sut = Mock.IAsyncTaskMethods(mock => mock.TaskWithResult(value => actual = value));

        // Act and Assert
        await sut.TaskWithResult("Whats in a name");

        // Assert
        Assert.Equal("Whats in a name", actual);
    }

    [Fact]
    public async Task TaskWithResult_WhenMockInitializedWithTaskFunction_ShouldCallAction()
    {
        // Arrange
        var actual = "";
        var sut = Mock.IAsyncTaskMethods(mock => mock.TaskWithResult(value =>
        {
            actual = value;
            return Task.FromResult("Result");
        }));

        // Act and Assert
        var actualReturnValue = await sut.TaskWithResult("Whats in a name");

        // Assert
        Assert.Equal("Whats in a name", actual);
        Assert.Equal("Result", actualReturnValue);
    }

    [Fact]
    public async Task TaskWithResult_WhenMockInitializedWithTaskCompleted_ShouldNotFail()
    {
        // Arrange
        var sut = Mock.IAsyncTaskMethods(mock => mock.TaskWithResult(Task.FromResult("Return")));

        // Act
        var actual = await sut.TaskWithResult("Whats in a name");

        // Assert
        Assert.Equal("Return", actual);
    }
}

/// <summary>
/// Configures <see cref="MultipleReturnValuesTests.IMultipleReturnValues.Method(string)"/>, <see cref="MultipleReturnValuesTests.IMultipleReturnValues.Method()"/>
/// </summary>
public static class MyClass
{
    internal static IMultipleReturnValuesMock.Config MethodOld(this IMultipleReturnValuesMock.Config c, IEnumerable<string> values)
    {
        var v = values.GetEnumerator();
        c.Method(() =>
        {
            if (v.MoveNext())
            {
                return v.Current;
            }

            throw new Exception("test");
        });
        return c;
    }
}
