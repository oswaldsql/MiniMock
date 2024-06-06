namespace MiniMock.Tests;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;


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

internal class WithOverloadsMock : WithOverloads
{
    public WithOverloadsMock(System.Action<Config>? config = null)
    {
        var result = new Config(this);
        config = config ?? new System.Action<Config>(t => { });
        config.Invoke(result);
        _MockConfig = result;
    }
    private Config _MockConfig { get; set; }

    public partial class Config
    {
        private readonly WithOverloadsMock target;

        public Config(WithOverloadsMock target)
        {
            this.target = target;
        }
    }

    #region void Method()
    public partial class Config
    {
        internal System.Action On_Method_ { get; set; } = () => throw new System.NotImplementedException();
        public Config Method(System.Action mock)
        {
            this.On_Method_ = mock;
            return this;
        }
    }

    public void Method()
    {
        _MockConfig.On_Method_.Invoke();
    }
    #endregion


    #region void Method(int i)
    public partial class Config
    {
        internal System.Action<int> On_Method_int { get; set; } = (_) => throw new System.NotImplementedException();
        public Config Method(System.Action<int> mock)
        {
            this.On_Method_int = mock;
            return this;
        }

    }

    public void Method(int i)
    {
        _MockConfig.On_Method_int.Invoke(i);
    }
    #endregion


    #region void Method(string s)
    public partial class Config
    {
        internal System.Action<string> On_Method_string { get; set; } = (_) => throw new System.NotImplementedException();
        public Config Method(System.Action<string> mock)
        {
            this.On_Method_string = mock;
            return this;
        }

    }

    public void Method(string s)
    {
        _MockConfig.On_Method_string.Invoke(s);
    }
    #endregion


    #region void Method(int i, string s)
    public partial class Config
    {
        internal System.Action<int, string> On_Method_int__string { get; set; } = (_, _) => throw new System.NotImplementedException();
        public Config Method(System.Action<int, string> mock)
        {
            this.On_Method_int__string = mock;
            return this;
        }

    }

    public void Method(int i, string s)
    {
        _MockConfig.On_Method_int__string.Invoke(i, s);
    }
    #endregion


    #region void Method(string s, int i)
    public partial class Config
    {
        internal System.Action<string, int> On_Method_string__int { get; set; } = (_, _) => throw new System.NotImplementedException();
        public Config Method(System.Action<string, int> mock)
        {
            this.On_Method_string__int = mock;
            return this;
        }

    }

    public void Method(string s, int i)
    {
        _MockConfig.On_Method_string__int.Invoke(s, i);
    }
    #endregion


    #region System.Threading.Tasks.Task Method(string s, System.Threading.CancellationToken token)
    public partial class Config
    {
        internal System.Func<string, System.Threading.CancellationToken, System.Threading.Tasks.Task> On_Method_string__System_Threading_CancellationToken__ { get; set; } = (_, _) => throw new System.NotImplementedException();
        public Config Method(System.Func<string, System.Threading.CancellationToken, System.Threading.Tasks.Task> call)
        {
            this.On_Method_string__System_Threading_CancellationToken__ = call;
            return this;
        }

    }


    public System.Threading.Tasks.Task Method(string s, System.Threading.CancellationToken token)
    {
        return _MockConfig.On_Method_string__System_Threading_CancellationToken__.Invoke(s, token);
    }
    #endregion


    #region System.Threading.Tasks.Task<int> Method(int i, System.Threading.CancellationToken token)
    public partial class Config
    {
        internal System.Func<int, System.Threading.CancellationToken, System.Threading.Tasks.Task<int>> On_Method_int__System_Threading_CancellationToken__ { get; set; } = (_, _) => throw new System.NotImplementedException();
        public Config Method(System.Func<int, System.Threading.CancellationToken, System.Threading.Tasks.Task<int>> call)
        {
            this.On_Method_int__System_Threading_CancellationToken__ = call;
            return this;
        }
        public Config Method(System.Func<int, System.Threading.CancellationToken, int> call) 
        {
            this.On_Method_int__System_Threading_CancellationToken__ = (i, token) => Task.FromResult(call(i, token));
            return this;
        }

    }


    public System.Threading.Tasks.Task<int> Method(int i, System.Threading.CancellationToken token)
    {
        return _MockConfig.On_Method_int__System_Threading_CancellationToken__.Invoke(i, token);
    }
    #endregion

}

public interface IMethodRepository
{
    Task<Guid> AddG(string name);
    //Task Add(string name);

    //void Drop();
    //void DropThis(string name);
    //string ReturnValue();
    //Guid CreateNewCustomer(string name);

    //(string name, int age) GetCustomerInfo(string name);

    //void Unlike() { }

    //static string StaticMethod() => "StaticMethod";

    //public string DefaultImp()
    //{
    //    return "Test";
    //}
}
internal class IMethodRepositoryMock : IMethodRepository
{
    public IMethodRepositoryMock(System.Action<Config>? config = null)
    {
        var result = new Config(this);
        config = config ?? new System.Action<Config>(t => { });
        config.Invoke(result);
        _MockConfig = result;
    }
    private Config _MockConfig { get; set; }

    public partial class Config
    {
        private readonly IMethodRepositoryMock target;

        public Config(IMethodRepositoryMock target)
        {
            this.target = target;
        }
    }

    #region System.Threading.Tasks.Task<System.Guid> AddG(string name)
    public partial class Config
    {
        internal System.Func<string, System.Threading.Tasks.Task<System.Guid>> On_AddG_string__ { get; set; } = (_) => throw new System.NotImplementedException();
        private Config Set_AddG(System.Func<string, System.Threading.Tasks.Task<System.Guid>> call)
        {
            this.On_AddG_string__ = call;
            return this;
        }
        public Config AddG(System.Func<string, System.Threading.Tasks.Task<System.Guid>> call) => this.Set_AddG(call);
        public Config AddG(System.Exception throws) => this.Set_AddG((_) => throw throws);
        public Config AddG(System.Threading.Tasks.Task<System.Guid> returns) => this.Set_AddG((_) => returns);
        public Config AddG(System.Guid returns) => this.Set_AddG((_) => System.Threading.Tasks.Task.FromResult(returns));
        public Config AddG2(System.Func<string, System.Guid> call) => this.Set_AddG(s => Task.FromResult(call(s)));
    }


    public System.Threading.Tasks.Task<System.Guid> AddG(string name)
    {
        return _MockConfig.On_AddG_string__.Invoke(name);
    }
    #endregion

}
