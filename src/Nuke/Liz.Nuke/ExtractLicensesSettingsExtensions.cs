using Liz.Core.License.Contracts.Models;
using Nuke.Common.IO;

namespace Liz.Nuke;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

/// <summary>
///     Extensions to help setup your <see cref="ExtractLicensesSettings"/>
/// </summary>
public static class ExtractLicensesSettingsExtensions
{
    /// <summary>
    ///     Set the target file
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <param name="targetFile">The value to set</param>
    /// <returns>The settings</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="targetFile"/> is null</exception>
    public static ExtractLicensesSettings SetTargetFile(this ExtractLicensesSettings settings, AbsolutePath targetFile)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (targetFile == null) throw new ArgumentNullException(nameof(targetFile));

        settings.TargetFile = targetFile;
        return settings;
    }
    
    /// <summary>
    ///     Set whether or not to include transitive (dependencies of dependencies) dependencies
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <param name="value">The value to set</param>
    /// <returns>The settings</returns>
    public static ExtractLicensesSettings SetIncludeTransitiveDependencies(
        this ExtractLicensesSettings settings,
        bool value)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        
        settings.IncludeTransitiveDependencies = value;
        return settings;
    }

    /// <summary>
    ///     Enable include transitive (dependencies of dependencies) dependencies
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <returns>The settings</returns>
    public static ExtractLicensesSettings EnableIncludeTransitiveDependencies(this ExtractLicensesSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        
        settings.IncludeTransitiveDependencies = true;
        return settings;
    }

    /// <summary>
    ///     Disable include transitive (dependencies of dependencies) dependencies
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <returns>The settings</returns>
    public static ExtractLicensesSettings DisableIncludeTransitiveDependencies(this ExtractLicensesSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        
        settings.IncludeTransitiveDependencies = false;
        return settings;
    }

    /// <summary>
    ///     Toggle include transitive (dependencies of dependencies) dependencies
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <returns>The settings</returns>
    public static ExtractLicensesSettings ToggleIncludeTransitiveDependencies(this ExtractLicensesSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        
        settings.IncludeTransitiveDependencies = !settings.IncludeTransitiveDependencies;
        return settings;
    }

    /// <summary>
    ///     Reset include transitive (dependencies of dependencies) dependencies to the default value
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <returns>The settings</returns>
    public static ExtractLicensesSettings ResetIncludeTransitiveDependencies(this ExtractLicensesSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        
        settings.IncludeTransitiveDependencies = new ExtractLicensesSettings().IncludeTransitiveDependencies;
        return settings;
    }
    
    
    /// <summary>
    ///     Set whether or not to suppress printing details
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <param name="value">The value to set</param>
    /// <returns>The settings</returns>
    public static ExtractLicensesSettings SetSuppressPrintDetails(
        this ExtractLicensesSettings settings,
        bool value)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        
        settings.SuppressPrintDetails = value;
        return settings;
    }

    /// <summary>
    ///     Enable suppress printing details
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <returns>The settings</returns>
    public static ExtractLicensesSettings EnableSuppressPrintDetails(this ExtractLicensesSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        
        settings.SuppressPrintDetails = true;
        return settings;
    }

    /// <summary>
    ///     Disable suppress printing details
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <returns>The settings</returns>
    public static ExtractLicensesSettings DisableSuppressPrintDetails(this ExtractLicensesSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        
        settings.SuppressPrintDetails = false;
        return settings;
    }

    /// <summary>
    ///     Toggle suppress printing details
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <returns>The settings</returns>
    public static ExtractLicensesSettings ToggleSuppressPrintDetails(this ExtractLicensesSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        
        settings.SuppressPrintDetails = !settings.SuppressPrintDetails;
        return settings;
    }

    /// <summary>
    ///     Reset suppress printing details to the default value
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <returns>The settings</returns>
    public static ExtractLicensesSettings ResetSuppressPrintDetails(this ExtractLicensesSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        
        settings.SuppressPrintDetails = new ExtractLicensesSettings().SuppressPrintDetails;
        return settings;
    }
    
    /// <summary>
    ///     Set suppress print issues
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <param name="value">The value to set</param>
    /// <returns>The settings</returns>
    public static ExtractLicensesSettings SetSuppressPrintIssues(
        this ExtractLicensesSettings settings,
        bool value)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        
        settings.SuppressPrintIssues = value;
        return settings;
    }

    /// <summary>
    ///     Enable suppress print issues
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <returns>The settings</returns>
    public static ExtractLicensesSettings EnableSuppressPrintIssues(this ExtractLicensesSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        
        settings.SuppressPrintIssues = true;
        return settings;
    }

    /// <summary>
    ///     Disable suppress print issues
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <returns>The settings</returns>
    public static ExtractLicensesSettings DisableSuppressPrintIssues(this ExtractLicensesSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        
        settings.SuppressPrintIssues = false;
        return settings;
    }

    /// <summary>
    ///     Toggle suppress print issues
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <returns>The settings</returns>
    public static ExtractLicensesSettings ToggleSuppressPrintIssues(this ExtractLicensesSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        
        settings.SuppressPrintIssues = !settings.SuppressPrintIssues;
        return settings;
    }

    /// <summary>
    ///     Reset suppress print issues to the default value
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <returns>The settings</returns>
    public static ExtractLicensesSettings ResetSuppressPrintIssues(this ExtractLicensesSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        
        settings.SuppressPrintIssues = new ExtractLicensesSettings().SuppressPrintIssues;
        return settings;
    }

    /// <summary>
    ///     Set the list of <see cref="LicenseTypeDefinition"/>s that describe license-types by providing inclusive/exclusive
    ///     license-text snippets
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <param name="licenseTypeDefinitions">The value to set</param>
    /// <returns>The settings</returns>
    /// <exception cref="ArgumentNullException">Thrown when any of the provided parameters is null</exception>
    public static ExtractLicensesSettings SetLicenseTypeDefinitions(
        this ExtractLicensesSettings settings,
        List<LicenseTypeDefinition> licenseTypeDefinitions)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (licenseTypeDefinitions == null) throw new ArgumentNullException(nameof(licenseTypeDefinitions));

        settings.LicenseTypeDefinitions = licenseTypeDefinitions ?? throw new ArgumentNullException(nameof(licenseTypeDefinitions));
        return settings;
    }
    
    /// <summary>
    ///     <para>
    ///         Set the path to a JSON-file (local or remote - remote will be downloaded automatically if available) containing
    ///         a list of <see cref="LicenseTypeDefinition"/>s that describe license-types by providing inclusive/exclusive
    ///         license-text snippets
    ///     </para>
    ///     <para>
    ///         If both "LicenseTypeDefinitions" and "LicenseTypeDefinitionsFilePath" are given, those two will be merged
    ///     </para>
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <param name="licenseTypeDefinitionsFilePath">The value to set</param>
    /// <returns>The settings</returns>
    /// <exception cref="ArgumentNullException">Thrown when the parameter <paramref name="settings"/> is null</exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when the parameter <paramref name="licenseTypeDefinitionsFilePath"/> is either null, empty or whitespace
    /// </exception>
    public static ExtractLicensesSettings SetLicenseTypeDefinitionsFilePath(
        this ExtractLicensesSettings settings,
        string licenseTypeDefinitionsFilePath)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (string.IsNullOrWhiteSpace(licenseTypeDefinitionsFilePath))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(licenseTypeDefinitionsFilePath));

        settings.LicenseTypeDefinitionsFilePath = licenseTypeDefinitionsFilePath;
        return settings;
    }

    /// <summary>
    ///     Set the license-url (key) to license-type (value) mapping, for licenses whose license-type could not be determined
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <param name="urlToLicenseTypeMapping">The value to set</param>
    /// <returns>The settings</returns>
    /// <exception cref="ArgumentNullException">Thrown when any of the provided parameters is null</exception>
    public static ExtractLicensesSettings SetUrlToLicenseTypeMapping(
        this ExtractLicensesSettings settings,
        Dictionary<string, string> urlToLicenseTypeMapping)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (urlToLicenseTypeMapping == null) throw new ArgumentNullException(nameof(urlToLicenseTypeMapping));

        settings.UrlToLicenseTypeMapping = urlToLicenseTypeMapping;
        return settings;
    }

    /// <summary>
    ///     <para>
    ///         Set the path to a JSON-file (local or remote - remote will be downloaded automatically if available) containing
    ///         a mapping from license-url (key) to license-type (value) for licenses whose license-type could not be determined
    ///     </para>
    ///     <para>
    ///         If both "UrlToLicenseTypeMapping" and "UrlToLicenseTypeMappingFilePath" are given, those two will be merged,
    ///         ignoring any duplicate keys.
    ///     </para>
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <param name="urlToLicenseTypeMappingFilePath">The value to set</param>
    /// <returns>The settings</returns>
    /// <exception cref="ArgumentNullException">Thrown when the parameter <paramref name="settings"/> is null</exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when the parameter <paramref name="urlToLicenseTypeMappingFilePath"/> is either null, empty or whitespace
    /// </exception>
    public static ExtractLicensesSettings SetUrlToLicenseTypeMappingFilePath(
        this ExtractLicensesSettings settings,
        string urlToLicenseTypeMappingFilePath)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (string.IsNullOrWhiteSpace(urlToLicenseTypeMappingFilePath))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(urlToLicenseTypeMappingFilePath));

        settings.UrlToLicenseTypeMappingFilePath = urlToLicenseTypeMappingFilePath;
        return settings;
    }
}
