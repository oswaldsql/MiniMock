namespace MiniMock.Tests.MethodTests;

using System.ComponentModel;
using System.Runtime.CompilerServices;

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
        Assert.True(((IBase)sut).Method1());

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

    public class TestNotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    [Fact]
    [Mock<TestNotifyPropertyChanged>]
    public void METHOD()
    {
        // Arrange


        // ACT

        // Assert

    }
}
