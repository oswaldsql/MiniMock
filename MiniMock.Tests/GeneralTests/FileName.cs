namespace MiniMock.Tests.GeneralTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public class InheritanceTests
{
    public interface IBase
    {
        bool method1();
        void method2();
        bool method3(string name);
        bool method4(out string name);
        void method5(string name);
        string method6() { return "base"; }
        bool method7(ref string name);
    }

    public interface IDerived : IBase
    {
        bool method1();
        void method2();
        bool method3(string name);
        bool method4(out string name);
        void method5(string name);
        string method6() { return "Derived "; }
        bool method7(ref string name);
    }

    [Fact]
    [Mock<IDerived>]
    public void FactMethodName()
    {
        var sut = Mock.IDerived(config => config.method1(true));

        Assert.True(sut.method1());
        Assert.True(((IDerived)sut).method1());
        Assert.True(((IBase)sut).method1());
    }

    [Fact]
    [Mock<IDerived>]
    public void FactMethodName2()
    {
        var sut = Mock.IDerived(config => config.method6("Mocked"));

        Assert.Equal("Mocked", sut.method6());
        Assert.Equal("Mocked", ((IDerived)sut).method6());
        Assert.Equal("Mocked", ((IBase)sut).method6());
    }

}
