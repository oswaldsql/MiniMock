namespace MiniMock.Builders;

/// <summary>
///     Represents a helper method with a signature, code, documentation, and an optional see cref.
/// </summary>
/// <param name="signature">The method signature.</param>
/// <param name="code">The method code.</param>
/// <param name="documentation">The method documentation.</param>
/// <param name="seeCref">An optional see cref reference.</param>
public class HelperMethod(string signature, string code, string documentation, string seeCref = "")
{
    /// <summary>
    ///     Gets the method signature.
    /// </summary>
    public string Signature { get; } = signature;

    /// <summary>
    ///     Gets the method code.
    /// </summary>
    public string Code { get; } = code;

    /// <summary>
    ///     Gets the method documentation.
    /// </summary>
    public string Documentation { get; } = documentation;

    /// <summary>
    ///     Gets the optional see cref reference.
    /// </summary>
    public string SeeCref { get; } = seeCref;
}
