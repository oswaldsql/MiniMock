namespace MiniMock.UnitTests;

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
        var source = @"namespace Demo;
using MiniMock.UnitTests;
using MiniMock;
using System;

[Mock<AsyncMethodTests.ISimpleTaskMethods>]
public class TestClass{
}";

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
        var source = @"namespace Demo;
using MiniMock.UnitTests;
using MiniMock;
using System;

[Mock<AsyncMethodTests.IGenericTaskMethods>]
public class TestClass{
}";

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.Empty(result.diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error));
    }
}
