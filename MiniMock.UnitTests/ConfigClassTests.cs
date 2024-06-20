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
    internal interface IEmptyInterface
    {
    }

    [Fact]
    public void EmptyInterfaceTests()
    {
        var source = Build.TestClass<IEmptyInterface>();

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.True(result.diagnostics.HasNoErrors());
    }

    internal class EmptyClass
    {
    }

    [Fact]
    public void EmptyClassTests()
    {
        var source = Build.TestClass<EmptyClass>();

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.True(result.diagnostics.HasNoErrors());
    }

    internal sealed class SealedClass
    {
    }

    [Fact]
    public void SealedClassTests()
    {
        var source = Build.TestClass<SealedClass>();

        var (syntaxTrees, diagnostics) = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(syntaxTrees, diagnostics);

        Assert.True(diagnostics.HasErrors());
        Assert.Contains(diagnostics, t => t.Id == "CS0509"); // Inheritance of sealed class is not allowed
    }

    internal abstract class AbstractClass
    {
    }

    [Fact]
    public void AbstractClassTests()
    {
        var source = Build.TestClass<AbstractClass>();

        var (syntaxTrees, diagnostics) = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(syntaxTrees, diagnostics);

        Assert.True(diagnostics.HasNoErrors());
    }
}
