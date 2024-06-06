namespace MiniMock.UnitTests;

public static class TestSourceBuilder
{
    public static string BuildEmptyClassWithAttribute<T>() =>
        $@"
namespace Demo;
using MiniMock.UnitTests;
using MiniMock;
using System;

[Mock<{typeof(T).FullName.Replace("+", ".")}>]
public class TestClass{{}}";
}
