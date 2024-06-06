namespace MiniMock.Tests;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

public interface ICustomerRepository
{
    public Task<Guid> Create(string name, CancellationToken token);
    public Task<Customer> Read(Guid id, CancellationToken token);
    public Task Update(Customer updated, CancellationToken token);
    public Task Delete(Guid id, CancellationToken token);
}

public interface IMailService
{
    public Task SendMail(string to, string subject, string body, CancellationToken token);
}

public record Customer(string Name, Guid Id);

[Mock<ICustomerRepository>]
[Mock<IMailService>]
[Mock<ILoveThisLibrary>]
public class Demo(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public async Task UseCustomerRepo()
    {
        var newGuid = Guid.NewGuid();

        var repo = Mock.ICustomerRepository(config => config
            .Create(newGuid)
            .Update(Task.CompletedTask)
            .Read((guid, _) => new("test", guid)));

        var mail = Mock.IMailService(config => config
                   .SendMail(Task.CompletedTask));

        repo.Create("test", CancellationToken.None);

        var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => repo.Delete(newGuid, CancellationToken.None));

        testOutputHelper.WriteLine(actual.Source);
    }

    [Fact]
    [Mock<ILoveThisLibrary>]
    public void IsLibraryLovable()
    {
        var versions = new Dictionary<string, Version>() {{"current", new Version(2,0,0,0)}};

        var lovable = Mock.ILoveThisLibrary(config =>
            config
                .DownloadExists(returns: true) // Returns true for all versions
                .DownloadExists(throws: new IndexOutOfRangeException()) // Throws IndexOutOfRangeException for all versions
                .DownloadExists(call: s => s.StartsWith("2.0.0") ? true : false ) // Returns true for version 2.0.0.x

                .DownloadExistsAsync(returns: Task.FromResult(true)) // Returns true for all versions
                .DownloadExistsAsync(call: s => Task.FromResult(s.StartsWith("2.0.0") ? true : false)) // Returns true for version 2.0.0.x
                .DownloadExistsAsync(returns: true) // Returns true for all versions
                .DownloadExistsAsync(throws: new IndexOutOfRangeException()) // Throws IndexOutOfRangeException for all versions
                .DownloadExistsAsync(call: s => s.StartsWith("2.0.0") ? true : false) // Returns true for version 2.0.0.x

                .Version(value: new Version(2, 0, 0, 0)) // Sets the version to 2.0.0.0
                .Version(onGet: () => new Version(2,0,0,0)) // Gets the version as 2.0.0.0
                .Version(onSet: version => throw new IndexOutOfRangeException()) // Throws IndexOutOfRangeException when setting the version

                .Indexer(values: versions) // Provides a dictionary to retrieve and store versions
                .On_stringIndex_Get(mock: s => versions[s]) // Calls specific method to get the version
                .On_stringIndex_Set(mock: (s, v) => versions[s] = v) // Calls specific method to set the version

                .Trigger_NewVersionAdded(new Version(3,0,0,0))
            );

        var actual = lovable.DownloadExists("2.0.0.0");

        Assert.True(actual);
    }
}

public interface ILoveThisLibrary
{
    Version Version { get; set; }

    bool DownloadExists(string version);
    Task<bool> DownloadExistsAsync(string version);

    Version this[string key] { get; set; }

    event EventHandler<Version> NewVersionAdded;
}


public interface IMethodRepository2
{
    //Task<Guid> AddG(string name);
    Task Add(string name);

    //void Drop();
    //void DropThis(string name);
    //string ReturnValue();
    //Guid CreateNewCustomer(string name);

    //(string name, int age) GetCustomerInfo(string name);

    //void Unlike() { }

    //static string StaticMethod() => "StaticMethod";

    //public string DefaultImp()
    //{
    //    return "Test";
    //}
}

internal class IMethodRepositoryMock2 : IMethodRepository2
{
    public IMethodRepositoryMock2(System.Action<Config>? config = null)
    {
        var result = new Config(this);
        config = config ?? new System.Action<Config>(t => { });
        config.Invoke(result);
        _MockConfig = result;
    }
    private Config _MockConfig { get; set; }

    public partial class Config
    {
        private readonly IMethodRepositoryMock2 target;

        public Config(IMethodRepositoryMock2 target)
        {
            this.target = target;
        }
    }

    #region System.Threading.Tasks.Task Add(string name)
    public partial class Config
    {
        internal System.Func<string, System.Threading.Tasks.Task> On_Add { get; set; } = (_) => throw new System.NotImplementedException();
        private Config _Add(System.Func<string, System.Threading.Tasks.Task> call)
        {
            this.On_Add = call;
            return this;
        }
        public Config Add(System.Func<string, System.Threading.Tasks.Task> call) => this._Add(call);

        public Config Add(System.Exception throws) => this._Add((_) => throw throws);
        public Config Add(System.Threading.Tasks.Task returns) => this._Add((_) => returns);

        public Config Add(System.Action<string> call) => this._Add((name) => { call(name); return System.Threading.Tasks.Task.CompletedTask; });
    }


    public System.Threading.Tasks.Task Add(string name)
    {
        return _MockConfig.On_Add.Invoke(name);
    }
    #endregion

}
