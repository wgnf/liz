using Liz.Core.License.Contracts.Models;
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
    ///     <para>
    ///         A path to a JSON-file (local or remote - remote will be downloaded automatically if available) containing a
    ///         list of <see cref="LicenseTypeDefinition"/>s that describe license-types by providing inclusive/exclusive
    ///         license-text snippets
    ///     </para>
    ///     <para>
    ///         If both <see cref="LicenseTypeDefinitions"/> and <see cref="LicenseTypeDefinitionsFilePath"/> are given,
    ///         those two will be merged
    ///     </para>
    /// </summary>
    public string? LicenseTypeDefinitionsFilePath { get; set; }

    /// <summary>
    ///     A mapping from license-url (key) to license-type (value) for licenses whose license-type could not be determined
    /// </summary>
    public Dictionary<string, string> UrlToLicenseTypeMapping { get; set; } = new();

    /// <summary>
    ///     <para>
    ///         A path to a JSON-file (local or remote - remote will be downloaded automatically if available) containing a
    ///         mapping from license-url (key) to license-type (value) for licenses whose license-type could not be determined
    ///     </para>
    ///     <para>
    ///         If both <see cref="UrlToLicenseTypeMapping"/> and <see cref="UrlToLicenseTypeMappingFilePath"/> are given,
    ///         those two will be merged, ignoring any duplicate keys
    ///     </para>
    /// </summary>
    public string? UrlToLicenseTypeMappingFilePath { get; set; }

    
    /// <summary>
    ///     <para>
    ///         A list of license-types, which are the only ones allowed, when validating the determined license-types.
    ///         Any license-type which is not in the whitelist will cause the validation to fail.
    ///     </para>
    ///     <para>
    ///         This option is mutually exclusive with <see cref="LicenseTypeBlacklist"/> and
    ///         <see cref="LicenseTypeBlacklistFilePath"/>
    ///     </para>
    /// </summary>
    public List<string> LicenseTypeWhitelist { get; set; } = new();
    
    /// <summary>
    ///     <para>
    ///         A path to a JSON-File (local or remote - remote will be downloaded automatically if available) containing
    ///         a list of license-types, which are the only ones allowed, when validating the determined license-types.
    ///         Any license-type which is not in the whitelist will cause the validation to fail.
    ///     </para>
    ///     <para>
    ///         This option is mutually exclusive with <see cref="LicenseTypeBlacklist"/> and
    ///         <see cref="LicenseTypeBlacklistFilePath"/>
    ///     </para>
    ///     <para>
    ///         If both <see cref="LicenseTypeWhitelist"/> and <see cref="LicenseTypeWhitelistFilePath"/> are given,
    ///         those two will be merged
    ///     </para>
    /// </summary>
    public string? LicenseTypeWhitelistFilePath { get; set; }

    /// <summary>
    ///     <para>
    ///         A list of license-types, which are the only ones disallowed, when validating the determined license-types.
    ///         Any license-type that is the same as within that blacklist will cause the validation to fail. Any other
    ///         license-type is allowed.
    ///     </para>
    ///     <para>
    ///         This option is mutually exclusive with <see cref="LicenseTypeWhitelist"/> and
    ///         <see cref="LicenseTypeWhitelistFilePath"/>
    ///     </para>
    /// </summary>
    public List<string> LicenseTypeBlacklist { get; set; } = new();
    
    /// <summary>
    ///     <para>
    ///         A path to a JSON-File (local or remote - remote will be downloaded automatically if available) containing
    ///         a list of license-types, which are the only ones disallowed, when validating the determined license-types.
    ///         Any license-type that is the same as within that blacklist will cause the validation to fail. Any other
    ///         license-type is allowed.
    ///     </para>
    ///     <para>
    ///         This option is mutually exclusive with <see cref="LicenseTypeWhitelist"/> and
    ///         <see cref="LicenseTypeWhitelistFilePath"/>
    ///     </para>
    ///     <para>
    ///         If both <see cref="LicenseTypeBlacklist"/> and <see cref="LicenseTypeBlacklistFilePath"/> are given,
    ///         those two will be merged
    ///     </para>
    /// </summary>
    public string? LicenseTypeBlacklistFilePath { get; set; }
    
    /// <summary>
    ///     <para>
    ///         A path to a directory to where the determined license-texts will be exported
    ///     </para>
    ///     <para>
    ///         Each license-text will be written to an individual file with the file-name being:
    ///         "{package-name}-{package-version}.txt". If the license-text is the content of a website, the contents
    ///         will be written into an ".html" file instead
    ///     </para>
    /// </summary>
    public string? ExportLicenseTextsDirectory { get; set; }

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
        if (!targetFileExtension.Contains("csproj", StringComparison.InvariantCultureIgnoreCase) &&
            !targetFileExtension.Contains("fsproj", StringComparison.InvariantCultureIgnoreCase) &&
            !targetFileExtension.Contains("sln", StringComparison.InvariantCultureIgnoreCase))
            throw new SettingsInvalidException("The given target-file is not a csproj, fsproj nor sln file");

        // this will ensure the mutual exclusivity of the license-type whitelist and blacklist
        if ((LicenseTypeWhitelist.Any() || !string.IsNullOrWhiteSpace(LicenseTypeWhitelistFilePath)) &&
            (LicenseTypeBlacklist.Any() || !string.IsNullOrWhiteSpace(LicenseTypeBlacklistFilePath)))
            throw new SettingsInvalidException("License-type whitelist and blacklist are mutually exclusive. " +
                                               "You cannot use them together. Either use the whitelist or the blacklist.");
    }
}
