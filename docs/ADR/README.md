# Architecture Decision Records (ADR)

Architecture Decision Records (ADRs) are documents that capture important architectural decisions made during the development of a MiniMock.

All the ADRs have been approved and are considered final decisions for the project.

## General ADRs [TL;DR](general/TLDR.md)

- [Do We __Really__ Need a New Mocking Framework?](general/DoWeNeedANewMockingFramework.md) - Deciding whether to build a new mocking framework.
- [Matching Target API in Mock API](general/MatchingTargetApi.md) - Ensures the mock API closely mirrors the target API.
- [How Strict Should MiniMock Be?](general/HowStrictShouldMiniMockBe.md) - Deciding how strict the framework should be.
- [No Built-in Assertion Feature](general/NoBuiltInAssertionFeature.md) - Users choose their preferred assertion framework.
- [No Dependencies to Shared Libraries](general/NoDependencies.md) - Avoid dependencies on shared libraries.
- [Documentation and Examples](general/DocumentationAndExamples.md) - Approach to documentation and examples for the framework.
- [Logging and Debugging](general/LoggingAndDebugging.md) - Approach to logging and debugging within the framework.

## Feature Specific ADRs [TL;DR](feature/TLDR.md)

- [Support for Classes and Interfaces](feature/SupportForClassesAndInterfaces.md) - How should classes and interfaces be supported.
- [Support for Constructors](feature/SupportForConstructors.md) - Decision on supporting constructors in the mocking framework.
- [Creating Mocks](feature/CreatingMocks.md) - Decision on how to create mocks in the framework.
- [Support for Methods](feature/SupportForMethods.md) - Decision on supporting methods in the mocking framework.
- [Support for Properties](feature/SupportForProperties.md) - Decision on supporting properties in the mocking framework.
- [Support for Events](feature/SupportForEvents.md) - Decision on supporting events in the mocking framework.
- [Support for Indexers](feature/SupportForIndexers.md) - Decision on supporting indexers in the mocking framework.

## Unsupported Features

- [Support for Extension Methods (WIP)](Unsupported/SupportForExtensionMethods.md) - Decision on supporting extension methods in the mocking framework.
- [Support for Sealed Classes (WIP)](Unsupported/SupportForSealedClasses.md) - Decision on supporting sealed classes in the mocking framework.
- [Support for Static Members (WIP)](Unsupported/SupportForStaticMembers.md) - Decision on supporting static members in the mocking framework.
