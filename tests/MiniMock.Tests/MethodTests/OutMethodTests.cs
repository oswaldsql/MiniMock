// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable MemberCanBePrivate.Global

namespace MiniMock.Tests.MethodTests;

public class OutMethodTests
{
    [Fact]
    [Mock<IMethodWithOutArgument>]
    public void OutParameterWithReturnValueShouldWork()
    {
        var sut = Mock.IMethodWithOutArgument(config =>
            config.OutWithReturn((string s, out int i) => int.TryParse(s, out i))
        );

        var result = sut.OutWithReturn("10", out var actual);

        Assert.Equal(10, actual);
        Assert.True(result);
    }


    [Fact]
    [Mock<IMethodWithOutArgument>]
    public void OutParameterWithoutReturnValueShouldWork()
    {
        var sut = Mock.IMethodWithOutArgument(config =>
            config
                .OutWithVoid((string s, out int i) => i = int.Parse(s))
        );

        sut.OutWithVoid("10", out var actual);

        Assert.Equal(10, actual);
    }

    [Fact]
    [Mock<IMethodWithOutArgument>]
    public void RefParameterWithReturnValueShouldWork()
    {
        var sut = Mock.IMethodWithOutArgument(config =>
            config
                .OutWithRef((string s, ref int value) =>
                {
                    value += int.Parse(s);
                    return true;
                })
        );

        var actual = 1;
        var result = sut.OutWithRef("10", ref actual);

        Assert.True(result);
        Assert.Equal(11, actual);
    }

    public interface IMethodWithOutArgument
    {
        bool OutWithReturn(string s, out int value);
        void OutWithVoid(string s, out int value);
        bool OutWithRef(string s, ref int value);
    }
}
