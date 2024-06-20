namespace MiniMock.UnitTests;

using System.ComponentModel;
using Microsoft.CodeAnalysis;

public class AsyncMethodTests(ITestOutputHelper testOutputHelper)
{
    public interface ISimpleTaskMethods
    {
        Task WithParameter(string name);
        Task WithoutParameter();
    }

    [Fact]
    public void SimpleTaskMethodsTests()
    {
        var source = Build.TestClass<ISimpleTaskMethods>();

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.Empty(result.diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error));
    }

    public interface IGenericTaskMethods
    {
        Task<string> WithParameter(string name);
        Task<int> WithoutParameter();
    }

    [Fact]
    public void GenericTaskMethodsTests()
    {
        var source = Build.TestClass<IGenericTaskMethods>(); 

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.Empty(result.diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error));
    }

    [Fact]
    public void INotifyPropertyChangedTests()
    {
        var source = Build.TestClass<INotifyPropertyChanged>();

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.True(result.diagnostics.HasNoErrors());
    }
}
