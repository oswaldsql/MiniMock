namespace MiniMock.Builders;

public class MethodSignature
{
    public MethodSignature(string signature, string code, string documentation, string seeCref = "")
    {
        this.Signature = signature;
        this.Code = code;
        this.Documentation = documentation;
        this.SeeCref = seeCref;
    }

    public string SeeCref { get; }
    public string Signature { get; }
    public string Code { get; }
    public string Documentation { get; }
}
