namespace MiniMock.UnitTests;

using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

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

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }

    internal class EmptyClass
    {
    }

    [Fact]
    public void EmptyClassTests()
    {
        var source = Build.TestClass<EmptyClass>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }

    internal sealed class SealedClass
    {
    }

    [Fact]
    public void SealedClassTests()
    {
        var source = Build.TestClass<SealedClass>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.True(generate.diagnostics.HasErrors());
        Assert.Contains(generate.GetErrors(), t => t.Id == "CS0509"); // Inheritance of sealed class is not allowed
    }

    internal abstract class AbstractClass
    {
    }

    [Fact]
    public void AbstractClassTests()
    {
        var source = Build.TestClass<AbstractClass>();

        var generate= new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }
}
