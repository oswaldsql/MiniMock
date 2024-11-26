// ReSharper disable EmptyConstructor
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
namespace MiniMock.Tests.ConstructorTests;

using System.Reflection;

public class AccessLevelTests
{
    internal class AccessLevelTestClass
    {
        static AccessLevelTestClass() {}

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
