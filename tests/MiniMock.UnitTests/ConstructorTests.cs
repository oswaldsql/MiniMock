namespace MiniMock.UnitTests;

public class ConstructorTests(ITestOutputHelper testOutputHelper)
{
    public class MyClass
    {
        public MyClass()
        {

        }

        public MyClass(string name)
        {

        }

        public MyClass(string name, int age)
        {

        }
    }

    public MyClass factory((string name, int age) ctor)
    {
        return new MyClass(ctor.name, ctor.age);
    }

    [Fact]
    public void NoneNullableValueTypesShouldBePermitted()
    {
        var source = Build.TestClass<MyClass>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetWarnings());
    }

    public interface ISupportedStaticInterfaceMembers
    {
        static ISupportedStaticInterfaceMembers() => StaticProperty = "Set from ctor";

        static string StaticProperty { get; set; }
        static string StaticMethod() => "value";
        static event EventHandler? StaticEvent;

        static void DoStaticEvent() => StaticEvent?.Invoke(null, EventArgs.Empty);

        static virtual string Bar => "value";  // with implementation
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
