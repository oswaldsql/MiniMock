namespace MiniMock;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal static class DiagnosticsBuilder
{
    private static readonly DiagnosticDescriptor Mm0001 = new("MM0001", "Creating mock",
        "Creating mock for '{0}'", "MiniMock", DiagnosticSeverity.Info,
        true);

    public static void AddCreatingMockFor(this SourceProductionContext context, IEnumerable<Location> locations,
        ITypeSymbol sourceType) =>
        context.ReportDiagnostic(Diagnostic.Create(Mm0001, locations.FirstOrDefault(), sourceType));

    private static readonly DiagnosticDescriptor Mm0002 = new("MM0002", "Unsupported feature",
        "{0}", "MiniMock", DiagnosticSeverity.Error,
        true);

    public static void AddRefPropertyNotSupported(this SourceProductionContext context, IEnumerable<Location> locations, string message)
    {
        var l = locations.ToArray();
        var diagnostic = Diagnostic.Create(Mm0002, l.FirstOrDefault(), l.Skip(1), message);
        context.ReportDiagnostic(diagnostic);
    }


    private static readonly DiagnosticDescriptor Mm0003 = new("MM0003", "Unsupported feature",
        "{0}", "MiniMock", DiagnosticSeverity.Error,
        true);

    public static void AddRefReturnTypeNotSupported(this SourceProductionContext context, IEnumerable<Location> locations, string message)
    {
        var l = locations.ToArray();
        var diagnostic = Diagnostic.Create(Mm0003, l.FirstOrDefault(), l.Skip(1), message);
        context.ReportDiagnostic(diagnostic);
    }


    private static readonly DiagnosticDescriptor Mm0004 = new("MM0004", "Unsupported feature",
        "{0}", "MiniMock", DiagnosticSeverity.Error,
        true);

    public static void AddGenericMethodNotSupported(this SourceProductionContext context, IEnumerable<Location> locations, string message)
    {
        var l = locations.ToArray();
        var diagnostic = Diagnostic.Create(Mm0004, l.FirstOrDefault(), l.Skip(1), message);
        context.ReportDiagnostic(diagnostic);
    }

    private static readonly DiagnosticDescriptor Mm0005 = new("MM0005", "Unsupported feature",
        "{0}", "MiniMock", DiagnosticSeverity.Error,
        true);

    public static void AddStaticAbstractMembersNotSupported(this SourceProductionContext context, IEnumerable<Location> locations, string message)
    {
        var l = locations.ToArray();
        var diagnostic = Diagnostic.Create(Mm0005, l.FirstOrDefault(), l.Skip(1), message);
        context.ReportDiagnostic(diagnostic);
    }
}
