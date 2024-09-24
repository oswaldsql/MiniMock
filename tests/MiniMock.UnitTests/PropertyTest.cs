namespace MiniMock.UnitTests;

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

    public abstract class AbstractClassWithDifferentProperties
    {
        public string NotAbstract { get; set; } = "";
        public abstract string Abstract { get; set; }
        public virtual string Virtual { get; set; } = "";

        public string NotAbstractGetOnly { get;  } = "";
        public abstract string AbstractGetOnly { get;  }
        public virtual string VirtualGetOnly { get;  } = "";

        public string NotAbstractSetOnly { set { } }
        public abstract string AbstractSetOnly { set; }
        public virtual string VirtualSetOnly { set{} }
    }

    [Fact]
    public void PropertyRepositoryTests()
    {
        var source = Build.TestClass<IPropertyInterface>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }

    [Fact]
    public void AbstractClassWithDifferentPropertyTypes()
    {
        var source = Build.TestClass<AbstractClassWithDifferentProperties>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }}
