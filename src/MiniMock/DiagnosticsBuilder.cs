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




    //private static readonly DiagnosticDescriptor Em0002 = new("EM0002", "No matching property or parameter found",
    //    "No parameter or property was found that matched '{0}'", "EasyMapper", DiagnosticSeverity.Error, true);

    //private static readonly DiagnosticDescriptor Em0003 = new("EM0003", "Unable to identify constructor",
    //    "Unable to identify constructor for type '{0}'", "EasyMapper", DiagnosticSeverity.Error, true);

    //private static readonly DiagnosticDescriptor Em0004 = new("EM0004", "From property or parameter not found",
    //    "Unable to find property or field '{0}'", "EasyMapper", DiagnosticSeverity.Error, true);

    //private static readonly DiagnosticDescriptor Em0005 = new("EM0005", "Potential multiple setting of value",
    //    "Property '{0}' exists in both constructor and as a property", "EasyMapper", DiagnosticSeverity.Warning, true);

    //private static readonly DiagnosticDescriptor Em0006 = new("EM0006", "The specified converter was not found",
    //    "The specified converter '{1}' for '{0}' was not found", "EasyMapper", DiagnosticSeverity.Error, true);


    //private static readonly DiagnosticDescriptor Em0007 = new("EM0007", "The specified converter is not compatible with the given parameters",
    //    "The specified converter '{1}' for '{0}' is not compatible", "EasyMapper", DiagnosticSeverity.Error, true);

    //private static readonly DiagnosticDescriptor Em0008 = new("EM0008", "The specified create method was not found or did not match the expected return type",
    //    "The specified create method '{0}' was not found or did not return '{1}'", "EasyMapper", DiagnosticSeverity.Error, true);


    //private static readonly DiagnosticDescriptor Em9999 = new("EM9999", "Unknown exception happened during mapping", "{0}",
    //    "EasyMapper", DiagnosticSeverity.Error, true);


    //public static void AddSpecifiedConverterNotFound(this List<Diagnostic> target, IEnumerable<Location> locations, string targetPath, string converter)
    //{
    //    target.Add(Diagnostic.Create(Em0006, locations.FirstOrDefault(), targetPath, converter));
    //}

    //public static void AddSpecifiedConverterNotCompatible(this List<Diagnostic> target, IEnumerable<Location> locations, string targetPath, string converter)
    //{
    //    target.Add(Diagnostic.Create(Em0007, locations.FirstOrDefault(), targetPath, converter));
    //}

    //public static void AddUnknownError(this List<Diagnostic> target, Exception ex, IEnumerable<Location> methodLocations)
    //{
    //    target.Add(Diagnostic.Create(Em9999, methodLocations.First(), ex.Message));
    //}

    //public static void AddSourcePropertyNotFound(this List<Diagnostic> target, IEnumerable<Location> locations, string from)
    //{
    //    target.Add(Diagnostic.Create(Em0004, locations.FirstOrDefault(), from));
    //}

    //public static void AddConstructorNotIdentified(this List<Diagnostic> target, IEnumerable<Location> returnTypeLocations, ITypeSymbol returnType)
    //{
    //    target.Add(Diagnostic.Create(Em0003, returnTypeLocations.First(), returnType));
    //}

    //public static void AddCreateMethodNotFound(this List<Diagnostic> target, IEnumerable<Location> returnTypeLocations, string createMethod, ITypeSymbol returnType)
    //{
    //    target.Add(Diagnostic.Create(Em0008, returnTypeLocations.First(), createMethod, returnType.Name));
    //}

    //public static void AddUnknownSource(this List<Diagnostic> target, IEnumerable<Location> locations, string targetName)
    //{
    //    target.Add(Diagnostic.Create(Em0002, locations.First(), targetName));
    //}

    //public static void AddMatchingParameterExistsInCtor(this List<Diagnostic> target, IEnumerable<Location> locations, string propertyName)
    //{
    //    target.Add(Diagnostic.Create(Em0005, locations.First(), propertyName));
    //}
}
