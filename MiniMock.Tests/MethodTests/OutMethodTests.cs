namespace MiniMock.Tests.MethodTests;

public class OutMethodTests
{
    public interface IMethodWithOutArgument
    {
        bool OutWithReturn(string s, out int value);
        void OutWithVoid(string s, out int value);
        int OutWithRef(string s, ref int value);
    }

    [Fact]
    [Mock<IMethodWithOutArgument>]
    public void FactMethodName()
    {
        var sut = Mock.IMethodWithOutArgument(config =>
            config
                .OutWithReturn((string s, out int i) => int.TryParse(s, out i))
                .OutWithVoid((string s, out int i) => i = 10));

        sut.OutWithReturn("name", out var value);
        sut.OutWithVoid("10", out var value2);
    }
}
