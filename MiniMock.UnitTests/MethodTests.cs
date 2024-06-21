namespace MiniMock.UnitTests;

using Microsoft.CodeAnalysis;
using MiniMock.UnitTests.Util;

public interface IMethodRepository
{
    Task<Guid> AddG(string name);
    Task Add(string name);

    void Drop();
    void DropThis(string name);
    string ReturnValue();
    Guid CreateNewCustomer(string name);

    (string name, int age) GetCustomerInfo(string name);

    virtual void Unlike() { }

    //static string StaticMethod() => "StaticMethod";

    public string DefaultImp() => "Test";
}

public class MethodTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void MethodRepositoryTests()
    {
        var source = Build.TestClass<IMethodRepository>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetWarnings());
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
        var source = Build.TestClass<IDefaultImplementation>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }

    public interface IGeneric<T>
    {
        public T ReturnGenericType();
        public void GenericParameter(T source);
    }

    [Fact]
    public void GenericTests()
    {
        var source =  @"namespace Demo;
using MiniMock.UnitTests;
using MiniMock;
using System;

[Mock<MethodTests.IGeneric<string>>]
public class TestClass{
}";

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error));
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
        var source = Build.TestClass<AbstractClass>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
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
        public Task MethodAsync();
        public Task MethodAsync(int i);
    }

    [Fact]
    public void InterfaceWithOverloadsTests()
    {
        var source = Build.TestClass<WithOverloads>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        Assert.Empty(generate.GetErrors());
    }
}
