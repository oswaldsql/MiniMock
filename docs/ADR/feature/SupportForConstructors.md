# Decision on Supporting Constructors

## Context

In the MiniMock framework, there is a need to determine the scope of support for mocking constructors.

## Decision

All constructors with the supported access level should be accessible. If no constructor exists, a parameterless constructor is created.
A factory for each option should be created.

If only internal or private constructors exist, the class is not generated and a warning is registered.

Additionally, the framework should support the following:

- **Parameterized Constructors**: Allow mocking of constructors with parameters, providing flexibility for more complex scenarios.
- **Constructor Overloads**: Support multiple constructors with different parameter lists.
- **Dependency Injection**: Enable mocking of constructors that use dependency injection, ensuring compatibility with modern design patterns.

## Consequences

### Positive:

- **Simplicity**: Simplifies the initial implementation by focusing on parameterless constructors.
- **Flexibility**: Supporting parameterized constructors and overloads provides more flexibility for developers.
- **Compatibility**: Ensures compatibility with dependency injection, making the framework more versatile.

### Negative:

- **Complexity**: Adding support for parameterized constructors and overloads increases the complexity of the framework.
- **Maintenance**: Requires ongoing maintenance to ensure that constructor mocking remains robust and up-to-date.

---

More ADRs can be found in the [docs/ADR](../README.md) directory.
