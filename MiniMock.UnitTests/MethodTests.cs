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

    static string StaticMethod() => "StaticMethod";

    public string DefaultImp() => "Test";
}

public class MethodTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void MethodRepositoryTests()
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
        public Task MethodAsync();
        public Task MethodAsync(int i);
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

        Assert.Empty(result.diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error));
    }


    internal class ILoveThisLibraryMock2 : MiniMock.UnitTests.LargeTest.ILoveThisLibrary
    {
        public ILoveThisLibraryMock2(System.Action<Config>? config = null)
        {
            var result = new Config(this);
            config = config ?? new System.Action<Config>(t => { });
            config.Invoke(result);
            _MockConfig = result;
        }
        internal Config _MockConfig { get; set; }

        public partial class Config
        {
            private readonly ILoveThisLibraryMock2 target;

            public Config(ILoveThisLibraryMock2 target)
            {
                this.target = target;
            }
        }

        #region bool DownloadExists(string version)
        public partial class Config
        {
            internal System.Func<string, bool> On_DownloadExists { get; set; } = (_) => throw new System.InvalidOperationException("The method 'DownloadExists' in 'ILoveThisLibrary' is not explicitly mocked.") { Source = "MiniMock.UnitTests.LargeTest.ILoveThisLibrary.DownloadExists(string)" };
            private Config _DownloadExists(System.Func<string, bool> call)
            {
                this.On_DownloadExists = call;
                return this;
            }

            public Config DownloadExists(System.Func<string, bool> call) => this._DownloadExists(call);
            public Config DownloadExists(System.Exception throws) => this._DownloadExists((_) => throw throws);
            public Config DownloadExists(bool returns) => this._DownloadExists((_) => returns);
        }


        bool ILoveThisLibrary.DownloadExists(string version)
        {
            return _MockConfig.On_DownloadExists.Invoke(version);
        }
        #endregion


        #region System.Threading.Tasks.Task<bool> DownloadExistsAsync(string version)
        public partial class Config
        {
            internal System.Func<string, System.Threading.Tasks.Task<bool>> On_DownloadExistsAsync { get; set; } = (_) => throw new System.InvalidOperationException("The method 'DownloadExistsAsync' in 'ILoveThisLibrary' is not explicitly mocked.") { Source = "MiniMock.UnitTests.LargeTest.ILoveThisLibrary.DownloadExistsAsync(string)" };
            private Config _DownloadExistsAsync(System.Func<string, System.Threading.Tasks.Task<bool>> call)
            {
                this.On_DownloadExistsAsync = call;
                return this;
            }

            public Config DownloadExistsAsync(System.Func<string, System.Threading.Tasks.Task<bool>> call) => this._DownloadExistsAsync(call);
            public Config DownloadExistsAsync(System.Exception throws) => this._DownloadExistsAsync((_) => throw throws);
            public Config DownloadExistsAsync(System.Threading.Tasks.Task<bool> returns) => this._DownloadExistsAsync((_) => returns);
            public Config DownloadExistsAsync(bool returns) => this._DownloadExistsAsync((_) => System.Threading.Tasks.Task.FromResult(returns));
            public Config DownloadExistsAsync(System.Func<string, bool> call) => this._DownloadExistsAsync((version) => System.Threading.Tasks.Task.FromResult(call(version)));
        }


        public System.Threading.Tasks.Task<bool> DownloadExistsAsync(string version)
        {
            return _MockConfig.On_DownloadExistsAsync.Invoke(version);
        }
        #endregion


        #region Version
        public partial class Config
        {
            public Config Version(System.Version value)
            {
                this.internal_Version = value;
                this.Get_Version = () => this.internal_Version;
                this.Set_Version = s => this.internal_Version = s;

                return this;
            }

            public Config Version(System.Func<System.Version> get, System.Action<System.Version> set)
            {
                this.Get_Version = get;
                this.Set_Version = set;
                return this;
            }

            private System.Version? internal_Version;
            internal System.Func<System.Version> Get_Version { get; set; } = () => throw new System.InvalidOperationException("The property 'Version' in 'ILoveThisLibrary' is not explicitly mocked.") { Source = "MiniMock.UnitTests.LargeTest.ILoveThisLibrary.Version" };
            internal System.Action<System.Version> Set_Version { get; set; } = s => throw new System.InvalidOperationException("The property 'Version' in 'ILoveThisLibrary' is not explicitly mocked.") { Source = "MiniMock.UnitTests.LargeTest.ILoveThisLibrary.Version" };
        }

        public System.Version Version
        {
            get
            {
                return _MockConfig.Get_Version();
            }
            set
            {
                _MockConfig.Set_Version(value);
            }
        }
        #endregion

        #region System.Version this[string]
        public partial class Config
        {
            internal System.Func<string, System.Version> On_stringIndexGet { get; set; } = (_) => throw new System.InvalidOperationException("The indexer 'this[]' in 'ILoveThisLibrary' is not explicitly mocked.") { Source = "MiniMock.UnitTests.LargeTest.ILoveThisLibrary.this[string]" };
            internal System.Action<string, System.Version> On_stringIndexSet { get; set; } = (_, _) => throw new System.InvalidOperationException("The indexer 'this[]' in 'ILoveThisLibrary' is not explicitly mocked.") { Source = "MiniMock.UnitTests.LargeTest.ILoveThisLibrary.this[string]" };
        }

        public partial class Config
        {
            public Config Indexer(System.Collections.Generic.Dictionary<string, System.Version> values)
            {
                this.On_stringIndexGet = s => values[s];
                this.On_stringIndexSet = (s, v) => values[s] = v;
                return this;
            }
        }

        public partial class Config
        {
            public Config Indexer(System.Func<string, System.Version> get, System.Action<string, System.Version> set)
            {
                this.On_stringIndexGet = get;
                this.On_stringIndexSet = set;
                return this;
            }
        }

        public System.Version this[string index]
        {
            get => _MockConfig.On_stringIndexGet(index);
            set => _MockConfig.On_stringIndexSet(index, value);
        }
        #endregion

        #region System.EventHandler<System.Version> NewVersionAdded
        public partial class Config
        {
            public void NewVersionAdded(out System.Action<System.Version> trigger)
            {
                trigger = args => target.NewVersionAdded?.Invoke(target, args);
            }
        }

        public event System.EventHandler<System.Version>? NewVersionAdded;
        #endregion
    }
}
