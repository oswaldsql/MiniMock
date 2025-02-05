# Unsupported features

## Context

In order to keep true to how mocking works some features will not be supported.

## Decision

**Extension methods** are not supported due to their nature. Extension methods are static methods that are called as if they were instance methods of the extended type.

**Sealed classes** are not supported since they can not be inherited from and such do not fit how the framework works.

**Static members** and **Static classes** are not supported due to the way the framework works.

## Consequences

### Positive:

- **No need for magic**: Mocking these features would require breaking the standard supported functionality. 

### Negative:

- **No magic**: Some functionality can be harder to test but if you need to test this maby you have some other issues.
