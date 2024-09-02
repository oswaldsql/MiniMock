# Workaround for using MiniMock with VB.net and F# test projects

MiniMock can be used with VB.Net and F# by using the following workarounds:

- Create a VB or f# test project
- Create a C# project referencing the MiniMock nuget package
- In the C# project specify InternalVisibleTo for the VB or F# project
```csharp
using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("<namespace of your test project>")]
```
- In your test project add a reference to the C# project
- Import the namespace MiniMock
