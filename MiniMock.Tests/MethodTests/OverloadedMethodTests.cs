namespace MiniMock.Tests.MethodTests;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;
using static MiniMock.Tests.MethodTests.InheritanceTests;

[Mock<IOverloadedMethods>]
public class OverloadedMethodTests(ITestOutputHelper output)
{
    public interface IOverloadedMethods
    {
        int OverloadedMethod();
        string OverloadedMethod(string name);
        string OverloadedMethod(string name, int value);
        string OverloadedMethod(int value, string name);
    }

    [Fact]
    public void OverloadedMethod_WhenNotInitialized_AllOverloadsShouldThrowException()
    {
        // Arrange
        var sut = Mock.IOverloadedMethods();

        // Act

        // Assert
        Assert.Throws<InvalidOperationException>(() => sut.OverloadedMethod());
        Assert.Throws<InvalidOperationException>(() => sut.OverloadedMethod("name"));
        Assert.Throws<InvalidOperationException>(() => sut.OverloadedMethod("name", 10));
        Assert.Throws<InvalidOperationException>(() => sut.OverloadedMethod(10, "name"));
    }

    [Fact]
    public void OverloadedMethod_WhenMockNotInitialized_ShouldThrowException()
    {
        // Arrange
        var sut = Mock.IOverloadedMethods();

        // Act
        var actual = Assert.Throws<InvalidOperationException>(() => sut.OverloadedMethod());

        // Assert
        Assert.NotNull(actual);
        output.WriteLine(actual.Message);
        Assert.Contains("OverloadedMethod", actual.Message);
        Assert.Contains("OverloadedMethod", actual.Source);
    }

    [Fact]
    public void OverloadedMethod_WhenMockInitializedWithNoParameters_ShouldCallMethod()
    {
        // Arrange
        var isCalled = false;
        var sut = Mock.IOverloadedMethods(mock => mock.OverloadedMethod(() =>
        {
            isCalled = true;
            return 10;
        }));

        // Act
        var actual = sut.OverloadedMethod();

        // Assert
        Assert.True(isCalled, "Should be true when the mock is called");
        Assert.Equal(10, actual);
    }

    [Fact]
    public void OverloadedMethod_WhenMockInitializedWithOneParameter_ShouldCallMethod()
    {
        // Arrange
        var actual = "";
        var sut = Mock.IOverloadedMethods(mock => mock.OverloadedMethod(value => actual = value));

        // Act
        sut.OverloadedMethod("Whats in a name");

        // Assert
        Assert.Equal("Whats in a name", actual);
    }

    [Fact]
    public void OverloadedMethod_WhenMockInitializedWithTwoParameters_ShouldCallMethod()
    {
        // Arrange
        var sut = Mock.IOverloadedMethods(mock => mock.OverloadedMethod((string name, int value) => $"{name} {value}"));

        // Act
        var actual = sut.OverloadedMethod("Whats in a name", 10);

        // Assert
        Assert.Equal("Whats in a name 10", actual);
    }
}
public class InheritanceTests(ITestOutputHelper testOutputHelper)
{
    public interface IBaseWithMethods
    {
        bool method1();
        void method2();
        bool method3(string name);
        bool method4(out string name);
        void method5(string name);
    }

    public interface IDerivedWithMethods : IBaseWithMethods
    {
        bool method1();
        void method2();
        bool method3(string name);
        bool method4(out string name);
        void method5(string name);
    }

    [Fact]
    [Mock<IDerivedWithMethods>]
    public void MethodInheritanceTests()
    {

    }

    public interface IBaseWithProperties
    {
        string Name1 { get; set; }
        string Name2 { set; }
        string Name3 { get; }
    }

    public interface IDerivedWithProperties : IBaseWithProperties
    {
        string Name1 { get; set; }
        string Name2 { set; }
        string Name3 { get; }
    }


    [Fact]
    [Mock<IDerivedWithProperties>]
    public void PropertyInheritanceTests()
    {

    }


    public interface IBaseWithEvent
    {
        event EventHandler Event1;
    }

    public interface IDerivedWithEvent : IBaseWithEvent
    {
        event EventHandler Event1;
    }


    //[Fact]
    //[Mock<IDerivedWithEvent>]
    //public void EventInheritanceTests()
    //{

    //}

}

public class Inheritance : IDerivedWithEvent
{    private EventHandler? e_1;
    event EventHandler? IBaseWithEvent.Event1
    {
        add => this.e_1 += value;
        remove => this.e_1 -= value;
    }



    event EventHandler? IDerivedWithEvent.Event1
    {
        add => throw new NotImplementedException();
        remove => throw new NotImplementedException();
    }
}


public class evt : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}


    internal class INotifyPropertyChangedMock : System.ComponentModel.INotifyPropertyChanged
    {
        private INotifyPropertyChangedMock(System.Action<Config>? config = null) {
            var result = new Config(this);
            config = config ?? new System.Action<Config>(t => { });
            config.Invoke(result);
            _MockConfig = result;
        }
        
        public static System.ComponentModel.INotifyPropertyChanged Create(System.Action<Config>? config = null) => new INotifyPropertyChangedMock(config);
        
        internal Config _MockConfig { get; set; }
        
        public partial class Config
        {
            private readonly INotifyPropertyChangedMock target;
        
            public Config(INotifyPropertyChangedMock target)
            {
                this.target = target;
            }
        }
        
        #region System.ComponentModel.PropertyChangedEventHandler? PropertyChanged
        internal event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged_1;
         event System.ComponentModel.PropertyChangedEventHandler? System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        {
            add => this.PropertyChanged_1 += value;
            remove => this.PropertyChanged_1 -= value;
        }
        internal void trigger_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //PropertyChanged_1?.Invoke(this, args);
        }
        
        public partial class Config
        {
            public void PropertyChanged(out System.Action<System.ComponentModel.PropertyChangedEventArgs> trigger)
            {
                trigger = args => this.PropertyChanged(this.target, args);
            }
        }
        
        #endregion
        public partial class Config {
            public Config PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
                target.trigger_PropertyChanged(sender, e);
                return this;
            }

        }
    }
