# Mocking Protected Methods

## Context

In our mocking framework, there is a consideration to support mocking protected methods. Protected methods are often used in base classes to provide functionality that is intended to be used by derived classes. Supporting the mocking of protected methods can enhance the flexibility and comprehensiveness of our testing framework.

## Decision

We will support the mocking of protected methods in our mocking framework. This will allow developers to create more thorough and flexible tests by enabling them to mock and verify the behavior of protected methods.

## Consequences

### Positive:

- **Flexibility**: Allows developers to mock and test protected methods, providing more comprehensive test coverage.
- **Thorough Testing**: Enables testing of internal logic that is encapsulated within protected methods.
- **Consistency**: Aligns with the ability to mock public methods, providing a consistent approach to mocking.

### Negative:

- **Complexity**: May introduce additional complexity in the mocking framework to handle protected methods.
- **Maintenance**: Requires ongoing maintenance to ensure that mocking protected methods continues to work correctly with future updates.
- **Potential Misuse**: Developers may misuse the feature by over-mocking, leading to tests that are too tightly coupled to implementation details.
