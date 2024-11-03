# Decision on Supporting Properties

## Context

In the MiniMock framework, there is a need to determine the scope of support for mocking properties. Properties are a fundamental part of C# classes and interfaces, and supporting them is essential for a comprehensive mocking framework.

## Decision

The MiniMock framework will support mocking both read-only and read-write properties. This includes properties in classes and interfaces, ensuring that the framework can handle a wide range of scenarios.

Properties must be mockable using the following parameters:

- Get/set : Delegates matching the property's getter and setter signature with functionality to be executed.
- Value : A value to be returned when the property is accessed.

Get-only and set-only properties must only allow mocking of the corresponding getter or setter.

if none of the above parameters are provided, Getting of setting the property must throw a InvalidOperationException with a message in the form "The property '__[property name]__' in '__[mocked class]__' is not explicitly mocked.".


## Consequences

### Positive:

- **Comprehensive**: Ensures that the framework can handle properties in both classes and interfaces.
- **Flexibility**: Provides developers with the ability to mock different kinds of properties, enhancing the framework's usability.

### Negative:

- **Complexity**: Supporting both read-only and read-write properties adds complexity to the framework.
- **Maintenance**: Requires ongoing maintenance to ensure that property mocking remains robust and up-to-date.

This decision ensures that the MiniMock framework supports a wide range of property types, providing flexibility and comprehensive mocking capabilities.
