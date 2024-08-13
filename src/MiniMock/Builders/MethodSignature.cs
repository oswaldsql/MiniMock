namespace MiniMock.Builders;

public class MethodSignature
{
    public MethodSignature(string signature, string code, string documentation)
    {
        this.Signature = signature;
        this.Code = code;
        this.Documentation = documentation;
    }

    public string Signature { get; }
    public string Code { get; }
    public string Documentation { get; }
}
