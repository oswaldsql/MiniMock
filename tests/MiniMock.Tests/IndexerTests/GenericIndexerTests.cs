namespace MiniMock.Tests.IndexerTests;

using System.Numerics;

public class GenericIndexerTests
{
    public interface IGenericIndexRepository<T, U> where U : new()
    {
        U this[T index] { get; set; }
    }

    [Fact]
    [Mock<IGenericIndexRepository<string, object>>]
    public void GenericMethodsCanBeConfiguredWithDictionary()
    {
        // Arrange
        var values = new Dictionary<string, DateTimeOffset> { { "birthday", new(2024, 08, 27, 0, 0, 00, TimeSpan.Zero) } };
        var sut = Mock.IGenericIndexRepository<string, DateTimeOffset>(c => c.Indexer(values));

        // ACT
        var actual = sut["birthday"];
        sut["nextBirthday"] = new(2025, 08, 27, 0, 0, 00, TimeSpan.Zero);

        // Assert
        Assert.Equal(new DateTimeOffset(2024, 08, 27, 0, 0, 00, TimeSpan.Zero), actual);
        Assert.True(values.ContainsKey("nextBirthday"));
        Assert.Equal(new DateTimeOffset(2025, 08, 27, 0, 0, 00, TimeSpan.Zero), values["nextBirthday"]);
    }
}

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

    public class MyClass : ISupportedStaticInterfaceMembers
    {
    }

    public interface IStaticAbstractInterfaceMembers
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
