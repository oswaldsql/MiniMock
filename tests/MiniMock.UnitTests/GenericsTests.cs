namespace MiniMock.UnitTests;

public class GenericsTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void EventInheritanceTests()
    {
        var source = $@"
namespace Demo;

using MiniMock.UnitTests;
using MiniMock;
using System;


[Mock<MiniMock.UnitTests.GenericsTests.IGeneric<int, int>>]
public class TestClass{{
    public void Test() {{
       {""}
    }}
}}";

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetWarnings());
    }

    [Theory]
    [InlineData("where T : struct", "int, int")]
    [InlineData("where T : class", "Baseclass, int")]
    [InlineData("where T : class?", "Baseclass, int")]
    [InlineData("where T : notnull", "int, int")]
    [InlineData("where T : unmanaged", "int, int")]
    [InlineData("where T : new()", "int, int")]
    [InlineData("where T : Baseclass", "Baseclass, int")]
    [InlineData("where T : Baseclass?", "Baseclass, int")]
    [InlineData("where T : IBaseInterface", "IBaseInterface, int")]
    [InlineData("where T : IBaseInterface?", "IBaseInterface, int")]
    [InlineData("where T : U", "int, int")]
//    [InlineData("where T : default")]
//    [InlineData("where T : allows ref struct")]
    public void GenericInterfaceTests(string constraint, string mockAttribute)
    {
        Console.WriteLine(constraint + " - " + mockAttribute);

        var source = $@"
#nullable enable
namespace Demo;

using MiniMock.UnitTests;
using MiniMock;
using System;

public class Baseclass {{}}
public interface IBaseInterface {{}}

public interface ISutInterface<T, U> {constraint}
{{

}}

[Mock<ISutInterface<{mockAttribute}>>]
public class TestClass{{
    public void Test() {{
       {""}
    }}
}}";
        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetWarnings());
    }

    public interface IGenericMethod
    {
        T parse<T>(string value) where T : struct;
    }

    [Fact]
    public void GenericMethodsInNoGenericInterfaceIsNotSupported()
    {
        var source = Build.TestClass<IGenericMethod>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        var error = Assert.Single(generate.GetErrors());

        Assert.Equal(error.GetMessage(), "Generic methods in non generic interfaces or classes is not currently supported for 'parse' in 'IGenericMethod'");
        Assert.Equal(error.Id, "MM0004");
    }

    public interface IGeneric<out TKey, in TValue> where TKey : IComparable<TKey>? //, IEnumerable<string>?
    {
        TKey Method1(TValue value);
//        void Method2(T value);
//        T Method3();
//        T Method4(out T value);
//        T Method5(ref T value);
    }

}
