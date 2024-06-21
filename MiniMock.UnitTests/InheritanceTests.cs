namespace MiniMock.UnitTests;

using MiniMock.UnitTests.Util;

public class InheritanceTests(ITestOutputHelper testOutputHelper)
{
    public interface IInheritance
    {
        void method1();
    }

    public interface IInheritance2 : IInheritance
    {
        void method2();
    }

    [Fact]
    public void SimpleInheritanceTests()
    {
        var source = Build.TestClass<IInheritance2>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }

    public class Inheritance
    {
        public void method1() { }
        public virtual void method2() { }
    }

    [Fact]
    public void ClassInheritanceTests()
    {
        var source = Build.TestClass<Inheritance>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetWarnings());
    }

}
