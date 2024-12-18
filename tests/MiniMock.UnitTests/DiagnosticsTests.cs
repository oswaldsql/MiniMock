﻿// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable MemberCanBePrivate.Global
namespace MiniMock.UnitTests;

using Microsoft.CodeAnalysis;

public class DiagnosticsTests(ITestOutputHelper testOutputHelper)
{
    public interface IRefProperty
    {
        ref string Name { get; }
    }

    [Fact]
    public void RefPropertyTests()
    {
        var source = Build.TestClass<IRefProperty>();

        var generate = new MiniMockGenerator().Generate(source);

        var diagnostics = generate.GetErrors();

        var actual = Assert.Single(diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, actual.Severity);
        Assert.Equal("MM0002", actual.Id);
        Assert.Equal("Ref property not supported for 'Name' in 'IRefProperty'", actual.GetMessage());

        Assert.Equal("Mock<MiniMock.UnitTests.DiagnosticsTests.IRefProperty>", actual.Location.GetCode());
    }

    public interface IRefMethod
    {
        ref string GetName();
    }

    [Fact]
    public void MethodWithRefReturnTypeShouldRaiseError()
    {
        var source = Build.TestClass<IRefMethod>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        var diagnostics = generate.GetErrors();

        var actual = Assert.Single(diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, actual.Severity);
        Assert.Equal("MM0003", actual.Id);
        Assert.Equal("Ref return type not supported for 'GetName' in 'IRefMethod'", actual.GetMessage());

        Assert.Equal("Mock<MiniMock.UnitTests.DiagnosticsTests.IRefMethod>", actual.Location.GetCode());
    }

    public sealed class SealedClass
    {
    }

    [Fact]
    public void MockingSealedClasesWillRaiseTheMm0006Error()
    {
        var source = Build.TestClass<SealedClass>();

        var generate = new MiniMockGenerator().Generate(source);

        testOutputHelper.DumpResult(generate);

        var diagnostics = generate.GetErrors();

        var actual = Assert.Single(diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, actual.Severity);
        Assert.Equal("MM0006", actual.Id);
        Assert.Equal("Cannot mock the sealed class 'SealedClass'", actual.GetMessage());

        Assert.Equal("Mock<MiniMock.UnitTests.DiagnosticsTests.SealedClass>", actual.Location.GetCode());
    }
}
