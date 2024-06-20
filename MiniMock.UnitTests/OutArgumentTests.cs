namespace MiniMock.UnitTests;

using Microsoft.CodeAnalysis;

public class OutArgumentTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void MethodWithOutArgumentTests()
    {
        var source = Build.TestClass<IMethodWithOutArgument>();

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.Empty(result.diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error));
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
