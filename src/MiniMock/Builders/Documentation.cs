namespace MiniMock.Builders;

internal static class Documentation
{
    internal const string CallBack = "Configures the mock to execute the specified action when the method matching the signature is called.";
    internal const string AcceptAny = "Configures the mock to accept any call to the method.";
    internal const string ThrowsException = "Configures the mock to throw the specified exception when the method is called.";
    internal const string SpecificValue = "Configures the mock to return the specific value.";
    internal const string SpecificValueList = "Configures the mock to return the consecutive values from an enumeration of values.";
    internal const string GenericTaskObject = "Configures the mock to return the specific value in a task object.";
    internal const string GenericTaskFunction = "Configures the mock to call the specified function and return the value wrapped in a task object when the method matching the signature is called.";
}
