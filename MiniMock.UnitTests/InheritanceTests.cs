namespace MiniMock.UnitTests;

using MiniMock.UnitTests.Util;

public class InheritanceTests(ITestOutputHelper testOutputHelper)
{
    public interface IInheritance
    {
        bool method1();
        void method2();
        bool method3(string name);
        bool method4(out string name);
        void method5(string name);
    }

    public interface IInheritance2 : IInheritance
    {
        bool method1();
        void method2();
        bool method3(string name);
        bool method4(out string name);
        void method5(string name);
    }

    [Fact]
    public void SimpleInheritanceTests()
    {
        var source = Build.TestClass<IInheritance2>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetWarnings());
    }

    public interface IBase
    {
        string Name { get; set; }
    }

    public interface IDerived : IBase
    {
        string Name { get; set; }
    }

    [Fact]
    public void ClassInheritanceTests2()
    {
        var source = Build.TestClass<IDerived>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetWarnings());
    }

    public abstract class Inheritance
    {
        public void Method1() { }
        public virtual void Method2() { }
        public abstract void Method3();
    }

    [Fact]
    public void ClassInheritanceTests()
    {
        var source = Build.TestClass<Inheritance>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetWarnings());
        var file = Assert.Single(generate.GetFileContent(nameof(Inheritance)));
        Assert.DoesNotContain("void Method1()", file);
        Assert.Contains("void Method2()", file);
        Assert.Contains("void Method3()", file);
    }

}


