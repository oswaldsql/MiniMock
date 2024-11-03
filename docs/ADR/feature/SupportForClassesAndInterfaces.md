# Decision on Supporting Classes and Interfaces

## Context

In the MiniMock framework, there is a need to determine the scope of support for mocking different types of members. While interfaces are the primary focus due to their flexibility and common usage in dependency injection, there is also a need to support classes to cover a broader range of use cases.

## Decision

The MiniMock framework will primarily focus on supporting interfaces but will also include support for classes. This approach ensures that the framework can be used in a wide variety of scenarios, providing flexibility and comprehensive mocking capabilities.

## Consequences

### Positive:

- **Flexibility**: Supports a wide range of use cases by allowing both interfaces and classes to be mocked.
- **Comprehensive**: Provides a robust mocking solution that can handle various types of dependencies.
- **Usability**: Makes the framework more versatile and useful for developers.

### Negative:

- **Complexity**: Adding support for classes may introduce additional complexity in the framework.
- **Maintenance**: Requires ongoing maintenance to ensure both interfaces and classes are supported effectively.
f mocking scenarios.

---

More ADRs can be found in the [docs/ADR](../README.md) directory.
