# Decision on Supporting Constructors

## Context

In the MiniMock framework, there is a need to determine the scope of support for mocking constructors. Initially, the framework will support only classes with parameterless constructors. This decision simplifies the initial implementation and allows for a gradual introduction of more complex constructor support.

## Decision

The MiniMock framework will initially support mocking only classes with parameterless constructors. A plan for adding support for classes with parameterized constructors is being developed and will be implemented in future iterations.

Since the main focus of the framework is to provide a simple and easy-to-use mocking solution for interfaces and classes, the decision to start with parameterless constructors aligns with this goal. See ADR [Support For Classes and Interfaces](SupportForClassesAndInterfaces.md) for more information.

## Consequences

### Positive:

- **Simplicity**: Simplifies the initial implementation by focusing on parameterless constructors.
- **Incremental Development**: Allows for a phased approach to adding more complex constructor support.
- **Usability**: Provides immediate value by supporting a common use case.

### Negative:

- **Limited Scope**: Initial support is limited to classes with parameterless constructors, which may not cover all use cases.
- **Future Work**: Additional effort will be required to implement support for parameterized constructors.

---

More ADRs can be found in the [docs/ADR](../README.md) directory.
