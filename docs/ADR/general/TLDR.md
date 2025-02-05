# TL;DR

## General concerns

### Need for a New Mocking Framework
- **Context**: Existing frameworks are complex and have external dependencies.
- **Decision**: Develop MiniMock with a minimalistic API and no external dependencies.
- **Consequences**: Simplifies usage and improves performance but may lack advanced features and require development effort.

### Supported Features
- **Context**: Which features should be supported, which should not and where do we set the bar.
- **Decision**: Common features that can be used in interfaces and classes should be supported but no 'magic' must be used.
- **Consequences**: Ensures predictability and consistency but limits the usage to what standard scenarios.

### How Strict the Framework Should Be
- **Context**: Determine strictness for handling calls to non-mocked members.
- **Decision**: Be strict, throwing exceptions for non-mocked members, with special handling for events.
- **Consequences**: Ensures precision and error detection but requires more setup effort and special event handling.

### Matching Target API in Mock API
- **Context**: Mock API should closely mirror the target API.
- **Decision**: Reflect the target API with minimal additional methods.
- **Consequences**: Ensures consistency and ease of use but limits flexibility and may require more effort for certain features.

### No Dependencies on Shared Libraries
- **Context**: Consideration of including dependencies on shared libraries.
- **Decision**: Do not include dependencies on shared libraries; implement functionality within the project or use static linking.
- **Consequences**: Provides control and stability but requires additional development effort and maintenance.


### Documentation and Examples
- **Context**: Effective documentation and examples are crucial for adoption and proper use.
- **Decision**: Create concise documentation and examples covering installation, configuration, usage, and common use cases.
- **Consequences**: Enhances clarity and adoption but requires effort and maintenance.


### Logging and Debugging
- **Context**: Effective logging and debugging are essential.
- **Decision**: Implement logging and use `DebuggerStepThrough` for smoother debugging.
- **Consequences**: Improves traceability and debugging but requires implementation effort and may introduce performance overhead.

### No Built-in Assertion Feature
- **Context**: Consideration of including a built-in assertion feature.
- **Decision**: Do not include a built-in assertion feature; rely on external assertion frameworks.
- **Consequences**: Provides flexibility and simplicity but requires users to manage additional dependencies.

