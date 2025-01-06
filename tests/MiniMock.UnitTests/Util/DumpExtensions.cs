namespace MiniMock.UnitTests.Util;

using Microsoft.CodeAnalysis;

public static class DumpExtensions
{
    public static void DumpResult(this ITestOutputHelper output,
        (SyntaxTree[] syntaxTrees, Diagnostic[] diagnostics) source) =>
        output.DumpResult(source.syntaxTrees, source.diagnostics);

    public static void DumpResult(this ITestOutputHelper output, SyntaxTree[] syntaxTrees, Diagnostic[] diagnostics)
    {
        if (diagnostics.Length == 0)
        {
            output.WriteLine("No diagnostics found");
        }
        else
        {
            output.WriteLine("--- diagnostics ---");

            foreach (var item in diagnostics)
            {
                output.WriteLine(item + " ['" + item.GetCode() + "']");
            }

            output.WriteLine("");
        }

        foreach (var syntaxTree in syntaxTrees.Where(t => !t.FilePath.EndsWith("MapperAttribute.g.cs")))
        {
            output.WriteLine("--- File : " + syntaxTree.FilePath + " ---");

            var t = syntaxTree.ToString().Split("\r\n");
            for (var i = 0; i < t.Length; i++)
            {
                output.WriteLine($"{i + 1:D5} {t[i]}");
            }

            output.WriteLine("");
        }
    }

    public static string GetCode(this Diagnostic actual) => actual.Location.GetCode();

    public static string GetCode(this Location location)
    {
        var start = location.SourceSpan.Start;
        var length = location.SourceSpan.Length;
        return location.SourceTree?.ToString().Substring(start, length) ?? "";
    }
}
