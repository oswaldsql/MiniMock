namespace MiniMock.Demo.BusinessLogic;

public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEMailService _emailService;

    public CustomerService(ICustomerRepository customerRepository, IEMailService emailService)
    {
        this._customerRepository = customerRepository;
        this._emailService = emailService;
    }

    public Task<Customer> GetCustomerById(int id, CancellationToken cancellationToken = default) =>
        this._customerRepository.GetCustomerById(id, cancellationToken);

    public async Task<int> CreateCustomer(string name, string email, CancellationToken cancellationToken = default)
    {
        await this._emailService.SendMail(email, "Welcome", $"Welcome to our service {name}");
        return await this._customerRepository.CreateCustomer(name, email, cancellationToken);
    }
}
