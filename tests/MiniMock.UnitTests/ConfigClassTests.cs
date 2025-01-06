namespace MiniMock.UnitTests;

/*Missing
 * Inherited interfaces
 * logging
 * Documentation
 * Static methods
 *
 * Fixed
 * Nested interfaces
 * Internal classes (with internals visible to)
 * Generic interfaces
 * Polymorphism
 * default implementations
 *
 */
public class ConfigClassTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void EmptyInterfaceTests()
    {
        var source = Build.TestClass<IEmptyInterface>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }

    [Fact]
    public void EmptyClassTests()
    {
        var source = Build.TestClass<EmptyClass>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }

    [Fact]
    public void SealedClassTests()
    {
        var source = Build.TestClass<SealedClass>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.True(generate.diagnostics.HasErrors());
        Assert.Contains(generate.GetErrors(), t => t.Id == "MM0006"); // Inheritance of sealed class is not allowed
    }

    [Fact]
    public void AbstractClassTests()
    {
        var source = Build.TestClass<AbstractClass>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }

    internal interface IEmptyInterface
    {
    }

    internal class EmptyClass
    {
    }

    internal sealed class SealedClass
    {
    }

    internal abstract class AbstractClass
    {
    }
}
