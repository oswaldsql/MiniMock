namespace MiniMock.UnitTests;

using Microsoft.CodeAnalysis;

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

    //static string StaticMethod() => "StaticMethod";

    public string DefaultImp() => "Test";
}

public class MethodTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void MethodRepositoryTests()
    {
        var source = Build.TestClass<IMethodRepository>();

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.Empty(result.diagnostics.GetErrors());
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
        var source =  @"namespace Demo;
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
        var source = Build.TestClass<AbstractClass>();

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
        public Task MethodAsync();
        public Task MethodAsync(int i);
    }

    [Fact]
    public void InterfaceWithOverloadsTests()
    {
        var source = Build.TestClass<WithOverloads>();

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.Empty(result.diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error));
    }
}

public class LargeTest(ITestOutputHelper testOutputHelper)
{
    public interface ILoveThisLibrary
    {
        Version Version { get; set; }

        bool DownloadExists(string version);
        Task<bool> DownloadExistsAsync(string version);

        Version this[string key] { get; set; }

        event EventHandler<Version> NewVersionAdded;
    }

    [Fact]
    public void MethodWithOutArgumentTests()
    {
        var source = @"
#nullable enable

namespace Demo;
using MiniMock.UnitTests;
using MiniMock;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;

public class TestClass{

    [Mock<LargeTest.ILoveThisLibrary>]
    public async Task IsLibraryLovable()
    {
        var versions = new Dictionary<string, Version>() { { ""current"", new Version(2, 0, 0, 0) } };

        Action<Version>? triggerNewVersionAdded = default;
        var lovable = Mock.ILoveThisLibrary(config =>
            config
                .DownloadExists(returns: true) // Returns true for all versions
                .DownloadExists(throws: new IndexOutOfRangeException()) // Throws IndexOutOfRangeException for all versions
                .DownloadExists(call: s => s.StartsWith(""2.0.0"") ? true : false) // Returns true for version 2.0.0.x

                .DownloadExistsAsync(returns: Task.FromResult(true)) // Returns true for all versions
                .DownloadExistsAsync(call: s => Task.FromResult(s.StartsWith(""2.0.0"") ? true : false)) // Returns true for version 2.0.0.x
                .DownloadExistsAsync(returns: true) // Returns true for all versions
                .DownloadExistsAsync(throws: new IndexOutOfRangeException()) // Throws IndexOutOfRangeException for all versions
                .DownloadExistsAsync(call: s => s.StartsWith(""2.0.0"") ? true : false) // Returns true for version 2.0.0.x

                .Version(value: new Version(2, 0, 0, 0)) // Sets the initial version to 2.0.0.0
                .Version(get: () => new Version(2,0,0,0), set: version => throw new IndexOutOfRangeException()) // Overwrites the property getter and setter

                .Indexer(values: versions) // Provides a dictionary to retrieve and store versions
                .Indexer(get: s => new Version(2,0,0,0), set: (s, version) => {}) // Overwrites the indexer getter and setter

                .NewVersionAdded(trigger: out triggerNewVersionAdded) // Provides a trigger for when a new version is added
            );

        var actual = lovable.DownloadExists(""2.0.0.0"");
        Assert.True(actual);

        var actualAsync = await lovable.DownloadExistsAsync(""2.0.0.0"");
        Assert.True(actualAsync);

        var preVersion = lovable.Version;
        lovable.Version = new Version(3, 0, 0, 0);
        var postVersion = lovable.Version;
        Assert.NotEqual(postVersion, preVersion);

        var preCurrent = lovable[""current""];
        lovable[""current""] = new Version(3, 0, 0, 0);
        var postCurrent = lovable[""current""];
        Assert.NotEqual(preCurrent, postCurrent);

        lovable.NewVersionAdded += (sender, version) => Console.WriteLine($""New version added: {version}"");
        triggerNewVersionAdded?.Invoke(new Version(2, 0, 0, 0));
    }
}";

        var result = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(result);

        Assert.Empty(result.diagnostics.GetErrors());
    }
}
