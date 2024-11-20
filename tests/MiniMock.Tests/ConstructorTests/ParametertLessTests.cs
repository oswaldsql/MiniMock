namespace MiniMock.Tests.ConstructorTests;

using System.Reflection;

public class ParameterLessTests
{
    public class ParameterLessClass
    {
        public ParameterLessClass() => this.CtorIsCalled = true;

        public bool CtorIsCalled;

        public bool ImplicitCtorIsCalled { get; set; } = true;
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
         /// <summary>
        /// test
        /// </summary>
        /// <param name="publicCtor">marker</param>
        public AccessLevelTestClass(bool publicCtor) { }

        protected AccessLevelTestClass(string protectedCtor) { }

        internal AccessLevelTestClass(int internalCtor) { }

        private AccessLevelTestClass(double privateCtor) { }
    }

    [Fact]
    [Mock<AccessLevelTestClass>]
    public void OnlyPublicAndProtectedCtorAreExposed()
    {
        // Arrange

        // ACT
        var actual = typeof(MockOf_AccessLevelTestClass).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

        // Assert
        Assert.Equal(2, actual.Length);
    }
}
