namespace MiniMock.Tests.GeneralTests;

public class InheritanceTests
{
    public interface IBase
    {
        bool Method1();
        void Method2();
        bool Method3(string name);
        bool Method4(out string name);
        void Method5(string name);
        string Method6() => "base";
        bool Method7(ref string name);
    }

    public interface IDerived : IBase
    {
        new bool Method1();
        new void Method2();
        new bool Method3(string name);
        new bool Method4(out string name);
        new void Method5(string name);
        new string Method6() => "Derived ";
        new bool Method7(ref string name);
    }

    [Fact]
    [Mock<IDerived>]
    public void FactMethodName()
    {
        var sut = Mock.IDerived(config => config.Method1(true));

        Assert.True(sut.Method1());
        Assert.True(((IDerived)sut).Method1());
        Assert.True(((IBase)sut).Method1());
    }

    [Fact]
    [Mock<IDerived>]
    public void FactMethodName2()
    {
        var sut = Mock.IDerived(config => config.Method6("Mocked"));

        Assert.Equal("Mocked", sut.Method6());
        Assert.Equal("Mocked", ((IDerived)sut).Method6());
        Assert.Equal("Mocked", ((IBase)sut).Method6());
    }
}
