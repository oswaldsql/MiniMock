# Approach to Logging and Debugging

## Context

Effective logging and debugging are essential for the development and maintenance of the MiniMock framework. Logging helps track the setup and usage of mocks, while debugging aids in identifying and resolving issues. To make debugging tests smoother, the `DebuggerStepThrough` attribute will be used to skip over the internal framework code during debugging sessions.

## Decision

We will implement logging to capture events related to the setup of mocks and calls to the mocks. Additionally, the `DebuggerStepThrough` attribute will be applied to relevant parts of the framework to streamline the debugging process. Logging functionality will be planned but not yet implemented.

## Status

Accepted

## Consequences

### Positive:

- **Traceability**: Logging provides a trace of mock setup and usage, aiding in troubleshooting and analysis.
- **Smooth Debugging**: The `DebuggerStepThrough` attribute helps developers focus on their test code rather than the internal workings of the framework.
- **Insight**: Logs offer insights into the behavior and interactions within the framework.

### Negative:

- **Implementation Effort**: Requires effort to implement and maintain logging functionality.
- **Performance Overhead**: Logging may introduce a slight performance overhead.

## Implementation Plan

1. **Logging**: Plan and design the logging mechanism to capture mock setup events and calls to mocks.
2. **DebuggerStepThrough**: Apply the `DebuggerStepThrough` attribute to relevant methods and classes to improve the debugging experience.

---

More ADRs can be found in the [docs/ADR](../README.md) directory.
