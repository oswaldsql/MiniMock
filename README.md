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
    .DownloadExists(returns: true) // Returns true for all versions
    .DownloadExists(throws: new IndexOutOfRangeException()) // Throws IndexOutOfRangeException for all versions
    .DownloadExists(call: s => s.StartsWith("2.0.0") ? true : false ) // Returns true for version 2.0.0.x
```

### Async methods

Simply return what you expect from async methods either as a Task object or a simple value

```csharp
    .DownloadExistsAsync(returns: Task.FromResult(true)) // Returns true for all versions
    .DownloadExistsAsync(call: s => Task.FromResult(s.StartsWith("2.0.0") ? true : false)) // Returns true for version 2.0.0.x
    .DownloadExistsAsync(returns: true) // Returns true for all versions
    .DownloadExistsAsync(throws: new IndexOutOfRangeException()) // Throws IndexOutOfRangeException for all versions
    .DownloadExistsAsync(call: s => s.StartsWith("2.0.0") ? true : false) // Returns true for version 2.0.0.x
```

### Setting properties

Unmocked properties will throw a exception.

```csharp
    .Version(value: new Version(2, 0, 0, 0)) // Sets the version to 2.0.0.0 and initializes the property to allow getting and setting the value
    .Version(onGet: () => new Version(2,0,0,0)) // Gets the version as 2.0.0.0
    .Version(onSet: version => throw new IndexOutOfRangeException()) // Throws IndexOutOfRangeException when setting the version
```

### Adding indexers

```csharp
    var versions = new Dictionary<string, Version>() {{"current", new Version(2,0,0,0)}};

    ...

    .Indexer(values: versions) // Provides a dictionary to retrieve and store versions
    .On_stringIndex_Get(mock: s => versions[s]) // Calls specific method to get the version
    .On_stringIndex_Set(mock: (s, v) => versions[s] = v) // Calls specific method to set the version
```

## Current features

- Mocking of methods, properties, indexers and events
- Mocking of interfaces, abstract classes and virtual methods
- Mocking of Async methods, overloads and generic methods

## Current limitations

- Limited validation of calls
- Ref and out parameters are not currently supported
- Inherited interfaces are not currently supported
