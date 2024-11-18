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

    public interface IBookRepository
    {
        Task<Guid> AddBook(Book book, CancellationToken token);
        Book this[Guid index] { get; set; }
        int BookCount { get; set; }
        event EventHandler<Book> NewBookAdded;
    }

    [Fact]
    [Mock<IBookRepository>]
    public async Task BookCanBeCreated()
    {
        Action<Book> trigger = _ => { };

        var mockRepo = Mock.IBookRepository(config => config
            .AddBook(returns: Guid.NewGuid())
            .BookCount(value: 10)
            .Indexer(values: new Dictionary<Guid, Book>())
            .NewBookAdded(trigger: out trigger));

        var sut = new BookModel(mockRepo);
        var actual = await sut.AddBook(new Book());

        Assert.Equal("We now have 10 books", actual);
    }

    public class BookModel(IBookRepository repo)
    {
        public async Task<string> AddBook(Book book)
        {
            repo.NewBookAdded += ((sender, book1) => {repo.BookCount = repo.BookCount++;});
            var id = await repo.AddBook(book, CancellationToken.None);
            repo[id] = book;
            return $"We now have {repo.BookCount} books";
        }
    }

    public record Book();

    public interface IMailService
    {
        public Task SendMail(string to, string subject, string body, CancellationToken token);
    }

    public record Customer(string Name, Guid Id);
}


