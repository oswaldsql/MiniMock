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
        string Name1 { get; set; }
        string Name2 { set; }
        string Name3 { get; }
    }

    public interface IDerived : IBase
    {
        string Name { get; set; }
        string Name2 { set; }
        string Name3 { get; }
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

    public interface IBaseWithEvent
    {
        event EventHandler MyEvent;
    }

    public interface IDerivedWithEvent : IBaseWithEvent
    {
        event EventHandler MyEvent;
    }

    [Fact]
    public void EventInheritanceTests()
    {
        var source = Build.TestClass<IDerivedWithEvent>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetWarnings());

    }

    public interface IBaseWithIndexer
    {
        int this[uint index] { set; }
        int this[int index] { get; }
        int this[string index] { get; set; }
    }

    public interface IDerivedWithIndexer : IBaseWithIndexer
    {
        int this[uint index] { set; }
        int this[int index] { get; }
        int this[string index] { get; set; }
    }

    [Fact]
    public void IndexerInheritanceTests()
    {
        var source = Build.TestClass<IDerivedWithIndexer>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetWarnings());

    }

}


