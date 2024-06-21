namespace MiniMock.UnitTests.Util;

public static class Build
{
    public static string TestClass<T>(string content = "") =>
        $@"
namespace Demo;

using MiniMock.UnitTests;
using MiniMock;
using System;

[Mock<{typeof(T).FullName!.Replace("+", ".")}>]
public class TestClass{{
    public void Test() {{
       {content}
    }}
}}";

}
