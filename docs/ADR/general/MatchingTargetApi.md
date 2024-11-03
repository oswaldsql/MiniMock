# Matching Target API in Mock API

When creating a mocking class, the mock API must closely mirror the API of the target being mocked. 
This ensures that the mock can be used as a drop-in replacement for the target, facilitating seamless testing and reducing the learning curve for developers.  

## Decision

The mock API should reflect the target API with minimal additional methods. 
The mock should only include methods that already exist in the target API, ensuring consistency and ease of use.  

## Status

Accepted

## Consequences

### Positive:
- Developers can use the mock without learning a new API.
- Tests are more readable and maintainable as they closely resemble the actual code.
- Reduces the risk of errors due to API mismatches.

### Negative:
- Limited flexibility in extending the mock API for advanced testing scenarios.
- May require more effort to implement certain mocking features without additional methods.

---

More ADRs can be found in the [docs/ADR](../README.md) directory.
