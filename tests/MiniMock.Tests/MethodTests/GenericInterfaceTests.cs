namespace MiniMock.Tests.MethodTests;

[Mock<IGeneric<int, int>>]
public class GenericInterfaceTests
{
    public interface IGeneric<TKey, TValue> where TKey : new() where TValue : new()
    {
        public TKey ReturnGenericType();
        public void GenericParameter(TKey source);

        public TKey Name { get; set; }

        public TValue Value { get; set; }
    }

    public interface IGeneric<T> where T : notnull
    {
        T ReturnGenericType();
        void GenericParameter(T source);

        bool TryParse(string value, out T result);

        string this[T key] { get; set; }

        event EventHandler<T> EventWithArgs;
    }

    [Fact]
    [Mock<IGeneric<string>>]
    public void GenericStringClass_ShouldReturnGenericType()
    {
        // Arrange
        var sut = Mock.IGeneric<string>(mock => mock
            .Indexer([])
            .ReturnGenericType("Result")
        );

        // Act
        sut.EventWithArgs += (_, _) => { };
        var actual = sut.ReturnGenericType();
        sut["key"] = "value";

        // Assert
        Assert.Equal("Result", actual);
    }

    [Fact]
    public void GenericStringClass_TryParseShouldReturnTrue()
    {
        // Arrange
        var sut = Mock.IGeneric<string>(mock => mock
            .TryParse((string value, out string result) => { result = "parsed " + value; return true; })
        );

        // Act
        var actual = sut.TryParse("test", out var actualResult);

        // Assert
        Assert.True(actual);
        Assert.Equal("parsed test", actualResult);
    }

    [Fact]
    public void GenericIntClass_ShouldReturnGenericType()
    {
        // Arrange
        var sut = Mock.IGeneric<int>(mock => mock.ReturnGenericType(10));

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
        var sut = Mock.IGeneric<string>(mock => mock.GenericParameter(value => actual = value));

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
        var sut = Mock.IGeneric<int>(mock => mock.GenericParameter(value => actual = value));

        // Act
        sut.GenericParameter(10);

        // Assert
        Assert.Equal(10, actual);
    }
}

public class GenericMethodTest
{
    public interface IGenericMethod
    {
        void ReturnGeneric(string value);
        T ReturnGeneric<T>(string value) where T : struct;
        IEnumerable<T> ReturnDerived<T>(string value) where T : struct;
        void ReturnVoid<T>(string value) where T : struct;
        T ReturnTwoGenerics<T, TU>(string value) where T : struct where TU : struct;
    }

    [Mock<IGenericMethod>]
    [Fact]
    public void MockCallFunctionGetTheTypeOfTheGenericAsParameter()
    {
        var sut = Mock.IGenericMethod(config => config.ReturnGeneric(call: ReturnGeneric));

        // ACT
        var actualInt = sut.ReturnGeneric<int>("test");
        var actualBool = sut.ReturnGeneric<bool>("test");

        // Assert
        Assert.Equal(10, actualInt);
        Assert.True(actualBool);
        return;

        // Arrange
        object ReturnGeneric(string value, Type t)
        {
            if (t == typeof(int))
            {
                return 10;
            }
            return true;
        }
    }
}
