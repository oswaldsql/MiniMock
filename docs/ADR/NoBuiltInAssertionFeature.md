# No Built-in Assertion Feature

## Context

In our mocking framework, there is a consideration to include a built-in assertion feature. However, there are numerous assertion frameworks available, each with its own strengths and user base. Including a built-in assertion feature may lead to redundancy and limit the flexibility for users to choose their preferred assertion framework.

## Decision

We will not include a built-in assertion feature in our mocking framework. Instead, we will rely on users to choose and use their preferred assertion framework.

## Consequences

### Positive:

- **Flexibility**: Users can choose the assertion framework that best fits their needs and preferences.
- **Simplicity**: Reduces the complexity of the mocking framework by not including redundant features.
- **Interoperability**: Ensures compatibility with a wide range of existing assertion frameworks.

### Negative:

- **Learning Curve**: Users may need to learn and integrate a separate assertion framework if they are not already familiar with one.
- **Dependency Management**: Users will need to manage additional dependencies for their chosen assertion framework.

---

More ADRs can be found in the [docs/ADR](../ADR/README.md) directory.
