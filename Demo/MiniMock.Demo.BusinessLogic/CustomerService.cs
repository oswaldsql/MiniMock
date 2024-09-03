namespace MiniMock.Demo.BusinessLogic;

public class CustomerService(ICustomerRepository customerRepository, IEMailService emailService)
{
    public Task<Customer> GetCustomerById(int id, CancellationToken cancellationToken = default) =>
        customerRepository.GetCustomerById(id, cancellationToken);

    public async Task<int> CreateCustomer(string name, string email, CancellationToken cancellationToken = default)
    {
        await emailService.SendMail(email, "Welcome", $"Welcome to our service {name}");
        return await customerRepository.CreateCustomer(name, email, cancellationToken);
    }
}
