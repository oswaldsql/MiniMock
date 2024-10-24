namespace MiniMock.UnitTests;

using Microsoft.CodeAnalysis;

public class StaticInterfaceMembersTest(ITestOutputHelper testOutputHelper)
{
    public interface ISupportedStaticInterfaceMembers
    {
        static ISupportedStaticInterfaceMembers() { }

        static int StaticProperty { get; set; }
        static string StaticMethod() => "value";

        static event EventHandler? StaticEvent;
        static void DoStaticEvent() => StaticEvent?.Invoke(null, EventArgs.Empty);

        static virtual string Bar => "value"; // with implementation    }
    }

    [Fact]
    public void SomeStaticMembersAreSupported()
    {
        // Arrange
        var source = Build.TestClass<ISupportedStaticInterfaceMembers>();

        // ACT
        var generate = new MiniMockGenerator().Generate(source);

        // Assert
        testOutputHelper.DumpResult(generate);
        Assert.Empty(generate.GetErrors());
    }

    public interface IStaticAbstractInterfaceMembers
    {
        static abstract string AbstractProperty { get; set; }
        static abstract string AbstractMethod();
        static abstract event EventHandler StaticEvent;
    }

    [Fact]
    public void AbstractStaticMembersAreNotSupported()
    {
        // Arrange
        var source = Build.TestClass("MiniMock.UnitTests.StaticInterfaceMembersTest.IStaticAbstractInterfaceMembers");

        // ACT
        var generate = new MiniMockGenerator().Generate(source);

        // Assert
        //testOutputHelper.DumpResult(generate);
        Assert.Single(generate.diagnostics, t => t.Id == "CS8920");
        var actualAbstractPropertyError = Assert.Single(generate.diagnostics, t => t.Id == "MM0005");
        Assert.Equal(DiagnosticSeverity.Error, actualAbstractPropertyError.Severity);
        Assert.Equal("Static abstract members in interfaces or classes is not supported for 'AbstractMethod' in 'IStaticAbstractInterfaceMembers'",actualAbstractPropertyError.GetMessage());
    }
}
