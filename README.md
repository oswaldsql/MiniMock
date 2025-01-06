# MiniMock

Mini mock offers a _minimalistic_ approach to mocking in .Net. It is designed to be simple to use and easy to understand.
It is not as feature rich as other mocking frameworks, but aims to solve __95%__ of the use cases. For the remaining __5%__ you should consider creating a custom mock.

Mini mock is __extremely strict__ requiring you to specify all features you want to mock. This is by design to make sure you are aware of what you are mocking.
Unmocked features will throw an exception if used.

## Simple example

```csharp
    public interface IVersionLibrary
    {
        bool DownloadExists(string version);
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

## Key feature summery

- Minimalistic api with fluent method chaining, documentation and full intellisence
- Mocking of interfaces, abstract classes and virtual methods (with limitations)
- Mocking of methods, properties, indexers and events
- Simple factory methods to initialize mocks
- Mocking of Async methods, overloads and generic methods
- Ref and out parameters in methods supported
- Generic interfaces supported

## Limitations

- No validation of calls
- Only support C# (workarounds exist for VB.Net and F#)
- No support for Generic methods ([issue #8](https://github.com/oswaldsql/MiniMock/issues/8))
- Ref return values as ref properties are not supported  ([issue #5](https://github.com/oswaldsql/MiniMock/issues/5))
- Partially mocking of classes
    - Base classes with constructors with parameters are not currently supported ([Issue #4](https://github.com/oswaldsql/MiniMock/issues/4))
- No support for static classes or methods

## Installation & Initialization

Reference nuget package in your test project

```csharp
dotnet add package MiniMock
```

Specify which interface to mock by using the [Mock] attribute before your test or test class.

```csharp
[Fact]
[Mock<IMyRepository>]
public void MyTest {
}
```

Create a mock by using the mock factory

```csharp
var mockRepository = Mock.IMyRepository();
```

Configure your mock to your needs

```csharp
var mockRepo = Mock.IMyRepository(config => config.CreateCustormerAsync(return: Guid.NewGuid());
```

Use the mocked object

```csharp
var sut = new CustomerMaitinance(mockRepo);
sut.Create(customerDTO, cancelationToken);
```

## Quality of life features

### Fluent interface with full intellisence and documentation.

All mockable members are available through a _fluent interface_ with _intellisence_, _type safety_ and _documentation_.

Since the mock code is generated at development time you can _inspect_, _stepped into_ and _debug_ the code. This also allows for _security_ and _vulnerability scanning_ of the code.

All code required to run MiniMock is generated and has _no runtime dependencies_.

### Simple return values

Simply specify what you expect returned from methods or properties. All parameters are ignored.

```csharp
    var mockLibrary = Mock.IVersionLibrary(config => config
                .DownloadExists(returns: true) // Returns true for any parameter
                .DownloadLinkAsync(returns: new Uri("http://downloads/2.0.0")) // Returns a task with a download link
                .CurrentVersion(value: new Version(2, 0, 0, 0)) // Sets the initial version to 2.0.0.0
                .Indexer(values: versions) // Provides a dictionary to retrieve and store versions
    );
```

### Multiple return values

Specify multiple return values for a method or property. The first value is returned for the first call, the second for the second call and so on.

```csharp
    var mockLibrary = Mock.IVersionLibrary(config => config
                .DownloadExists(returnValues: true, false, true) // Returns true, false, true for the first, second and third call
                .DownloadLinkAsync(returnValues: [Task.FromResult(new Uri("http://downloads/2.0.0")), Task.FromResult(new Uri("http://downloads/2.0.1"))]) // Returns a task with a download link for the first and second call
                .DownloadLinkAsync(returnValues: new Uri("http://downloads/2.0.0"), new Uri("http://downloads/2.0.1")) // Returns a task with a download link for the first and second call
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

Raise events using an event trigger.

```csharp
    Action<Version>? triggerNewVersionAdded = null;
    
    var versionLibrary = Mock.IVersionLibrary(config => config
            .NewVersionAdded(trigger: out triggerNewVersionAdded) // Provides a trigger for when a new version is added
        );
    
    triggerNewVersionAdded?.Invoke(new Version(2, 0, 0, 0));
```

### Argument matching

MiniMock does not support argument matching using matchers like other mocking frameworks.
Instead, you can use the call parameter to match arguments using predicates or internal functions.

```csharp
    var versionLibrary = Mock.IVersionLibrary(config => config
            .DownloadExists(call: version => version is { Major: 2, Minor: 0 }) // Returns true for version 2.0.x based on a version parameter
        );
```

__using internal functions__

```csharp
    var versionLibrary = Mock.IVersionLibrary(config =>
    {
       bool downloadExists(Version version) => version switch {
               { Major: 1, Minor: 0 } => true, // Returns true for version 1.0.x based on a version parameter
               { Major: 2, Minor: 0, Revision: 0 } => true, // Returns true for version 2.0.0.0 based on a version parameter
               { Major: 3, } => false, // Returns false for version 3.x based on a version parameter
               _ => throw new ArgumentException() // Throws an exception for all other versions
           };
    
       config.DownloadExists(downloadExists);
    });
```
