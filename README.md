# MiniMock

MiniMock offers a _minimalistic_ approach to mocking in .NET. It is designed to be simple to use and easy to understand. It is not as feature-rich as other mocking frameworks but aims to solve __95%__ of the use cases. For the remaining __5%__, you should consider creating a custom mock.

MiniMock is __extremely strict__, requiring you to specify all features you want to mock. This is by design to make sure you are aware of what you are mocking. Unmocked features will throw an exception if used.

[View the documentation here](https://oswaldsql.github.io/MiniMock/)

## Table of Contents
- [Simple Example](#simple-example)
- [Key Feature Summary](#key-feature-summary)
- [Limitations](#limitations)
- [Installation & Initialization](#installation--initialization)
- [Quality of Life Features](#quality-of-life-features)
  - [Fluent Interface](#fluent-interface)
  - [Simple Return Values](#simple-return-values)
  - [Multiple Return Values](#multiple-return-values)
  - [Intercept Method Calls](#intercept-method-calls)
  - [Async Methods](#async-methods)
  - [Strict Mocking](#strict-mocking)
  - [Adding Indexers](#adding-indexers)
  - [Raising Events](#raising-events)
  - [Argument Matching](#argument-matching)

## Simple Example

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

## Key Feature Summary

- Minimalistic API with fluent method chaining, documentation, and full IntelliSense
- Mocking of interfaces, abstract classes, and virtual methods (with limitations)
- Mocking of methods, properties, indexers, and events
- Simple factory methods to initialize mocks
- Mocking of async methods, overloads, and generic methods
- Ref and out parameters in methods supported
- Generic interfaces supported

## Limitations

- No validation of calls
- Only supports C# (workarounds exist for VB.NET and F#)
- Ref return values and ref properties are not supported ([issue #5](https://github.com/oswaldsql/MiniMock/issues/5))
- Partial mocking of classes
    - Base classes with constructors with parameters are not currently supported ([Issue #4](https://github.com/oswaldsql/MiniMock/issues/4))
- No support for static classes or methods

## Installation & Initialization

Reference the NuGet package in your test project:

```sh
dotnet add package MiniMock
```

Specify which interface to mock by using the `[Mock<T>]` attribute before your test or test class:

```csharp
[Fact]
[Mock<IMyRepository>] // Specify which interface to mock
public void MyTest() {
    var mockRepo = Mock.IMyRepository(config => config // Create a mock using the mock factory
            .CreateCustomerAsync(return: Guid.NewGuid()) // Configure your mock to your needs
        );
    var sut = new CustomerMaintenance(mockRepo); // Inject the mock into your system under test
    
    sut.Create(customerDTO, cancellationToken);
}
```

## Quality of Life Features

### Fluent Interface

All mockable members are available through a _fluent interface_ with _IntelliSense_, _type safety_, and _documentation_.

Since the mock code is generated at development time, you can _inspect_, _step into_, and _debug_ the code. This also allows for _security_ and _vulnerability scanning_ of the code.

All code required to run MiniMock is generated and has _no runtime dependencies_.

### Simple Return Values

Simply specify what you expect returned from methods or properties. All parameters are ignored.

```csharp
var mockLibrary = Mock.IVersionLibrary(config => config
    .DownloadExists(returns: true) // Returns true for any parameter
    .DownloadLinkAsync(returns: new Uri("http://downloads/2.0.0")) // Returns a task with a download link
    .CurrentVersion(value: new Version(2, 0, 0, 0)) // Sets the initial version to 2.0.0.0
    .Indexer(values: versions) // Provides a dictionary to retrieve and store versions
);
```

### Multiple Return Values

Specify multiple return values for a method or property. The first value is returned for the first call, the second for the second call, and so on.

```csharp
var mockLibrary = Mock.IVersionLibrary(config => config
    .DownloadExists(returnValues: true, false, true) // Returns true, false, true for the first, second, and third call
    .DownloadLinkAsync(returnValues: [Task.FromResult(new Uri("http://downloads/2.0.0")), Task.FromResult(new Uri("http://downloads/2.0.1"))]) // Returns a task with a download link for the first and second call
    .DownloadLinkAsync(returnValues: new Uri("http://downloads/2.0.0"), new Uri("http://downloads/2.0.1")) // Returns a task with a download link for the first and second call
);
```

### Intercept Method Calls

```csharp
[Fact]
[Mock<IVersionLibrary>]
public async Task InterceptMethodCalls()
{
    var currentVersionMock = new Version(2, 0, 0);

    var versionLibrary = Mock.IVersionLibrary(config => config
        .DownloadExists(call: (string s) => s.StartsWith("2.0.0") ? true : false) // Returns true for version 2.0.0.x based on a string parameter
        .DownloadExists(call: (Version v) => v is { Major: 2, Minor: 0, Revision: 0 }) // Returns true for version 2.0.0.x based on a version parameter
        // or
        .DownloadExists(call: LocalIntercept) // Calls a local function
        .DownloadExists(call: version => this.ExternalIntercept(version, true)) // Calls function in class

        .DownloadLinkAsync(call: s => Task.FromResult(new Uri($"http://downloads/{s}"))) // Returns a task containing a download link for version 2.0.0.x otherwise an error link
        .DownloadLinkAsync(call: s => new Uri($"http://downloads/{s}")) // Returns a task containing a download link for version 2.0.0.x otherwise an error link

        .CurrentVersion(get: () => currentVersionMock, set: version => currentVersionMock = version) // Overwrites the property getter and setter
        .Indexer(get: s => new Version(2, 0, 0, 0), set: (s, version) => {}) // Overwrites the indexer getter and setter
    );

    return;

    bool LocalIntercept(Version version)
    {
        return version is { Major: 2, Minor: 0, Revision: 0 };
    }
}

private bool ExternalIntercept(string version, bool startsWith) => startsWith ? version.StartsWith("2.0.0") : version == "2.0.0";
```

### Async Methods

Simply return what you expect from async methods either as a `Task` object or a simple value.

```csharp
var versionLibrary = Mock.IVersionLibrary(config => config
    .DownloadLinkAsync(returns: Task.FromResult(new Uri("http://downloads/2.0.0"))) // Returns a task containing a download link for all versions
    .DownloadLinkAsync(call: s => Task.FromResult(new Uri($"http://downloads/{s}"))) // Returns a task containing a download link for version 2.0.0.x otherwise an error link
    // or
    .DownloadLinkAsync(returns: new Uri("http://downloads/2.0.0")) // Returns a task containing a download link for all versions
    .DownloadLinkAsync(call: s => new Uri($"http://downloads/{s}")) // Returns a task containing a download link for version 2.0.0.x otherwise an error link
);
```

### Strict Mocking

Unmocked features will always throw `InvalidOperationException`.

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

### Adding Indexers

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

### Raising Events

Raise events using an event trigger.

```csharp
Action<Version>? triggerNewVersionAdded = null;

var versionLibrary = Mock.IVersionLibrary(config => config
    .NewVersionAdded(trigger: out triggerNewVersionAdded) // Provides a trigger for when a new version is added
);

triggerNewVersionAdded?.Invoke(new Version(2, 0, 0, 0));
```

### Argument Matching

MiniMock does not support argument matching using matchers like other mocking frameworks. Instead, you can use the call parameter to match arguments using predicates or internal functions.

```csharp
var versionLibrary = Mock.IVersionLibrary(config => config
    .DownloadExists(call: version => version is { Major: 2, Minor: 0 }) // Returns true for version 2.0.x based on a version parameter
);
```

__Using Internal Functions__

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
