namespace MiniMock.UnitTests;

using MiniMock.UnitTests.Util;

public class OutArgumentTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void MethodWithOutArgumentTests()
    {
        var source = Build.TestClass<IMethodWithOutArgument>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }

    public interface IMethodWithOutArgument
    {
        bool OutWithReturn(string s, out int value);
        void OutWithVoid(string s, out int value);
        int WithRef(string s, ref int value);

        //        T OutGeneric<T>(T s, out T value);

        //void RefMethod(ref int value);
    }
}
