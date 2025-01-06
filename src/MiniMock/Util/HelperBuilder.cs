namespace MiniMock.Util;

using System.Collections.Generic;
using System.Linq;
using Builders;

public static class HelperBuilder
{
    internal static CodeBuilder BuildHelpers(this List<HelperMethod> helpers, string name)
    {
        CodeBuilder builder = new();

        if (helpers.Count == 0)
        {
            return builder;
        }

        var signatures = helpers.ToLookup(t => t.Signature);

        builder.Add("internal partial class Config {").Indent();

        foreach (var grouping in signatures)
        {
            AddDocumentation(builder, grouping);
            AddCode(name, builder, grouping);
        }

        builder.Unindent().Add("}");

        return builder;
    }

    private static void AddCode(string name, CodeBuilder builder, IGrouping<string, HelperMethod> methodSignatures)
    {
        builder.Add($"public Config {name}({methodSignatures.Key}) {{").Indent();

        foreach (var mse in methodSignatures)
        {
            builder.Add(mse.Code);
        }

        builder.Add("return this;").Unindent();
        builder.Add("}");
        builder.Add();
    }

    private static void AddDocumentation(CodeBuilder builder, IGrouping<string, HelperMethod> methodSignatures)
    {
        builder.Add("/// <summary>");

        foreach (var docLine in methodSignatures.Select(t => t.Documentation).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct())
        {
            builder.Add("///     " + docLine);
        }

        var references = methodSignatures.Select(t => t.SeeCref).Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => $"<see cref=\"{t.EscapeToHtml()}\" />").ToArray();
        if (references.Any())
        {
            builder.Add("///     Configures " + string.Join(", ", references));
        }

        builder.Add("""
                    /// </summary>
                    /// <returns>The updated configuration.</returns>
                    """);
    }
}
