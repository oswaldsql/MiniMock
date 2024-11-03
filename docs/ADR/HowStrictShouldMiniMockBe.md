# Decision on How Strict the Framework Should Be

## Context

In the MiniMock framework, there is a need to determine the level of strictness when handling calls to members that are not explicitly mocked. A strict framework can help catch unintended calls and ensure that tests are precise and reliable. However, events are a special case and should not require listeners when they are called.

## Decision

The framework will be strict, throwing exceptions when a member that is not mocked is called. This approach ensures that all interactions are explicitly defined and helps catch unintended calls. However, events will be treated as a special case and will not require listeners when they are called.

## Consequences

### Positive:

- **Precision**: Ensures that all interactions with mocks are explicitly defined, leading to more precise and reliable tests.
- **Error Detection**: Helps catch unintended calls to members that are not mocked, reducing the risk of false positives in tests.
- **Consistency**: Provides a consistent approach to handling calls to non-mocked members.

### Negative:

- **Strictness**: The strict approach may require more effort to set up mocks, as all interactions must be explicitly defined.
- **Event Handling**: Special handling for events may introduce some complexity in the framework.
