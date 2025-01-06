// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable MemberCanBePrivate.Global

namespace MiniMock.Tests;

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

    [Fact]
    [Mock<IBookRepository>]
    public async Task BookCanBeCreated()
    {
        Action<Book> trigger = _ => { };

        Mock.IMailService(out _);

        var iBookRepository = Mock.IBookRepository(out var configIBookRepository);
        configIBookRepository
            .AddBook(Guid.NewGuid())
            .BookCount(10);

        var mockRepo = Mock.IBookRepository(config => config
            .AddBook(Guid.NewGuid())
            .BookCount(10)
            .Indexer(new Dictionary<Guid, Book>())
            .NewBookAdded(out trigger));

        var sut = new BookModel(mockRepo);
        var actual = await sut.AddBook(new Book());
        trigger(new Book());

        Assert.Equal("We now have 10 books", actual);
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
        Book this[Guid index] { get; set; }
        int BookCount { get; set; }
        Task<Guid> AddBook(Book book, CancellationToken token);
        event EventHandler<Book> NewBookAdded;
    }

    public class BookModel(IBookRepository repo)
    {
        public async Task<string> AddBook(Book book)
        {
            repo.NewBookAdded += (_, _) => { repo.BookCount = repo.BookCount++; };
            var id = await repo.AddBook(book, CancellationToken.None);
            repo[id] = book;
            return $"We now have {repo.BookCount} books";
        }
    }

    public record Book;

    public interface IMailService
    {
        public Task SendMail(string to, string subject, string body, CancellationToken token);
    }

    public record Customer(string Name, Guid Id);
}
