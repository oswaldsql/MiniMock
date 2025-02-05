# Decision on Accessibility

## Context

The accessibility rules that applies to a manual created mock should also apply. No magic reflection logic will be done to access otherwise inaccessible members. 

## Decision

As such the following will be accessible.

- **Virtual** and **Abstract** members must be supported. 
- **Protected** and **Public** must be supported. 

And the following will not.

- **Partial**, **Sealed** and None overridable members will not be supported.
- **Internal** and **Private** members will not be exposed. 

## Consequences

### Positive:

- **Predictable**: Only exposes the parts of a mock that would normally be exposed.

### Negative:

- **Limitation**: Will not support advanced use cases.  

---

More ADRs can be found in the [docs/ADR](../README.md) directory.
