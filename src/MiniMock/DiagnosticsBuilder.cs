namespace MiniMock;

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

internal static class DiagnosticsBuilder
{
    private static readonly DiagnosticDescriptor Mm0002 = new("MM0002", "Unsupported feature", "{0}", "Usage", DiagnosticSeverity.Error, true);


    private static readonly DiagnosticDescriptor Mm0003 = new("MM0003", "Unsupported feature", "{0}", "Usage", DiagnosticSeverity.Error, true);

    private static readonly DiagnosticDescriptor Mm0005 = new("MM0005", "Unsupported feature", "{0}", "Usage", DiagnosticSeverity.Error, true);

    private static readonly DiagnosticDescriptor Mm0006 = new("MM0006", "Unsupported feature", "{0}", "Usage", DiagnosticSeverity.Error, true);

    public static void AddRefPropertyNotSupported(this SourceProductionContext context, IEnumerable<Location> locations, string message)
    {
        foreach (var l in locations)
        {
            context.ReportDiagnostic(Diagnostic.Create(Mm0002, l, message));
        }
    }

    public static void AddRefReturnTypeNotSupported(this SourceProductionContext context, IEnumerable<Location> locations, string message)
    {
        foreach (var l in locations)
        {
            context.ReportDiagnostic(Diagnostic.Create(Mm0003, l, message));
        }
    }

    public static void AddStaticAbstractMembersNotSupported(this SourceProductionContext context, IEnumerable<Location> locations, string message)
    {
        foreach (var l in locations)
        {
            context.ReportDiagnostic(Diagnostic.Create(Mm0005, l, message));
        }
    }

    public static void CanNotMockASealedClass(this SourceProductionContext context, IEnumerable<Location> locations, string message)
    {
        foreach (var l in locations)
        {
            context.ReportDiagnostic(Diagnostic.Create(Mm0006, l, message));
        }
    }
}
