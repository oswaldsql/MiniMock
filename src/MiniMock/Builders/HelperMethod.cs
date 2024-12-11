namespace MiniMock.Builders;

public class HelperMethod(string signature, string code, string documentation, string seeCref = "")
{
    public string SeeCref { get; } = seeCref;
    public string Signature { get; } = signature;
    public string Code { get; } = code;
    public string Documentation { get; } = documentation;
}
