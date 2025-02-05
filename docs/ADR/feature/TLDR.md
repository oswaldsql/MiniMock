# TL;DR

## Summary of Decisions

### Decision on Supporting Classes and Interfaces
- **Context**: Determine the scope of support for mocking different types of members.
- **Decision**: Primarily support interfaces but also include support for classes.
- **Consequences**: Flexible and comprehensive but adds complexity and requires ongoing maintenance.

### Decision on Accessibility
- **Context**: Determine the accessibility rules for mocking.
- **Decision**: Support virtual, abstract, protected, and public members. Do not support partial, sealed, non-overridable, internal, and private members.
- **Consequences**: Predictable behaviour but limited to standard use cases.

### How to Create Mocks
- **Context**: Establish a standardized approach for creating mocks.
- **Decision**: Use a mock factory for centralized and consistent mock creation but keep constructors accessible.
- **Consequences**: Ensures consistency and ease of use but adds complexity and maintenance requirements.

### Decision on Supporting Constructors
- **Context**: Determine the scope of support for mocking constructors.
- **Decision**: Support all constructors with the supported access level. If no constructor exists, create a parameterless constructor. Do not generate classes with only internal or private constructors.
- **Consequences**: Simplifies initial implementation but limits advanced usage scenarios.

### Decision on Supporting Methods
- **Context**: Determine the scope of support for mocking methods.
- **Decision**: Support all standard ways of creating methods, including instance, static, virtual, and abstract methods. Address support for methods returning `ref` values in future updates.
- **Consequences**: Comprehensive and flexible but adds complexity and requires future work for `ref` values.

### Decision on Supporting Properties
- **Context**: Determine the scope of support for mocking properties.
- **Decision**: Support mocking both read-only and read-write properties in classes and interfaces.
- **Consequences**: Comprehensive and flexible but adds complexity and requires ongoing maintenance.

### Decision on Supporting Indexers
- **Context**: Determine the scope of support for mocking indexers.
- **Decision**: Support mocking both read-only and read-write indexers.
- **Consequences**: Comprehensive and flexible but adds complexity and requires ongoing maintenance.

### Decision on Supporting Events
- **Context**: Determine the scope of support for mocking events.
- **Decision**: Support mocking all types of events, including standard, custom, and events with different delegate types.
- **Consequences**: Comprehensive and flexible but adds complexity and requires ongoing maintenance.
