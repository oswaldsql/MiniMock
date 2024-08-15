namespace MiniMock.Tests.GeneralTests;

public class GenericInterfaceTests
{
    public interface IGeneric2<T>
    {

    }

    [Fact]
    [Mock<IGeneric2<string>>]
    public void METHOD()
    {
        // Arrange


        // ACT
        Mock.IGeneric2<string>(config => { });

        // Assert

    }
}
