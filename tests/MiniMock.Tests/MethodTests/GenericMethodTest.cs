namespace MiniMock.Tests.MethodTests;

using Microsoft.Extensions.Logging;

public class GenericMethodTest
{
    public interface IGenericMethod
    {
        void ReturnGeneric(string value);
        T ReturnGeneric<T>(string value) where T : struct;
        void Register<T>(T value);
        IEnumerable<T> ReturnDerived<T>(string value) where T : struct;
        void ReturnVoid<T>(string value) where T : struct;
        T ReturnTwoGenerics<T, U>(string value) where T : struct where U : struct;
    }

    [Mock<IGenericMethod>]
    [Fact]
    public void SupportDifferentTypesOfGenericMethods()
    {
        // Arrange
        var sut = Mock.IGenericMethod(config =>
        {
            object Call(string value, Type t)
            {
                if(t == typeof(int))
                    return 10;
                return true;
            }

            config.ReturnGeneric(10).ReturnGeneric(call: Call);
        });

        // ACT
        var actualInt = sut.ReturnGeneric<int>("test");
        var actualBool = sut.ReturnGeneric<bool>("test");

        // Assert

    }

    [Fact]
    [Mock<Microsoft.Extensions.Logging.ILogger<string>>]
    public void SupportTheILogger()
    {
        // Arrange
        var sut = Mock.ILogger<string>(config => config.Log());

        // ACT

        // Assert

    }
}
