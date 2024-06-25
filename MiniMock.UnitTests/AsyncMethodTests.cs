namespace MiniMock.UnitTests;

using System.ComponentModel;
using System.Numerics;
using MiniMock.UnitTests.Util;

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

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
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

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }

    [Fact]
    public void INotifyPropertyChangedTests()
    {
        var source = Build.TestClass<INotifyPropertyChanged>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }
}

