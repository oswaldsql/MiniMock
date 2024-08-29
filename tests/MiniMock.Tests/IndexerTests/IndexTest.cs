namespace MiniMock.Tests.IndexerTests;

[Mock<IIndexRepository>]
public class IndexTest
{
    internal interface IIndexRepository
    {
        int this[uint index] { set; }
        int this[int index] { get; }
        int this[string index] { get; set; }
        (string name, int age) this[Guid index] { get; set; }
        string this[(string name, int age) index] { get; set; }
    }

    [Fact]
    public void CallToUnConfiguredMockThrowsException()
    {
        // Arrange
        var sut = Mock.IIndexRepository();

        // ACT

        // Assert
        Assert.Throws<InvalidOperationException>(() => sut[(uint)10] = 10);
        Assert.Throws<InvalidOperationException>(() => { _ = sut[10]; });
        Assert.Throws<InvalidOperationException>(() => sut["key"] = 10);
        Assert.Throws<InvalidOperationException>(() => { _ = sut["key"]; });
        Assert.Throws<InvalidOperationException>(() => sut[Guid.Empty] = ("name", 10));
        Assert.Throws<InvalidOperationException>(() => {_ = sut[Guid.Empty]; });
        Assert.Throws<InvalidOperationException>(() => { _ = sut[("name", 10)]; });
        Assert.Throws<InvalidOperationException>(() => sut[("name", 10)] = "value");
    }

    [Fact]
    public void SetterOnlyIndexerCanBeConfigured()
    {
        // Arrange
        uint actualKey = 0;
        var actualValue = 0;

        var source = new Dictionary<uint, int>();
        var sut = Mock.IIndexRepository(config => config.Indexer(source));
        var sut2 = Mock.IIndexRepository(config => config.Indexer(set : (uint key, int value) => { actualKey = key;  actualValue = value; }));

        // ACT
        sut[(uint)10] = 10;
        sut2[(uint)10] = 10;

        // Assert
        Assert.True(source.ContainsKey(10));
        Assert.Equal(10, source[10]);
        Assert.Equal((uint)10, actualKey);
        Assert.Equal(10, actualValue);
    }

    [Fact]
    public void GetterOnlyIndexerCanBeConfigured()
    {
        // Arrange
        var source = new Dictionary<int, int>() {{10,100}};
        var sut = Mock.IIndexRepository(config => config.Indexer(source));
        var sut2 = Mock.IIndexRepository(config => config.Indexer(get : i => i * 10));

        // Act
        var actualFromDictionary = sut[10];
        var actualFromFunction = sut2[10];

        // Assert
        Assert.Equal(100, actualFromDictionary);
       Assert.Equal(100, actualFromFunction);
    }

    [Fact]
    public void StandardIndexerCanBeConfigured()
    {
        // Arrange
        var actualKey = "";
        var actualValue = 0;

        var source = new Dictionary<string, int>() {{"ti",10}};
        var sut = Mock.IIndexRepository(config => config.Indexer(source));
        var sut2 = Mock.IIndexRepository(config => config.Indexer(get : (string s) => 10, set:(string key, int value) => { actualKey = key;  actualValue = value; }));

        // Act
        sut["to"] = 2;
        var actualFromDictionary = sut["ti"];
        sut2["to"] = 2;
        var actualFromFunction = sut2["ti"];

        // Assert
        Assert.Equal(10, actualFromDictionary);
        Assert.Equal(10, actualFromFunction);
        Assert.True(source.ContainsKey("to"));
        Assert.Equal("to", actualKey);
        Assert.Equal(2, actualValue);
    }
}
