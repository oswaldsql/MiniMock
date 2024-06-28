namespace MiniMock.Tests.GeneralTests;

public class InheritanceTests
{
    public interface IBase
    {
        /// <summary>
        /// This is the base method
        /// </summary>
        /// <returns>A base value.</returns>
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
        /// <summary>
        /// this is a new method
        /// </summary>
        /// <returns>a boolean</returns>
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
        Assert.True(sut.Method1());
        Assert.True(((IBase)sut).Method1());
    }

    [Fact]
    [Mock<IDerived>]
    public void FactMethodName2()
    {
        var sut = Mock.IDerived(config => config.Method6("Mocked"));

        Assert.Equal("Mocked", sut.Method6());
        Assert.Equal("Mocked", sut.Method6());
        Assert.Equal("Mocked", ((IBase)sut).Method6());
    }
}


internal class IBasicMethodsMock2 : MiniMock.Tests.MethodTests.BasicMethodTests.IBasicMethods
{
    private IBasicMethodsMock2(System.Action<Config>? config = null)
    {
        var result = new Config(this);
        config = config ?? new System.Action<Config>(t => { });
        config.Invoke(result);
        _MockConfig = result;
    }

    public static MiniMock.Tests.MethodTests.BasicMethodTests.IBasicMethods Create(System.Action<Config>? config = null) => new IBasicMethodsMock2(config);

    internal Config _MockConfig { get; set; }

    public partial class Config
    {
        private readonly IBasicMethodsMock2 target;

        public Config(IBasicMethodsMock2 target)
        {
            this.target = target;
        }
    }

    #region Method : void VoidWithoutParameters()
    void MiniMock.Tests.MethodTests.BasicMethodTests.IBasicMethods.VoidWithoutParameters()
    {
        this.On_VoidWithoutParameters_14.Invoke();
    }
    internal System.Action On_VoidWithoutParameters_14 { get; set; } = () => throw new System.InvalidOperationException("The method 'VoidWithoutParameters' in 'IBasicMethods' is not explicitly mocked.") { Source = "MiniMock.Tests.MethodTests.BasicMethodTests.IBasicMethods.VoidWithoutParameters()" };

    public partial class Config
    {
        private Config _VoidWithoutParameters_15(System.Action mock)
        {
            target.On_VoidWithoutParameters_14 = mock;
            return this;
        }
    }
    #endregion
    public partial class Config
    {
        public Config VoidWithoutParameters(System.Action call)
        {
            this._VoidWithoutParameters_15(call);
            return this;
        }

        public Config VoidWithoutParameters(System.Exception throws)
        {
            this._VoidWithoutParameters_15(() => throw throws);
            return this;
        }

        public Config VoidWithoutParameters()
        {
            this._VoidWithoutParameters_15(() => { });
            return this;
        }

    }

    #region Method : void VoidWithParameters(string name)
    void MiniMock.Tests.MethodTests.BasicMethodTests.IBasicMethods.VoidWithParameters(string name)
    {
        this.On_VoidWithParameters_15.Invoke(name);
    }
    internal System.Action<string> On_VoidWithParameters_15 { get; set; } = (string name) => throw new System.InvalidOperationException("The method 'VoidWithParameters' in 'IBasicMethods' is not explicitly mocked.") { Source = "MiniMock.Tests.MethodTests.BasicMethodTests.IBasicMethods.VoidWithParameters(string)" };

    public partial class Config
    {
        private Config _VoidWithParameters_16(System.Action<string> mock)
        {
            target.On_VoidWithParameters_15 = mock;
            return this;
        }
    }
    #endregion
    public partial class Config
    {
        public Config VoidWithParameters(System.Action<string> call)
        {
            this._VoidWithParameters_16(call);
            return this;
        }

        public Config VoidWithParameters(System.Exception throws)
        {
            this._VoidWithParameters_16((string name) => throw throws);
            return this;
        }

        /// <summary>
        /// Configures the mock to perform the specified action when the method <see cref="VoidWithParameters"/> is called without any parameters.
        /// </summary>
        /// <returns>The updated configuration.</returns>
        public Config VoidWithParameters()
        {
            this._VoidWithParameters_16((string name) => { });
            return this;
        }

    }

    #region Method : string ReturnWithoutParameters()
    string MiniMock.Tests.MethodTests.BasicMethodTests.IBasicMethods.ReturnWithoutParameters()
    {
        return this.On_ReturnWithoutParameters_16.Invoke();
    }
    internal System.Func<string> On_ReturnWithoutParameters_16 { get; set; } = () => throw new System.InvalidOperationException("The method 'ReturnWithoutParameters' in 'IBasicMethods' is not explicitly mocked.") { Source = "MiniMock.Tests.MethodTests.BasicMethodTests.IBasicMethods.ReturnWithoutParameters()" };

    public partial class Config
    {
        private Config _ReturnWithoutParameters_17(System.Func<string> call)
        {
            target.On_ReturnWithoutParameters_16 = call;
            return this;
        }
    }

    #endregion
    public partial class Config
    {
        public Config ReturnWithoutParameters(System.Func<string> call)
        {
            this._ReturnWithoutParameters_17(call);
            return this;
        }

        public Config ReturnWithoutParameters(System.Exception throws)
        {
            this._ReturnWithoutParameters_17(() => throw throws);
            return this;
        }

        public Config ReturnWithoutParameters(string returns)
        {
            this._ReturnWithoutParameters_17(() => returns);
            return this;
        }

    }

    #region Method : string ReturnWithParameters(string name)
    string MiniMock.Tests.MethodTests.BasicMethodTests.IBasicMethods.ReturnWithParameters(string name)
    {
        return this.On_ReturnWithParameters_17.Invoke(name);
    }
    internal System.Func<string, string> On_ReturnWithParameters_17 { get; set; } = (string _) => throw new System.InvalidOperationException("The method 'ReturnWithParameters' in 'IBasicMethods' is not explicitly mocked.") { Source = "MiniMock.Tests.MethodTests.BasicMethodTests.IBasicMethods.ReturnWithParameters(string)" };

    public partial class Config
    {
        private Config _ReturnWithParameters_18(System.Func<string, string> call)
        {
            target.On_ReturnWithParameters_17 = call;
            return this;
        }
    }

    #endregion
    public partial class Config
    {
        public Config ReturnWithParameters(System.Func<string, string> call)
        {
            this._ReturnWithParameters_18(call);
            return this;
        }

        public Config ReturnWithParameters(System.Exception throws)
        {
            this._ReturnWithParameters_18((string _) => throw throws);
            return this;
        }

        public Config ReturnWithParameters(string returns)
        {
            this._ReturnWithParameters_18((string _) => returns);
            return this;
        }

    }
}
