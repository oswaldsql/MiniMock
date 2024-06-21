namespace MiniMock.UnitTests;

using MiniMock.UnitTests.Util;

public class PropertyTest(ITestOutputHelper testOutputHelper)
{
    public interface IPropertyInterface
    {
        string GetSet { get; set; }
        string GetInit { get; init; }
        string? Nullable { get; set; }
        string GetOnly { get; }
        string SetOnly { set; }
    }

    [Fact]
    public void PropertyRepositoryTests()
    {
        var source = Build.TestClass<IPropertyInterface>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }
}
