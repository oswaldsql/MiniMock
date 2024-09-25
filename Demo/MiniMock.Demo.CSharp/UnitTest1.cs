namespace MiniMock.Demo.CSharp;

using BusinessLogic;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var sut = MiniMock.Mock.ICustomerRepository(config => config.GetCustomerById(new Customer(10, "Tom", "toms@diner.com")));

    }
}
