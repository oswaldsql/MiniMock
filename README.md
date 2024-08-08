# MiniMock

Mini mock offers a _minimalistic_ approach to mocking in .Net. It is designed to be simple to use and easy to understand. 
It is not as feature rich as other mocking frameworks, but aims to solve __95%__ of the use cases.
It is designed to be used in simple scenarios where you just need to mock a few methods or properties.

Mini mock is extremely strict requiring you to specify all features you want to mock. This is by design to make sure you are aware of what you are mocking.
Unmocked features will throw an exception if used.

```csharp
    public interface IVersionLibrary
    {
        Version CurrentVersion { get; set; }

        bool DownloadExists(string version);
        bool DownloadExists(Version version);

        Task<Uri> DownloadLinkAsync(string version);

        Version this[string key] { get; set; }

        event EventHandler<Version> NewVersionAdded;
    }

    [Fact]
    [Mock<IVersionLibrary>]
    public void SimpleExample()
    {
        var library = Mock.IVersionLibrary(config =>
            config.DownloadExists(returns: true));

        var actual = library.DownloadExists("2.0.0.0");

        Assert.True(actual);
    }
```

## Quality of life features

### Fluent interface with full intellisence and documentation.

All mockable members are available through a _fluent interface_ with _intellisence_, _type safety_ and _documentation_.

Since the mock code is generated at development time allowing you to _inspect_, _stepped into_ and _debug_. 

All code required to run MiniMock is generated and has _no runtime dependencies_.

### Simple return values

Simply specify what you expect returned from methods or properties. All parameters are ignored.

```csharp
        var mockLibrary = Mock.IVersionLibrary(config => config
                    .DownloadExists(returns: true) // Returns true for all versions
                    .DownloadLinkAsync(returns: new Uri("http://downloads/2.0.0")) // Returns a task with a download link
                    .CurrentVersion(value: new Version(2, 0, 0, 0)) // Sets the initial version to 2.0.0.0
                    .Indexer(values: versions) // Provides a dictionary to retrieve and store versions
        );
```

### Intercept method calls

```csharp
    [Fact]
    [Mock<IVersionLibrary>]
    public async Task InterceptMethodCalls()
    {
        var currentVersionMock = new Version(2, 0, 0);
        
        var versionLibrary = Mock.IVersionLibrary(config => config
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
```

### Async methods

Simply return what you expect from async methods either as a Task object or a simple value.

```csharp
        var versionLibrary = Mock.IVersionLibrary(config => config
                    .DownloadLinkAsync(returns: Task.FromResult(new Uri("http://downloads/2.0.0"))) // Returns a task containing a download link for all versions
                    .DownloadLinkAsync(call: s => Task.FromResult(new Uri($"http://downloads/{s}"))) // Returns a task containing a download link for version 2.0.0.x otherwise a error link
                // or
                    .DownloadLinkAsync(returns: new Uri("http://downloads/2.0.0")) // Returns a task containing a download link for all versions
                    .DownloadLinkAsync(call: s => new Uri($"http://downloads/{s}")) // Returns a task containing a download link for version 2.0.0.x otherwise a error link
            );
```

### Strict mocking

Unmocked features will always throw InvalidOperationException.

```csharp
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
```

### Adding indexers

Mocking indexers is supported either by overloading the get and set methods or by providing a dictionary with expected values.

```csharp
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

```

### Raising events 

Raise events using a event trigger.

```csharp
        Action<Version>? triggerNewVersionAdded = null;

        var versionLibrary = Mock.IVersionLibrary(config => config
                .NewVersionAdded(trigger: out triggerNewVersionAdded) // Provides a trigger for when a new version is added
            );

        triggerNewVersionAdded?.Invoke(new Version(2, 0, 0, 0));
```

## Feature summery

- Mocking of methods, properties, indexers and events
- Mocking of interfaces, abstract classes and virtual methods
- Simple factory methods to initialize mocks
- Mocking of Async methods, overloads and generic methods
- Ref and out parameters in methods supported
- Inherited interfaces are supported
- Limited generic interfaces support

## Current limitations

- Generic interfaces are not fully supported
- No validation of calls
- Ref return values are not currently supported
- Base classes with constructors with parameters are not currently supported
- Limited support of mocking of classes
