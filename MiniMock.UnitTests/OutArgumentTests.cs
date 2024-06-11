namespace MiniMock.UnitTests;

using Microsoft.CodeAnalysis;

public class OutArgumentTests(ITestOutputHelper testOutputHelper)
{
    public interface IMethodWithOutArgument
    {
        public bool TryParse(string s, out int value);

        public void RefMethod(ref int value);
    }

    public class MethodWithOutArgument : IMethodWithOutArgument
    {
        public delegate bool TryParse_Delegate(string s, out int value);
        public TryParse_Delegate On_TryParse { get; set; } = (string s, out int value) => { value = 10; return true; };

        public bool TryParse(string s, out int value) => this.On_TryParse(s, out value);

        public void Set_TryParse(TryParse_Delegate call) => this.On_TryParse = call;

        public void Method(out int value)
        {
            this.On_TryParse = (string s, out int i) => int.TryParse(s, out i);
            this.Set_TryParse(call: (string _, out int i) => int.TryParse("10", out i));
            this.Set_TryParse(call: int.TryParse);
            value = 10;
        }

        public void RefMethod(ref int value) => throw new NotImplementedException();
    }

    [Fact]
    public void MethodWithOutArgumentTests()
    {
        var source = @"namespace Demo;
using MiniMock.UnitTests;
using MiniMock;
using System;

[Mock<MethodTests.IMethodWithOutArgument>]
public class TestClass{
}";

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        //Assert.Empty(result.diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error));
    }
}
