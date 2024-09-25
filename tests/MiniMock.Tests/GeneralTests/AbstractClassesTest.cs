namespace MiniMock.Tests.GeneralTests;

public class AbstractClassesTest
{
    public abstract class AbstractClass
    {
        public string NotAbstract { get; set; } = "";
        public abstract string Abstract { get; set; }
        public virtual string Virtual { get; set; } = "";

        public string NotAbstractGetOnly { get; } = "";
        public abstract string AbstractGetOnly { get;  }
        public virtual string VirtualGetOnly { get; } = "";

        public string NotAbstractSetOnly { set { } }
        public abstract string AbstractSetOnly { set; }
        public virtual string VirtualSetOnly { set{} }
    }

    [Fact]
    [Mock<AbstractClass>]
    public void AbstractClassPropertiesCanBeSet()
    {
        // Arrange
        var sut = Mock.AbstractClass(config => config
                .Abstract("Abstract")
                .AbstractGetOnly("AbstractGetOnly")
                .AbstractSetOnly("AbstractSetOnly")
                .Virtual("Virtual")
                .VirtualGetOnly("VirtualGetOnly")
                .VirtualSetOnly("VirtualSetOnly")
        );

        // ACT


        // Assert
        Assert.Equal("Abstract",sut.Abstract);
        Assert.Equal("AbstractGetOnly",sut.AbstractGetOnly);
        sut.AbstractSetOnly = "AbstractSetOnly";
        Assert.Equal("Virtual",sut.Virtual);
        Assert.Equal("VirtualGetOnly",sut.VirtualGetOnly);
        sut.VirtualSetOnly = "VirtualSetOnly";
    }

    [Fact]
    [Mock<AbstractClass>]
    public void AbstractClassPropertiesFunctionsCanBeSet()
    {
        // Arrange
        var sut = Mock.AbstractClass(config => config
            .Abstract(get: () => "Abstract", set:s => {})
            .AbstractGetOnly(get: () => "AbstractGetOnly")
            .AbstractSetOnly(set: s => { })
            .Virtual(get: () => "Virtual", set: s => {})
            .VirtualGetOnly(get:() => "VirtualGetOnly")
            .VirtualSetOnly(set : s => {})
        );

        // ACT


        // Assert
        Assert.Equal("Abstract",sut.Abstract);
        Assert.Equal("AbstractGetOnly",sut.AbstractGetOnly);
        sut.AbstractSetOnly = "AbstractSetOnly";
        Assert.Equal("Virtual",sut.Virtual);
        Assert.Equal("VirtualGetOnly",sut.VirtualGetOnly);
        sut.VirtualSetOnly = "VirtualSetOnly";
    }
}
