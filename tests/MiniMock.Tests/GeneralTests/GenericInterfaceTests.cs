namespace MiniMock.Tests.GeneralTests;

public class GenericInterfaceTests
{
    public interface IGeneric<TKey,TValue> where TKey : System.IComparable<TKey>
    {

    }

    [Fact]
    [Mock<IGeneric<string, string>>]
    public void METHOD()
    {
        // Arrange


        // ACT
        //Mock.IGeneric2<string>(config => { });

        // Assert

    }
}
