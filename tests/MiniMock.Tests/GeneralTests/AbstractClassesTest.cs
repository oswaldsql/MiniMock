namespace MiniMock.Tests.GeneralTests;

using Xunit.Sdk;

public class AbstractClassesTest
{
    public abstract class AbstractClass
    {
        public string NotAbstract { get; set; } = "";
        public abstract string Abstract { get; set; }
        public virtual string Virtual { get; set; } = "";

        public string NotAbstractGetOnly => "";
        public abstract string AbstractGetOnly { get;  }
        public virtual string VirtualGetOnly => throw new TestClassException("this should never be called");

        internal string? notAbstractSetOnly;
        public string NotAbstractSetOnly { set => this.notAbstractSetOnly = value; }
        public abstract string AbstractSetOnly { set; }
        public virtual string VirtualSetOnly { set => throw new TestClassException("this should never be called"); }
    }

    [Fact]
    [Mock<AbstractClass>]
    public void AbstractClassPropertiesCanBeSet()
    {
        // Arrange
        string? actualVirtualSetOnly = null;

        var sut = Mock.AbstractClass(config => config
                .Abstract("Abstract")
                .AbstractGetOnly("AbstractGetOnly")
                .AbstractSetOnly("AbstractSetOnly")
                .Virtual("Virtual")
                .VirtualGetOnly("VirtualGetOnly")
                .VirtualSetOnly(set: s => actualVirtualSetOnly = s)
        );

        // ACT


        // Assert
        Assert.Equal("Abstract",sut.Abstract);
        Assert.Equal("AbstractGetOnly",sut.AbstractGetOnly);
        sut.AbstractSetOnly = "AbstractSetOnly";
        Assert.Equal("Virtual",sut.Virtual);
        Assert.Equal("VirtualGetOnly",sut.VirtualGetOnly);
        sut.VirtualSetOnly = "VirtualSetOnly";
        Assert.Equal("VirtualSetOnly", actualVirtualSetOnly);
    }

    [Fact]
    [Mock<AbstractClass>]
    public void NoneAbstractClassPropertiesAreParsedThroughToTheBaseClass()
    {
        // Arrange
        var sut = Mock.AbstractClass();

        // ACT
        sut.NotAbstract = "test";
        var sutNotAbstractGetOnly = sut.NotAbstractGetOnly;
        sut.NotAbstractSetOnly = "test";

        // Assert
        Assert.Equal("test", sut.NotAbstract);
        Assert.Equal("",sutNotAbstractGetOnly);
    }

    [Fact]
    [Mock<AbstractClass>]
    public void AbstractClassPropertiesFunctionsCanBeSet()
    {
        string? actualAbstract = null;
        string? actualAbstractSetOnly = null;
        string? actualVirtual = null;
        string? actualVirtualSetOnly = null;
        // Arrange
        var sut = Mock.AbstractClass(config => config
            .Abstract(get: () => "Abstract", set:s => actualAbstract = s)
            .AbstractGetOnly(get: () => "AbstractGetOnly")
            .AbstractSetOnly(set: s => actualAbstractSetOnly = s)
            .Virtual(get: () => "Virtual", set: s => actualVirtual = s)
            .VirtualGetOnly(get:() => "VirtualGetOnly")
            .VirtualSetOnly(set : s => actualVirtualSetOnly = s)
        );

        // ACT
        sut.Abstract = "setAbstract";
        sut.AbstractSetOnly = "setAbstractSetOnly";
        sut.Virtual = "setVirtual";
        sut.VirtualSetOnly = "setVirtualSetOnly";

        // Assert
        Assert.Equal("setAbstract",actualAbstract);
        Assert.Equal("setAbstractSetOnly",actualAbstractSetOnly);
        Assert.Equal("setVirtual",actualVirtual);
        Assert.Equal("setVirtualSetOnly",actualVirtualSetOnly);


        Assert.Equal("Abstract",sut.Abstract);
        Assert.Equal("AbstractGetOnly",sut.AbstractGetOnly);
        sut.AbstractSetOnly = "AbstractSetOnly";
        Assert.Equal("Virtual",sut.Virtual);
        Assert.Equal("VirtualGetOnly",sut.VirtualGetOnly);
        sut.VirtualSetOnly = "VirtualSetOnly";
    }
}
