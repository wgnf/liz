using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Liz.Core.License.Contracts.Models;

/// <summary>
///     An object containing information about a license
/// </summary>
[ExcludeFromCodeCoverage] // DTO
public sealed class LicenseInformation
{
    private readonly List<string> _types = new();

    /// <summary>
    ///     Creates a new instance of <see cref="LicenseInformation"/>
    /// </summary>
    public LicenseInformation()
    {
        Url = string.Empty;
        Text = string.Empty;
    }

    /// <summary>
    ///     The types of a license as an SPDX-ID (see https://spdx.org/licenses/)
    /// </summary>
    /// <example>MIT</example>
    public IEnumerable<string> Types => _types;

    /// <summary>
    ///     The URL of the license
    /// </summary>
    /// <example>https://licenses.nuget.org/MIT</example>
    public string Url { get; set; }

    /// <summary>
    ///     The raw license text of that license
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    ///     Provides a string that represents this instance
    /// </summary>
    /// <returns>A string representing this instance</returns>
    public override string ToString()
    {
        var builder = new StringBuilder();

        builder.Append(nameof(LicenseInformation));
        builder.Append(" { ");
        builder.Append($"{nameof(Types)}={string.Join(", ", Types)}, ");
        builder.Append($"{nameof(Url)}={Url}");
        // leaving out 'Text' because it can be quite long
        builder.Append(" }");

        var objectString = builder.ToString();
        return objectString;
    }

    internal void AddLicenseType(string licenseType)
    {
        if (string.IsNullOrWhiteSpace(licenseType))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(licenseType));
        
        // no need to add if already contained
        if (_types.Contains(licenseType)) return;
        
        _types.Add(licenseType);
    }
}
