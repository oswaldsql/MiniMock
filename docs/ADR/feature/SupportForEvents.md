# Decision on Supporting Events

## Context

In the MiniMock framework, there is a need to determine the scope of support for mocking events. Events are a crucial part of the C# language, enabling the publisher-subscriber pattern. Supporting all types of events is essential to ensure the framework's flexibility and usability.

## Decision

The MiniMock framework will support mocking all types of events. This includes standard events, custom events, and events with different delegate types. This decision ensures that the framework can handle a wide range of scenarios involving event handling.

Events must be mockable using the following parameters:

- Raise : A method to raise the event, triggering all subscribed handlers.
- Trigger : A delegate as a out parameter to be used to trigger the event.
- Add/Remove : Delegates matching the event's add and remove signatures with functionality to be executed.

## Consequences

### Positive:

- **Comprehensive**: Ensures that the framework can handle all types of events, providing flexibility and comprehensive mocking capabilities.
- **Usability**: Enhances the framework's usability by allowing developers to mock events in various scenarios.

### Negative:

- **Complexity**: Supporting all types of events adds complexity to the framework.
- **Maintenance**: Requires ongoing maintenance to ensure that event mocking remains robust and up-to-date.

---

More ADRs can be found in the [docs/ADR](../README.md) directory.
