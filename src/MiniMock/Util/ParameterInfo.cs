namespace MiniMock.Builders;

public class ParameterInfo(string type, string name, string outString, string function)
{
    public string Type { get; } = type;
    public string Name { get; } = name;
    public string OutString { get; } = outString;
    public string Function { get; } = function;
}
