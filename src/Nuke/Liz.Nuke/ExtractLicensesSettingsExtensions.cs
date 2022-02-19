using Nuke.Common.IO;

namespace Liz.Nuke;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

/// <summary>
///     
/// </summary>
public static class ExtractLicensesSettingsExtensions
{
    /// <summary>
    ///     Set the target file
    /// </summary>
    /// <param name="settings">The settings to set the value on</param>
    /// <param name="value">The value to set</param>
    /// <returns>The settings</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null</exception>
    public static ExtractLicensesSettings SetTargetFile(this ExtractLicensesSettings settings, AbsolutePath value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        settings.TargetFile = value;
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
        settings.SuppressPrintIssues = new ExtractLicensesSettings().SuppressPrintIssues;
        return settings;
    }
}
