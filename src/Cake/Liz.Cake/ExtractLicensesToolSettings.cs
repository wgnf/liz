using System.Diagnostics.CodeAnalysis;

namespace Liz.Cake;

/// <summary>
///     Settings to parameterize the license extraction of the Liz-Tool
/// </summary>
[ExcludeFromCodeCoverage] // simple settings DTO
public class ExtractLicensesToolSettings
{
    /// <summary>
    ///     Whether or not to include transitive (dependencies of dependencies) dependencies
    /// </summary>
    public bool IncludeTransitiveDependencies { get; set; }
    
    /// <summary>
    ///     Whether or not to suppress printing details of analyzed package-references and license-information
    /// </summary>
    public bool SuppressPrintDetails { get; set; }
    
    /// <summary>
    ///     Whether or not to suppress printing found issues of analyzed package-references and license-information
    /// </summary>
    public bool SuppressPrintIssues { get; set; }
}
