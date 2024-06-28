namespace MiniMock.Tests;
using System;
using System.Threading.Tasks;
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

public class Demo(ITestOutputHelper testOutputHelper)
{
    [Fact]
    [Mock<ICustomerRepository>]
    [Mock<IMailService>]
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

        var actual =
            await Assert.ThrowsAsync<InvalidOperationException>(() => repo.Delete(newGuid, CancellationToken.None));

        testOutputHelper.WriteLine(actual.Source);
    }
}

public class Demo2(ITestOutputHelper testOutputHelper)
{
    [Fact]
    [Mock<ILoveThisLibrary>]
    public async Task IsLibraryLovable()
    {
        var versions = new Dictionary<string, Version>() {{"current", new Version(2,0,0,0)}};

        Action<Version>? triggerNewVersionAdded = default;
        var lovable = Mock.ILoveThisLibrary(config =>
            config
                .DownloadExists(returns: true) // Returns true for all versions
                .DownloadExists(throws: new IndexOutOfRangeException()) // Throws IndexOutOfRangeException for all versions
                .DownloadExists(call: s => s.StartsWith("2.0.0") ? true : false ) // Returns true for version 2.0.0.x

                .DownloadLinkAsync(returns: Task.FromResult(new Uri("http://downloads/2.0.0"))) // Returns true for all versions
                .DownloadLinkAsync(call: s => Task.FromResult(s.StartsWith("2.0.0") ? new Uri("http://downloads/2.0.0") : new Uri("http://downloads/UnknownVersion"))) // Returns true for version 2.0.0.x
                .DownloadLinkAsync(new Uri("http://downloads/2.0.0")) // Returns true for all versions
                .DownloadLinkAsync(throws: new IndexOutOfRangeException()) // Throws IndexOutOfRangeException for all versions
                .DownloadLinkAsync(call: s => s.StartsWith("2.0.0") ? new Uri("http://downloads/2.0.0") : new Uri("http://downloads/UnknownVersion")) // Returns true for version 2.0.0.x

                .CurrentVersion(get: () => new Version(2, 0, 0, 0), set: version => throw new IndexOutOfRangeException()) // Overwrites the property getter and setter
                .CurrentVersion(value: new Version(2, 0, 0, 0)) // Sets the initial version to 2.0.0.0

                .Indexer(get: s => new Version(2,0,0,0), set: (s, version) => {}) // Overwrites the indexer getter and setter
                .Indexer(values: versions) // Provides a dictionary to retrieve and store versions

                .NewVersionAdded(eventArgs: new Version(2,0,0,0))
                .NewVersionAdded(trigger: out triggerNewVersionAdded) // Provides a trigger for when a new version is added
            );

        var actual = lovable.DownloadExists("2.0.0.0");
        Assert.True(actual);

        var actualAsync = await lovable.DownloadLinkAsync("2.0.0.0");
        Assert.Equal(new Uri("http://downloads/2.0.0"), actualAsync);

        var preVersion = lovable.CurrentVersion;
        lovable.CurrentVersion = new Version(3, 0, 0, 0);
        var postVersion = lovable.CurrentVersion;
        Assert.NotEqual(postVersion, preVersion);

        var preCurrent = lovable["current"];
        lovable["current"] = new Version(3, 0, 0, 0);
        var postCurrent = lovable["current"];
        Assert.NotEqual(preCurrent, postCurrent);

        lovable.NewVersionAdded += (sender, version) => testOutputHelper.WriteLine($"New version added: {version}");
        triggerNewVersionAdded(new Version(2, 0, 0, 0));
    }
}

public interface ILoveThisLibrary
{
    Version CurrentVersion { get; set; }

    bool DownloadExists(string version);
    Task<Uri> DownloadLinkAsync(string version);

    Version this[string key] { get; set; }

    event EventHandler<Version> NewVersionAdded;
}
