namespace MiniMock.Demo;

using BusinessLogic;

[Mock<ICustomerRepository>]
[Mock<IEMailService>]
public class CustomerServiceTests
{
    [Fact]
    public async Task CustomersCanBeCreated()
    {
        var customerRepository = Mock.ICustomerRepository(config => config
            .CreateCustomer(1)
        );
        var emailService = Mock.IEMailService(config => config
            .SendMail()
        );

        var sut = new CustomerService(customerRepository, emailService);

        var actual = await sut.CreateCustomer("tom", "toms@diner.co.uk", CancellationToken.None);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task CustomersCanBeRead()
    {
        var sut = CreateCustomerServiceFactory(config => config.GetCustomerById(returns:new Customer(1,"Name","Email")), config => config.SendMail());

        var actual = await sut.GetCustomerById(1, CancellationToken.None);

        Assert.NotNull(actual);
    }

    private static CustomerService CreateCustomerServiceFactory(Action<ICustomerRepositoryMock.Config>? repoConfig = null, Action<IEMailServiceMock.Config>? emailConfig = null) =>
        new(Mock.ICustomerRepository(repoConfig), Mock.IEMailService(emailConfig));
}
