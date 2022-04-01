using Liz.Core.License.Sources.LicenseType;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.Settings;

/// <summary>
///     Settings to configure the Extract-Licenses-Tool aka Liz
/// </summary>
[ExcludeFromCodeCoverage] // simple DTO
public abstract class ExtractLicensesSettingsBase
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
    ///    Whether or not to suppress printing found issues of analyzed package-references and license-information
    /// </summary>
    public bool SuppressPrintIssues { get; set; }

    /// <summary>
    ///     A list of <see cref="LicenseTypeDefinition"/>s that describe license-types by providing inclusive/exclusive
    ///     license-text snippets
    /// </summary>
    public List<LicenseTypeDefinition> LicenseTypeDefinitions { get; set; } = new();

    /// <summary>
    ///     Gets the target file of these settings
    /// </summary>
    /// <returns>The set target file</returns>
    public abstract string? GetTargetFile();

    /// <summary>
    ///     Ensures the validity of these settings and throws an exception when there's an issue
    /// </summary>
    /// <exception cref="SettingsInvalidException">Thrown when there is an issue with the settings</exception>
    public void EnsureValidity()
    {
        var targetFile = GetTargetFile();
        
        if (string.IsNullOrWhiteSpace(targetFile))
            throw new SettingsInvalidException("The target-file cannot be null/empty/whitespace");

        if (!File.Exists(targetFile))
            throw new SettingsInvalidException("The given target-file does not exist");

        var targetFileExtension = Path.GetExtension(targetFile);
        if (targetFileExtension != ".csproj" &&
            targetFileExtension != ".fsproj" &&
            targetFileExtension != ".sln")
            throw new SettingsInvalidException("The given target-file is not a csproj, fsproj nor sln file");
    }
}
