# Allowing Skipping Arguments in Mock Setup

## Context

In our mocking framework, we have the option to skip arguments when setting up mocks for methods. Although this feature has been found to be rarely used, it can provide significant flexibility and convenience in certain testing scenarios.

## Decision

We will allow the skipping of arguments when setting up mocks for methods. This will enable developers to focus on the relevant arguments and simplify the mock setup process when certain arguments are not needed for specific tests.

## Consequences

### Positive:
- **Flexibility**: Provides greater flexibility in setting up mocks, allowing developers to skip irrelevant arguments.
- **Convenience**: Simplifies the mock setup process, especially for methods with many parameters.
- **Focus**: Allows tests to focus on the relevant arguments, improving readability and maintainability.

### Negative:
- **Clarity**: May reduce the clarity of tests if arguments are skipped without proper documentation.
- **Consistency**: Could lead to inconsistent mock setups across the codebase if not used judiciously.
- **Error Potential**: Increases the risk of errors if skipped arguments are assumed incorrectly.~~~~

---

More ADRs can be found in the [docs/ADR](../ADR/README.md) directory.
