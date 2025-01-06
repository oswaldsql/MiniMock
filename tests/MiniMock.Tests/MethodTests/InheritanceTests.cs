// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable MemberCanBePrivate.Global

namespace MiniMock.Tests.MethodTests;

public class InheritanceTests
{
    [Fact]
    [Mock<IDerived>]
    public void FactMethodName()
    {
        var sut = Mock.IDerived(config => config.ReturnBool(true));

        Assert.True(sut.ReturnBool());
        Assert.True(((IBase)sut).ReturnBool());

        Assert.Throws<InvalidOperationException>(() => ((IBase)sut).Method6());
    }

    [Fact]
    [Mock<IDerived>]
    public void FactMethodName2()
    {
        var sut = Mock.IDerived(config => config.Method6("Mocked"));

        Assert.Equal((string?)"Mocked", (string?)sut.Method6());
        Assert.Equal((string?)"Mocked", (string?)sut.Method6());
        Assert.Equal("Mocked", ((IBase)sut).Method6());
    }

    public interface IBase
    {
        bool ReturnBool();
        void Method2();
        bool Method3(string name);
        bool Method4(out string name);
        void Method5(string name);
        string Method6() => "base";
        bool Method7(ref string name);
    }

    public interface IDerived : IBase
    {
        new bool ReturnBool();
        new void Method2();
        new bool Method3(string name);
        new bool Method4(out string name);
        new void Method5(string name);
        new string Method6() => "Derived ";
        new bool Method7(ref string name);
    }
}
