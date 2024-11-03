# Decision on Supporting Methods

## Context

In the MiniMock framework, there is a need to determine the scope of support for mocking methods. Supporting all standard ways of creating methods is essential to ensure the framework's flexibility and usability. However, there are currently issues with supporting methods that return `ref` values, which need to be addressed.

## Decision

The MiniMock framework will support all standard ways of creating methods. This includes instance methods, static methods, virtual methods, and abstract methods. However, support for methods returning `ref` values will require additional work and will be addressed in future updates. See [issue #5](https://github.com/oswaldsql/MiniMock/issues/5)

Methods must be mockable using the following parameters

- Call : A Delegate matching the method signature with functionality to be executed.
- Throw : An exception to be thrown when the method is called.
- Return : A value to be returned when the method is called.
- ReturnValues : A sequence of values to be returned when the method is called multiple times.
- () : Methods returning `void` can be mocked using an empty delegate.

if none of the above parameters are provided, calling the method must throw a InvalidOperationException with a message in the form "The method '__[method name]__' in '__[mocked class]__' is not explicitly mocked.".

Special cases like [Overloads], [Generic Methods] and [Async Methods] have dedicated ADRs.

## Consequences

### Positive:

- **Comprehensive**: Ensures that the framework can handle a wide variety of method types.
- **Flexibility**: Provides developers with the ability to mock different kinds of methods, enhancing the framework's usability.

### Negative:

- **Complexity**: Supporting all standard methods adds complexity to the framework.
- **Ref Values**: Current issues with methods returning `ref` values need to be resolved, which may require significant effort.
