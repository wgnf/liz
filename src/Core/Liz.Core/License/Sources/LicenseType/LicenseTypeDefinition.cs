using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.License.Sources.LicenseType;

/// <summary>
///     Definition that describes a license-type
/// </summary>
[ExcludeFromCodeCoverage] // simple DTO
public class LicenseTypeDefinition
{
    /// <summary>
    ///     Creates a new instance of <see cref="LicenseTypeDefinition" />
    /// </summary>
    /// <param name="licenseType">The defined license-type</param>
    /// <param name="textSnippets">
    ///     The snippets that are contained in the license-text of that license-type, which define the
    ///     license
    /// </param>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="licenseType" /> is either null, empty or whitespace</exception>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="textSnippets" /> are null</exception>
    public LicenseTypeDefinition(string licenseType, params string[] textSnippets)
    {
        if (string.IsNullOrWhiteSpace(licenseType))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(licenseType));

        LicenseType = licenseType;
        InclusiveTextSnippets = textSnippets ?? throw new ArgumentNullException(nameof(textSnippets));
        ExclusiveTextSnippets = Enumerable.Empty<string>();
    }
    
    /// <summary>
    ///     The defined license-type
    /// </summary>
    public string LicenseType { get; }

    /// <summary>
    ///     The snippets that are contained in the license-text of that license-type, which define the license
    /// </summary>
    public IEnumerable<string> InclusiveTextSnippets { get; }

    /// <summary>
    ///     The snippets that are NOT allowed to be contained in the license-text of that license-type
    /// </summary>
    public IEnumerable<string> ExclusiveTextSnippets { get; set; }
}
