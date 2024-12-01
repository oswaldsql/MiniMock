using MiniMock;
using MiniMock.Tests.GeneralTests;

[assembly:Mock(MockFactoryName = "Mock")]

internal static class Mock2
{
    /// <summary>
    ///     Creates a mock object for <see cref="MiniMock.Tests.GeneralTests.AbstractClassesTest.AbstractClass"/>.
    /// </summary>
    ///     <param name="config">Optional configuration for the mock object.</param>
    /// <returns>The mock object for <see cref="MiniMock.Tests.GeneralTests.AbstractClassesTest.AbstractClass"/>.</returns>
    internal static MiniMock.Tests.GeneralTests.AbstractClassesTest.AbstractClass AbstractClass
        (System.Action<MiniMock.Tests.GeneralTests.MockOf_AbstractClass.Config>? config = null)
        => MiniMock.Tests.GeneralTests.MockOf_AbstractClass.Create(config);

    /// <summary>
    ///     Creates a mock object for <see cref="MiniMock.Tests.GeneralTests.AbstractClassesTest.AbstractClass"/>.
    /// </summary>
    ///     <param name="config">Optional configuration for the mock object.</param>
    /// <returns>The mock object for <see cref="MiniMock.Tests.GeneralTests.AbstractClassesTest.AbstractClass"/>.</returns>
    internal static MiniMock.Tests.GeneralTests.AbstractClassesTest.AbstractClass AbstractClass
        (out MiniMock.Tests.GeneralTests.MockOf_AbstractClass.Config config)
    {
        MiniMock.Tests.GeneralTests.MockOf_AbstractClass.Config tempConfig = null!;
        var result = MiniMock.Tests.GeneralTests.MockOf_AbstractClass.Create(c =>
        {
            tempConfig = c;
        });
        config = tempConfig;
        return result;
    }

    public static MiniMock.Tests.GeneralTests.AbstractClassesTest.AbstractClass Create(out MiniMock.Tests.GeneralTests.MockOf_AbstractClass.Config config)
    {
        var result = new MockOf_AbstractClass();
        result.GetConfig(out config);
        return result;
    }
}
