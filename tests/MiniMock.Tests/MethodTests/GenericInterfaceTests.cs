namespace MiniMock.Tests.MethodTests;

[Mock<IGeneric<int, int>>]
public class GenericInterfaceTests
{
    public interface IGeneric<TKey, TValue> where TKey : new() where TValue : new()
    {
        public TKey ReturnGenericType();
        public void GenericParameter(TKey source);

        public TKey Name { get; set; }
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
            .TryParse((string value, out string result) => { result = value.ToString(); return true; })
        );

        // Act
        sut.EventWithArgs += (sender, e) => { };
        var actual = sut.ReturnGenericType();
        var t = sut.TryParse("test", out var result2);

        sut["key"] = "value";


        // Assert
        Assert.Equal("Result", actual);
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
        T ReturnTwoGenerics<T, U>(string value) where T : struct where U : struct;
    }

    [Mock<IGenericMethod>]
    [Fact]
    public void METHOD()
    {
        // Arrange
        var sut = Mock.IGenericMethod(config =>
        {
            object Call(string value, Type t)
            {
                if(t == typeof(int))
                    return 10;
                return true;
            }

            config.ReturnGeneric(10).ReturnGeneric(call: Call);
        });

        // ACT
        sut.ReturnGeneric<int>("test");
        sut.ReturnGeneric<bool>("test");

        // Assert

    }
}
