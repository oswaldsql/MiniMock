namespace MiniMock.Tests;

public interface IRepository
{
    event EventHandler<string> OnCustomerCreated;

    public string Name { get;set;}

    public void CreateNewCustomer(string name);
    public string GetCustomerName();
    public void Drop();
    public Guid GetCustomerId(string name);
}

public interface IInd
{
    public int this[int index] { get; set; }

    public string DefaultImp()
    {
        return "Test";
    }
}

public class II : IInd
{
    string IInd.DefaultImp()
    {
        return "base.";
    }

    private System.Func<int, int> On_stringIndexGet { get; set; } = (_) => throw new System.NotImplementedException();
    private System.Action<int, int> On_stringIndexSet { get; set; } = (_, _) => throw new System.NotImplementedException();
    public int this[int index]
    {
        get => this.On_stringIndexGet(index);
        set => this.On_stringIndexSet(index, value);
    }
}

public interface IGeneric<T>
{
    //public T ReturnGenericType();
    //public void GenericParameter(T source);
}

public interface WithOverloads
{
    public void Method();
    public void Method(int i);
    public void Method(string s);
    public void Method(int i, string s);
    public void Method(string s, int i);
    public Task Method(string s, CancellationToken token);
    public Task<int> Method(int i, CancellationToken token);
}
