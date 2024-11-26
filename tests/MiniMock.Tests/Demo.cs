// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedVariable
// ReSharper disable RedundantLambdaParameterType
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

        var versionLibrary = Mock.IVersionLibrary(config => config
                .DownloadExists(returns: true) // Returns true for all versions
                .DownloadExists(throws: new IndexOutOfRangeException()) // Throws IndexOutOfRangeException for all versions
                .DownloadExists(call: s => s.StartsWith("2.0.0")) // Returns true for version 2.0.0.x base on a string parameter
                .DownloadExists(call: v => v is { Major: 2, Minor: 0, Revision: 0 }) // Returns true for version 2.0.0.x based on a version parameter
                .DownloadExists(returnValues: [true, true, false]) // Returns true two times, then false

                .DownloadLinkAsync(returns: Task.FromResult(new Uri("http://downloads/2.0.0"))) // Returns a task containing a download link for all versions
                .DownloadLinkAsync(call: s => Task.FromResult(s.StartsWith("2.0.0") ? new Uri("http://downloads/2.0.0") : new Uri("http://downloads/UnknownVersion"))) // Returns a task containing a download link for version 2.0.0.x otherwise a error link
                .DownloadLinkAsync(throws: new TaskCanceledException()) // Throws IndexOutOfRangeException for all parameters
                .DownloadLinkAsync(returns: new Uri("http://downloads/2.0.0")) // Returns a task containing a download link for all versions
                .DownloadLinkAsync(call: s => s.StartsWith("2.0.0") ? new Uri("http://downloads/2.0.0") : new Uri("http://downloads/UnknownVersion")) // Returns a task containing a download link for version 2.0.0.x otherwise a error link
                .DownloadLinkAsync(returnValues: [Task.FromResult(new Uri("http://downloads/1.0.0")), Task.FromResult(new Uri("http://downloads/1.1.0")), Task.FromResult(new Uri("http://downloads/2.0.0"))]) // Returns a task with a download link
                .DownloadLinkAsync(returnValues: [new Uri("http://downloads/2.0.0"), new Uri("http://downloads/2.0.0"), new Uri("http://downloads/2.0.0")]) // Returns a task with a download link

                .CurrentVersion(get: () => new Version(2, 0, 0, 0), set: version => throw new IndexOutOfRangeException()) // Overwrites the property getter and setter
                .CurrentVersion(value: new Version(2, 0, 0, 0)) // Sets the initial version to 2.0.0.0

                .Indexer(get: key => new Version(2, 0, 0, 0), set: (key, value) => { }) // Overwrites the indexer getter and setter
                .Indexer(values: versions) // Provides a dictionary to retrieve and store versions

                .NewVersionAdded(raise: new Version(2, 0, 0, 0)) // Raises the event right away
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

        var versionLibrary = Mock.IVersionLibrary(config => config
                .DownloadExists(returns: true) // Returns true for all versions
                .DownloadLinkAsync(returns: Task.FromResult(new Uri("http://downloads/2.0.0"))) // Returns a task with a download link
                .DownloadLinkAsync(returns: new Uri("http://downloads/2.0.0")) // Returns a task with a download link
                .CurrentVersion(value: new Version(2, 0, 0, 0)) // Sets the initial version to 2.0.0.0
                .Indexer(values: versions) // Provides a dictionary to retrieve and store versions
        );
    }

    [Fact]
    [Mock<IVersionLibrary>]
    public void MultipleReturnValue()
    {
        var versionLibrary = Mock.IVersionLibrary(config => config
                .DownloadExists(returnValues: [true, true, false]) // Returns true two times, then false
                .DownloadLinkAsync(returnValues: [Task.FromResult(new Uri("http://downloads/1.0.0")), Task.FromResult(new Uri("http://downloads/1.1.0")), Task.FromResult(new Uri("http://downloads/2.0.0"))]) // Returns a task with a download link
                .DownloadLinkAsync(returnValues: [new Uri("http://downloads/2.0.0"), new Uri("http://downloads/2.0.0"), new Uri("http://downloads/2.0.0")]) // Returns a task with a download link
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
    public void InterceptMethodCalls()
    {
        var currentVersionMock = new Version(2, 0, 0);

        var versionLibrary = Mock.IVersionLibrary(config =>
                config
                    .DownloadExists(call: (string s) => s.StartsWith("2.0.0") ) // Returns true for version 2.0.0.x base on a string parameter
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

    [Fact]
    public void ArgumentMatching()
    {
        // Arrange
        var sut = Mock.IVersionLibrary(config =>
        {
            bool DownloadExists(Version version) => version switch {
                    { Major: 1, Minor: 0 } => true,
                    { Major: 2, Minor: 0, Revision: 0 } => true,
                    { Major: 3, } => false,
                    _ => throw new ArgumentException()
                };

            config.DownloadExists(DownloadExists);
        });

        // ACT
        var actual = sut.DownloadExists(new Version(2, 0, 0, 0));

        // Assert
        Assert.True(actual);
    }

    /// <summary>
    ///  Represents a library that can download versions.
    /// </summary>
    public interface IVersionLibrary
    {
        /// <summary> Gets or sets the current version of the library. </summary>
        Version CurrentVersion { get; set; }

        /// <summary> Gets a value indicating whether a download exists for the specified version. </summary>
        /// <param name="version">The version as a <c>string</c></param>
        /// <returns><c>true</c> if exists, otherwise <c>false</c></returns>
        bool DownloadExists(string version);

        /// <summary> Gets a value indicating whether a download exists for the specified version. </summary>
        /// <param name="version">The version</param>
        /// <returns><c>true</c> if exists, otherwise <c>false</c></returns>
        bool DownloadExists(Version version);

        /// <summary>
        /// Gets the download link for the specified version.
        /// </summary>
        /// <param name="version">The version</param>
        /// <returns>The uri to the specified version</returns>
        Task<Uri> DownloadLinkAsync(string version);

        /// <summary>
        ///  Gets the versoion for the specified key.
        /// </summary>
        /// <param name="key">The version key.</param>
        Version this[string key] { get; set; }

        /// <summary>
        ///  Occurs when a new version is added.
        /// </summary>
        event EventHandler<Version> NewVersionAdded;
    }
}
