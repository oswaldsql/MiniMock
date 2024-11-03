# Decision on Supporting Indexers

## Context

In the MiniMock framework, there is a need to determine the scope of support for mocking indexers. Indexers allow objects to be indexed in a similar way to arrays, and supporting them is essential for a comprehensive mocking framework.

## Decision

The MiniMock framework will support mocking indexers. This includes both read-only and read-write indexers, ensuring that the framework can handle a wide range of scenarios.

Indexers must be mockable using the following parameters:

- Get/Set : Delegates matching the indexer's getter and setter signature with functionality to be executed.
- Values : A dictionary optionally containing values to be used as the indexers source.

if none of the above parameters are provided, accessing the indexer must throw a InvalidOperationException with a message in the form "The indexer for '__[indexer type]__' in '__[mocked class]__' is not explicitly mocked.".

## Consequences

### Positive:

- **Comprehensive**: Ensures that the framework can handle indexers in both classes and interfaces.
- **Flexibility**: Provides developers with the ability to mock different kinds of indexers, enhancing the framework's usability.

### Negative:

- **Complexity**: Supporting both read-only and read-write indexers adds complexity to the framework.
- **Maintenance**: Requires ongoing maintenance to ensure that indexer mocking remains robust and up-to-date.
