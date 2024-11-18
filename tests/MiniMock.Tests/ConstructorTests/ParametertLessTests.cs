namespace MiniMock.Tests.ConstructorTests;

using System.Reflection;

public class ParameterLessTests
{
    public class ParameterLessClass
    {
        public bool CtorIsCalled;
        public bool ImplicitCtorIsCalled { get; set; } = true;

        public ParameterLessClass()
        {
            this.CtorIsCalled = true;
        }
    }

    public interface IInterfaceWithoutCtor
    {
    }

    [Fact]
    [Mock<ParameterLessClass>]
    public void ParameterlessConstructorCanBeUsed()
    {
        // Arrange

        // ACT
        var sut = Mock.ParameterLessClass();

        // Assert
        Assert.IsAssignableFrom<ParameterLessClass>(sut);
        Assert.True(sut.CtorIsCalled);
        Assert.True(sut.ImplicitCtorIsCalled);
    }

    [Fact]
    [Mock<IInterfaceWithoutCtor>]
    public void InterfaceWithoutCtorCanBeUsed()
    {
        // Arrange

        // ACT
        var sut = Mock.IInterfaceWithoutCtor();

        // Assert
        Assert.IsAssignableFrom<IInterfaceWithoutCtor>(sut);
    }
}

public class AccessLevelTests
{
    internal class AccessLevelTestClass
    {
        public AccessLevelTestClass() { }

//        public AccessLevelTestClass(bool publicCtor) { }

        protected AccessLevelTestClass(string protectedCtor) { }

        internal AccessLevelTestClass(int internalCtor) { }

        private AccessLevelTestClass(double privateCtor) { }
    }

    [Fact]
    [Mock<AccessLevelTestClass>]
    public void OnlyPublicCtorCanBeUsed()
    {
        // Arrange

        // ACT
        var publicSut = new MockOf_AccessLevelTestClass();
        var actual = publicSut.GetType().GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

        //Mock.AccessLevelTestClass()

        // Assert
        Assert.Equal(2, actual.Length);
    }
}
