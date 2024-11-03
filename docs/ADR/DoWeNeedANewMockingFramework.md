# Need for a New Mocking Framework

## Context

Existing mocking frameworks in the .NET ecosystem often come with clunky APIs and external dependencies that can complicate the development process. These frameworks may offer extensive features, but they can also introduce unnecessary complexity and bloat, making them less suitable for projects that prioritize simplicity and minimalism.

## Decision

We will develop a new mocking framework, MiniMock, that focuses on providing a minimalistic and straightforward API. This framework will avoid external dependencies to ensure ease of use and integration.

## Consequences

### Positive:

- **Simplicity**: A minimalistic API will make the framework easier to learn and use, reducing the learning curve for new developers.
- **No External Dependencies**: By avoiding external dependencies, the framework will be easier to integrate and maintain, reducing potential conflicts and bloat.
- **Performance**: A lightweight framework can offer better performance due to reduced overhead.
- **Control**: Greater control over the framework's features and behavior, ensuring it meets the specific needs of the project.

### Negative:

- **Feature Limitations**: The framework may lack some advanced features found in more comprehensive mocking frameworks.
- **Development Effort**: Additional effort will be required to develop and maintain the new framework.
- **Adoption**: Convincing developers to switch to a new framework may be challenging, especially if they are accustomed to existing solutions.

---

More ADRs can be found in the [docs/ADR](../ADR/README.md) directory.
