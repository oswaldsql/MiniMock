# Decision on How to Create Mocks

## Context

In the MiniMock framework, there is a need to establish a standardized approach for creating mocks. A consistent and efficient method for creating mocks will enhance the usability and maintainability of the framework.

## Decision

Mocks will be created using a mock factory. The mock factory will provide a centralized and consistent way to create and configure mocks, ensuring that all mocks are created following the same process and standards.

## Consequences

### Positive:

- **Consistency**: Ensures that all mocks are created in a consistent manner.
- **Centralization**: Provides a single point of control for mock creation, making it easier to manage and update.
- **Ease of Use**: Simplifies the process of creating mocks for developers.

### Negative:

- **Complexity**: Introduces an additional layer of abstraction, which may add some complexity to the framework.
- **Maintenance**: Requires ongoing maintenance to ensure the mock factory remains up-to-date with any changes to the framework.
