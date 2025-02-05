# Decision on Supporting Methods

## Context

In the MiniMock framework, there is a need to determine the scope of support for mocking methods. Supporting all standard ways of creating methods is essential to ensure the framework's 
flexibility and usability. 

The following type of methods must be supported
- __Asynchronous__ with Task<>, Task, CancellationToken  
- __Overloaded__ methods in a way that keeps the required effort to setup the mock to a minimum.
- __Generic__ Including the 'where' __constraints__.
- __Out__ and __ref__ attributes on the method parameters.

## Decision

The MiniMock framework will support all standard ways of creating methods. This includes instance methods, static methods, virtual methods, and abstract methods. However, support for methods returning `ref` values will require additional work and will be addressed in future updates. See [issue #5](https://github.com/oswaldsql/MiniMock/issues/5)

Methods must be mockable using the following parameters

- Call : A Delegate matching the method signature with functionality to be executed.
- Throw : An exception to be thrown when the method is called.
- Return : A value to be returned when the method is called.
- ReturnValues : A sequence of values to be returned when the method is called multiple times.
- () : Methods returning `void` can be mocked using an empty delegate.

if none of the above parameters are provided, calling the method must throw a InvalidOperationException with a message in the form "The method '__[method name]__' in '__[mocked class]__' is not explicitly mocked.".

__Asynchronous__ methods are supported. Helper methods are provided to simplify the testing of asynchronous methods. Overloads of the following helper methods are added

- Return : Allows for returning either a Task object or the object to be wrapped in the task object.
- ReturnValues : Allows for returning either a sequence of Task objects or a sequence of objects to be wrapped in task objects.
- () : Methods returning Task can also use the empty delegate.

__Overloaded__ methods can either be mocked explicitly by using `Call` or collectively using the following 

- Throw : An exception to be thrown when calling any of the overwritten methods.
- Return : A value to be returned when a method with that return type is called.
- ReturnValues : A sequence of values to be returned when a method with those return types is called multiple times.
- () : Methods returning `void` or `Task` can be mocked using an empty delegate.

Generic methods are supported. The generic type is passed as a type parameter to the 'call' labmda method.

## Consequences

### Positive:

- **Comprehensive**: Ensures that the framework can handle a wide variety of method types.
- **Flexibility**: Provides developers with the ability to mock different kinds of methods, enhancing the framework's usability.

### Negative:

- **Complexity**: Supporting all standard methods adds complexity to the framework.
- **Ref Values**: Current issues with methods returning `ref` values need to be resolved, which may require significant effort.

---

More ADRs can be found in the [docs/ADR](../README.md) directory.
