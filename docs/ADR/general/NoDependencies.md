# No Dependencies to Shared Libraries

## Context

In our project, there is a consideration to include dependencies on shared libraries. However, relying on shared libraries can introduce several challenges, including version conflicts, increased complexity, and reduced control over the project's dependencies.

## Decision

We will not include any dependencies on shared libraries in our project. Instead, we will aim to implement necessary functionality within the project itself using source generation.

## Consequences

### Positive:

- **Control**: Greater control over the project's dependencies and versions.
- **Simplicity**: Reduces the complexity of managing external dependencies.
- **Stability**: Minimizes the risk of version conflicts and compatibility issues.
- **Security**: Reduces the attack surface by limiting external dependencies.

### Negative:

- **Development Effort**: May require additional effort to implement functionality that would otherwise be provided by shared libraries.
- **Code Duplication**: Potential for code duplication if similar functionality is needed across multiple projects.
- **Maintenance**: Increased maintenance burden as all functionality must be maintained within the project.

---

More ADRs can be found in the [docs/ADR](../README.md) directory.
