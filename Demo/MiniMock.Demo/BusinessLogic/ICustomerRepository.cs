namespace MiniMock.Demo.BusinessLogic;

public interface ICustomerRepository
{
    Task<Customer> GetCustomerById(int id, CancellationToken cancellationToken = default);
    bool UserExists(string email);
    Task<int> CreateCustomer(string name, string email, CancellationToken cancellationToken = default);
}
