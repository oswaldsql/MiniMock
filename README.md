# MiniMock

Mini mock offers a __minimalistic__ approach to mocking in .Net. It is designed to be simple to use and easy to understand. 
It is not as feature rich as other mocking frameworks, but aims to solve __95%__ of the usecases.
It is designed to be used in simple scenarios where you just need to mock a few methods or properties.

Mini mock is extreamly strict requiring you to specify all methods and properties you want to mock. This is by design to make sure you are aware of what you are mocking.
Unmocked methods and properties will throw an exception if called.

```csharp
    [Fact]
    [Mock<ILoveThisLibrary>]
    public void IsLibraryLovable()
    {
        var lovable = Mock.ILoveThisLibrary(config =>
            config.DownloadExists(returns: true));

        var actual = lovable.DownloadExists("2.0.0.0");

        Assert.True(actual);
    }
```

## Quality of life features

### Full intellisence

All mockable members are available through intellisence with type safety and documentation.

### Simpel return values

Simply return what you expect from the method

```csharp
        var lovable = Mock.ILoveThisLibrary(config =>
            config
                .DownloadExists(returns: true) // Returns true for all versions
                .DownloadExists(throws: new IndexOutOfRangeException()) // Throws IndexOutOfRangeException for all versions
                .DownloadExists(call: s => s.StartsWith("2.0.0") ? true : false ) // Returns true for version 2.0.0.x
            );
        var actual = lovable.DownloadExists("2.0.0.0");
        Assert.True(actual);
```

### Async methods

Simply return what you expect from async methods either as a Task object or a simple value

```csharp
        var lovable = Mock.ILoveThisLibrary(config =>
            config
                .DownloadExistsAsync(returns: Task.FromResult(true)) // Returns true for all versions
                .DownloadExistsAsync(call: s => Task.FromResult(s.StartsWith("2.0.0") ? true : false)) // Returns true for version 2.0.0.x
                .DownloadExistsAsync(returns: true) // Returns true for all versions
                .DownloadExistsAsync(throws: new IndexOutOfRangeException()) // Throws IndexOutOfRangeException for all versions
                .DownloadExistsAsync(call: s => s.StartsWith("2.0.0") ? true : false) // Returns true for version 2.0.0.x
            );
        var actualAsync = await lovable.DownloadExistsAsync("2.0.0.0");
        Assert.True(actualAsync);
```

### Setting properties

Unmocked properties will throw a exception.

```csharp
        var lovable = Mock.ILoveThisLibrary(config =>
            config
                .Version(value: new Version(2, 0, 0, 0)) // Sets the initial version to 2.0.0.0
                .Version(get: () => new Version(2,0,0,0), set: version => throw new IndexOutOfRangeException()) // Overwrites the property getter and setter
            );
        var preVersion = lovable.Version;
        lovable.Version = new Version(3, 0, 0, 0);
        var postVersion = lovable.Version;
        Assert.NotEqual(postVersion, preVersion);

```

### Adding indexers

```csharp
        var versions = new Dictionary<string, Version>() {{"current", new Version(2,0,0,0)}};

        Action<Version>? triggerNewVersionAdded = default;
        var lovable = Mock.ILoveThisLibrary(config =>
            config
                .Indexer(values: versions) // Provides a dictionary to retrieve and store versions
                .Indexer(get: s => new Version(2,0,0,0), set: (s, version) => {}) // Overwrites the indexer getter and setter
            );

        var preCurrent = lovable["current"];
        lovable["current"] = new Version(3, 0, 0, 0);
        var postCurrent = lovable["current"];
        Assert.NotEqual(preCurrent, postCurrent);
```

### Triggering events 

```csharp
        Action<Version>? triggerNewVersionAdded = null;
        var lovable = Mock.ILoveThisLibrary(config =>
            config
                .NewVersionAdded(trigger: out triggerNewVersionAdded) // Provides a trigger for when a new version is added
            );

        triggerNewVersionAdded?.Invoke(new Version(2, 0, 0, 0));
```

## Current features

- Mocking of methods, properties, indexers and events
- Mocking of interfaces, abstract classes and virtual methods
- Mocking of Async methods, overloads and generic methods

## Current limitations

- Limited validation of calls
- Ref and out parameters are not currently supported
- Inherited interfaces are not currently supported
