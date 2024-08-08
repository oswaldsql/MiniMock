namespace MiniMock.Tests;
using System;
using System.Threading.Tasks;

public class RepoDemo
{
    [Fact]
    [Mock<ICustomerRepository>]
    [Mock<IMailService>]
    public async Task CreateNewCustomerWillUseGuidFromRepository()
    {
        // arrange
        var newGuid = Guid.NewGuid();

        var mockRepository = Mock.ICustomerRepository(config => config.Create(newGuid));
        var mockMailService = Mock.IMailService(config => config.SendMail(Task.CompletedTask));

        var sut = new CustomersModel(mockRepository, mockMailService);

        // act
        var actual = await sut.Create("new user", "Some@email.com", CancellationToken.None);

        // assert
        Assert.Equal(newGuid, actual);
    }

    public class CustomersModel(ICustomerRepository repo, IMailService mailService)
    {
        public async Task<Guid> Create(string name, string email, CancellationToken token)
        {
            var id = await repo.Create(name, email, token);
            await mailService.SendMail(email, "Welcome", "Welcome as a customer", token);
            return id;
        }
    }

    public interface ICustomerRepository
    {
        public Task<Guid> Create(string name, string email, CancellationToken token);
        public Task<Customer> Read(Guid id, CancellationToken token);
        public Task Update(Customer updated, CancellationToken token);
        public Task Delete(Guid id, CancellationToken token);
    }

    public interface IMailService
    {
        public Task SendMail(string to, string subject, string body, CancellationToken token);
    }

    public record Customer(string Name, Guid Id);
}
