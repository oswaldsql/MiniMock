namespace MiniMock.UnitTests;

using System.Runtime.CompilerServices;

/*Missing
 * Inherited interfaces
 * default implementations
 * logging
 * Polymorphism
 * Documentation
 * Static methods
 *
 * Fixed
 * Nested interfaces
 * Internal classes (with internals visible to)
 * Generic interfaces
 *
 */
public class ConfigClassTests(ITestOutputHelper testOutputHelper)
{
    internal interface IEmptyInterface
    {
    }

    [Fact]
    public void IEmptyInterfaceTests()
    {
        var source = TestSourceBuilder.BuildEmptyClassWithAttribute<IEmptyInterface>();

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.True(result.diagnostics.HasNoErrors());
    }

    internal class EmptyClass
    {
    }

    [Fact]
    public void IEmptyClassTests()
    {
        var source = TestSourceBuilder.BuildEmptyClassWithAttribute<EmptyClass>();

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
        var source = TestSourceBuilder.BuildEmptyClassWithAttribute<SealedClass>();

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
        var source = TestSourceBuilder.BuildEmptyClassWithAttribute<AbstractClass>();

        var (syntaxTrees, diagnostics) = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(syntaxTrees, diagnostics);

        Assert.True(diagnostics.HasNoErrors());
    }
}

public class Test(string tester){}
