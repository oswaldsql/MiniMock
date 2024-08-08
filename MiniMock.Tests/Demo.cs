namespace MiniMock.Tests;

using Xunit.Abstractions;

public class Demo(ITestOutputHelper testOutputHelper)
{
    [Fact]
    [Mock<IVersionLibrary>]
    public void SimpleExample()
    {
        var library = Mock.IVersionLibrary(config =>
            config.DownloadExists(returns: true));

        var actual = library.DownloadExists("2.0.0.0");

        Assert.True(actual);
    }

    [Fact]
    [Mock<IVersionLibrary>]
    public async Task TestingAllTheOptions()
    {
        var versions = new Dictionary<string, Version>() {{"current", new Version(2,0,0,0)}};
        Action<Version> triggerNewVersionAdded = _ => { };

        var versionLibrary = Mock.IVersionLibrary(config =>
                config
                    .DownloadExists(returns: true) // Returns true for all versions
                    .DownloadExists(throws: new IndexOutOfRangeException()) // Throws IndexOutOfRangeException for all versions
                    .DownloadExists(call: (string s) => s.StartsWith("2.0.0")) // Returns true for version 2.0.0.x base on a string parameter
                    .DownloadExists(call: (Version v) => v is { Major: 2, Minor: 0, Revision: 0 })// Returns true for version 2.0.0.x based on a version parameter

                    .DownloadLinkAsync(returns: Task.FromResult(new Uri("http://downloads/2.0.0"))) // Returns a task containing a download link for all versions
                    .DownloadLinkAsync(call: s => Task.FromResult(s.StartsWith("2.0.0") ? new Uri("http://downloads/2.0.0") : new Uri("http://downloads/UnknownVersion"))) // Returns a task containing a download link for version 2.0.0.x otherwise a error link
                    .DownloadLinkAsync(throws: new TaskCanceledException()) // Throws IndexOutOfRangeException for all parameters
                    .DownloadLinkAsync(returns: new Uri("http://downloads/2.0.0")) // Returns a task containing a download link for all versions
                    .DownloadLinkAsync(call: s => s.StartsWith("2.0.0") ? new Uri("http://downloads/2.0.0") : new Uri("http://downloads/UnknownVersion")) // Returns a task containing a download link for version 2.0.0.x otherwise a error link

                    .CurrentVersion(get: () => new Version(2, 0, 0, 0), set: version => throw new IndexOutOfRangeException()) // Overwrites the property getter and setter
                    .CurrentVersion(value: new Version(2, 0, 0, 0)) // Sets the initial version to 2.0.0.0

                    .Indexer(get: s => new Version(2,0,0,0), set: (s, version) => {}) // Overwrites the indexer getter and setter
                    .Indexer(values: versions) // Provides a dictionary to retrieve and store versions

                    .NewVersionAdded(eventArgs: new Version(2,0,0,0)) // Raises the event right away
                    .NewVersionAdded(trigger: out triggerNewVersionAdded) // Provides a trigger for when a new version is added
        );

        var actual = versionLibrary.DownloadExists("2.0.0.0");
        Assert.True(actual);

        var actualAsync = await versionLibrary.DownloadLinkAsync("2.0.0.0");
        Assert.Equal(new Uri("http://downloads/2.0.0"), actualAsync);

        var preVersion = versionLibrary.CurrentVersion;
        versionLibrary.CurrentVersion = new Version(3, 0, 0, 0);
        var postVersion = versionLibrary.CurrentVersion;
        Assert.NotEqual(postVersion, preVersion);

        var preCurrent = versionLibrary["current"];
        versionLibrary["current"] = new Version(3, 0, 0, 0);
        var postCurrent = versionLibrary["current"];
        Assert.NotEqual(preCurrent, postCurrent);

        versionLibrary.NewVersionAdded += (sender, version) => testOutputHelper.WriteLine($"New version added: {version}");
        triggerNewVersionAdded(new Version(2, 0, 0, 0));
    }

    [Fact]
    [Mock<IVersionLibrary>]
    public void SimpleReturnValue()
    {
        var versions = new Dictionary<string, Version>() {{"current", new Version(2,0,0,0)}};

        var versionLibrary = Mock.IVersionLibrary(config =>
                config.DownloadExists(returns: true) // Returns true for all versions
                    .DownloadLinkAsync(returns: Task.FromResult(new Uri("http://downloads/2.0.0"))) // Returns a task with a download link
                    .DownloadLinkAsync(returns: new Uri("http://downloads/2.0.0")) // Returns a task with a download link
                    .CurrentVersion(value: new Version(2, 0, 0, 0)) // Sets the initial version to 2.0.0.0
                    .Indexer(values: versions) // Provides a dictionary to retrieve and store versions
        );
    }

    [Fact]
    [Mock<IVersionLibrary>]
    public void Indexers()
    {
        var versions = new Dictionary<string, Version>() {{"current", new Version(2,0,0,0)}};

        var versionLibrary = Mock.IVersionLibrary(config => config
                .Indexer(get: s => new Version(2,0,0,0), set: (s, version) => {}) // Overwrites the indexer getter and setter
                .Indexer(values: versions) // Provides a dictionary to retrieve and store versions
        );

        var preCurrent = versionLibrary["current"];
        versionLibrary["current"] = new Version(3, 0, 0, 0);
        var postCurrent = versionLibrary["current"];
        Assert.NotEqual(preCurrent, postCurrent);
    }

    [Fact]
    [Mock<IVersionLibrary>]
    public async Task InterceptMethodCalls()
    {
        var currentVersionMock = new Version(2, 0, 0);

        var versionLibrary = Mock.IVersionLibrary(config =>
                config
                    .DownloadExists(call: (string s) => s.StartsWith("2.0.0") ? true : false ) // Returns true for version 2.0.0.x base on a string parameter
                    .DownloadExists(call: (Version v) => v is { Major: 2, Minor: 0, Revision: 0 })// Returns true for version 2.0.0.x based on a version parameter
                    //or
                    .DownloadExists(call: LocalIntercept) // calls a local function
                    .DownloadExists(call: version => this.ExternalIntercept(version, true)) // calls function in class

                    .DownloadLinkAsync(call: s => Task.FromResult(new Uri($"http://downloads/{s}"))) // Returns a task containing a download link for version 2.0.0.x otherwise a error link
                    .DownloadLinkAsync(call: s => new Uri($"http://downloads/{s}")) // Returns a task containing a download link for version 2.0.0.x otherwise a error link

                    .CurrentVersion(get: () => currentVersionMock, set: version => currentVersionMock = version) // Overwrites the property getter and setter
                    .Indexer(get: s => new Version(2,0,0,0), set: (s, version) => {}) // Overwrites the indexer getter and setter
        );

        return;

        bool LocalIntercept(Version version)
        {
            return version is { Major: 2, Minor: 0, Revision: 0 };
        }
    }

    private bool ExternalIntercept(string version, bool startsWith) => startsWith ? version.StartsWith("2.0.0") : version == "2.0.0";

    [Fact]
    [Mock<IVersionLibrary>]
    public void UnmockedFeaturesAlwaysThrowInvalidOperationException()
    {
        var versionLibrary = Mock.IVersionLibrary();

        var propertyException = Assert.Throws<InvalidOperationException>(() => versionLibrary.CurrentVersion);
        var methodException = Assert.Throws<InvalidOperationException>(() => versionLibrary.DownloadExists("2.0.0"));
        var asyncException = Assert.ThrowsAsync<InvalidOperationException>(() => versionLibrary.DownloadLinkAsync("2.0.0"));
        var indexerException = Assert.Throws<InvalidOperationException>(() => versionLibrary["2.0.0"]);
    }

    [Fact]
    public void TriggeringEventsUsingAction()
    {
        // Arrange
        Version? actual = null;
        Action<Version>? triggerNewVersionAdded = null;

        var versionLibrary = Mock.IVersionLibrary(config => config
                .NewVersionAdded(trigger: out triggerNewVersionAdded) // Provides a trigger for when a new version is added
        );

        versionLibrary.NewVersionAdded += (sender, version) => actual = version;

        // ACT
        triggerNewVersionAdded?.Invoke(new Version(2, 0, 0, 0));

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(new Version(2,0,0,0), actual);
    }

    public interface IVersionLibrary
    {
        Version CurrentVersion { get; set; }

        bool DownloadExists(string version);
        bool DownloadExists(Version version);

        Task<Uri> DownloadLinkAsync(string version);

        Version this[string key] { get; set; }

        event EventHandler<Version> NewVersionAdded;
    }
}
