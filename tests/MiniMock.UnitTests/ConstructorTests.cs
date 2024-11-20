namespace MiniMock.UnitTests;

public class ConstructorTests(ITestOutputHelper testOutputHelper)
{
/// <summary>
/// teste
/// </summary>
    public class MultiCtorClass
    {
        /// <summary>
        /// Empty ctor
        /// </summary>
        public MultiCtorClass()
        {

        }


        /// <summary>
        /// one parameter
        /// </summary>
        /// <param name="name">Name to set</param>
        public MultiCtorClass(string name)
        {

        }

        /// <summary>
        /// Two Parameters
        /// </summary>
        /// <param name="name">Name to set</param>
        /// <param name="age">Age to set</param>
        public MultiCtorClass(string name, int age)
        {

        }
    }

    [Fact]
    public void NoneNullableValueTypesShouldBePermitted()
    {
        var source = Build.TestClass<MultiCtorClass>();

        var generate = new MiniMockGenerator().Generate(source);

        var code = generate.syntaxTrees.Where(t => t.FilePath.EndsWith("MiniMock.Mock.g.cs")).ToArray();

        testOutputHelper.DumpResult(code, generate.diagnostics);

        Assert.Empty(generate.GetWarnings());
    }

    public interface ISupportedStaticInterfaceMembers
    {
        static ISupportedStaticInterfaceMembers() => StaticProperty = "Set from ctor";

        static string StaticProperty { get; set; }
    }

    [Fact]
    public void StaticConstructorsDosNotCount()
    {
        var source = Build.TestClass<ISupportedStaticInterfaceMembers>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetWarnings());
    }

    public abstract class AbstractClass
    {
    }

    [Fact]
    public void AbstractClassTest()
    {
        var source = Build.TestClass<AbstractClass>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetWarnings());
    }
}
