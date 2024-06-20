namespace MiniMock.UnitTests;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public static class TestGenerator
{
    private const LanguageVersion Language = LanguageVersion.CSharp11;

    public static (SyntaxTree[] syntaxTrees, Diagnostic[] diagnostics) Generate<T>(params string[] sourceCode)
        where T : IIncrementalGenerator, new() => new T().Generate(sourceCode);

    public static (SyntaxTree[] syntaxTrees, Diagnostic[] diagnostics) Generate(this IIncrementalGenerator generator,
        params string[] sourceCode)
    {
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
            .Cast<MetadataReference>();

        var syntaxTree =
            sourceCode.Select((t, index) =>
                CSharpSyntaxTree.ParseText(t, path: $"source{index}.Input.cs", options: new(Language)));

        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

        var compilation = CSharpCompilation.Create("GeneratedAssembly",
            syntaxTree,
            references,
            options);

        CSharpGeneratorDriver.Create(generator).WithUpdatedParseOptions(new CSharpParseOptions(Language))
            .RunGeneratorsAndUpdateCompilation(compilation,
                out var outputCompilation, out var diagnosticsResult);

        var syntaxTrees = outputCompilation.SyntaxTrees
            .Where(t => !t.FilePath.EndsWith(".Input.cs")).ToArray();

        var success = outputCompilation.Emit(new MemoryStream());
        var diagnostics = success.Diagnostics.Where(t => t.Severity > DiagnosticSeverity.Hidden)
            .Concat(diagnosticsResult).ToArray();

        return (syntaxTrees, diagnostics);
    }
}

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
            for (int i = 0; i < t.Length; i++)
            {
                output.WriteLine($"{(i + 1):D5} {t[i]}");
            }

            output.WriteLine("");
        }
    }

    public static string GetCode(this Diagnostic actual) => GetCode(actual.Location);

    public static string GetCode(this Location location)
    {
        var start = location.SourceSpan.Start;
        var length = location.SourceSpan.Length;
        return location.SourceTree?.ToString().Substring(start, length) ?? "";
    }
}

public static class AssertExtensions
{
    public static bool HasErrors(this Diagnostic[] diagnostics) => diagnostics.Any(t => t.Severity == DiagnosticSeverity.Error);
    public static bool HasNoErrors(this Diagnostic[] diagnostics) => diagnostics.All(t => t.Severity < DiagnosticSeverity.Error);
    public static bool HasNoWarnings(this Diagnostic[] diagnostics) => diagnostics.All(t => t.Severity < DiagnosticSeverity.Warning);

    public static Diagnostic[] GetErrors(this Diagnostic[] diagnostics) => diagnostics.Where(t => t.Severity == DiagnosticSeverity.Error).ToArray();
}
