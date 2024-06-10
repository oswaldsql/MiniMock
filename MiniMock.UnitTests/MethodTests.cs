namespace MiniMock.UnitTests;

using Microsoft.CodeAnalysis;
using static MiniMock.UnitTests.MethodTests;

public interface IMethodRepository
{
    Task<Guid> AddG(string name);
    Task Add(string name);

    void Drop();
    void DropThis(string name);
    string ReturnValue();
    Guid CreateNewCustomer(string name);

    (string name, int age) GetCustomerInfo(string name);

    void Unlike() { }

    static string StaticMethod() => "StaticMethod";

    public string DefaultImp()
    {
        return "Test";
    }
}

public class MethodTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void IMethodRepositoryTests()
    {
        var source = @"namespace Demo;
using MiniMock.UnitTests;
using MiniMock;
using System;

[Mock<IMethodRepository>]
public class TestClass{
}";

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.Empty(result.diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error));
    }

    public interface IDefaultImplementation
    {
        public string NotDefault();

        public string DefaultImp()
        {
            return "Test";
        }
    }

    [Fact]
    public void DefaultImplementationTests()
    {
        var source = @"namespace Demo;
using MiniMock.UnitTests;
using MiniMock;
using System;

[Mock<MethodTests.IDefaultImplementation>]
public class TestClass{
}";

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.Empty(result.diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error));
    }

    public interface IGeneric<T>
    {
        public T ReturnGenericType();
        public void GenericParameter(T source);
    }

    [Fact]
    public void GenericTests()
    {
        var source = @"namespace Demo;
using MiniMock.UnitTests;
using MiniMock;
using System;

[Mock<MethodTests.IGeneric<string>>]
public class TestClass{
}";

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.Empty(result.diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error));
    }

    public abstract class AbstractClass
    {
        public abstract void AbstractMethod();
        public virtual void VirtualMethod() { }
        public abstract string AbstractProperty { get; set; }
        public virtual string VirtualProperty { get; set; }
    }

    [Fact]
    public void AbstractClassTests()
    {
        var source = @"namespace Demo;
using MiniMock.UnitTests;
using MiniMock;
using System;

[Mock<MethodTests.AbstractClass>]
public class TestClass{
}";

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.Empty(result.diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error));
    }

    public interface WithOverloads
    {
        public void Method();
        public void Method(int i);
        public void Method(string s);
        public void Method(int i, string s);
        public void Method(string s, int i);
        public Task Method(string s, CancellationToken token);
        public Task<int> Method(int i, CancellationToken token);
    }

    [Fact]
    public void InterfaceWithOverloadsTests()
    {
        var source = @"namespace Demo;
using MiniMock.UnitTests;
using MiniMock;
using System;

[Mock<MethodTests.WithOverloads>]
public class TestClass{
}";

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.Empty(result.diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error));
    }

    public interface IMethodWithOutArgument
    {
        public void Method(out int value);
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

        Assert.Empty(result.diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error));
    }

}

