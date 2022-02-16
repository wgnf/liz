using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Liz.Cake.Logging;
using Liz.Core;
using Liz.Core.Extract.Contracts;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Settings;
using System.Diagnostics.CodeAnalysis;
using Path = Cake.Core.IO.Path;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Liz.Cake;

/// <summary>
///     Aliases for the Liz-Tool to extract licenses
/// </summary>
[ExcludeFromCodeCoverage] // not sure how to test this and if this even makes sense (in the future for integration-test, yes)
[CakeAliasCategory("Licenses")]
public static class LizAliases
{
    /// <summary>
    ///     Extract licenses for the provided <paramref name="targetFile"/> with the default settings
    /// </summary>
    /// <param name="context">The cake context to execute on</param>
    /// <param name="targetFile">The target-file to analyze. This can be a .csproj,.fsproj,.sln</param>
    /// <returns>The package-references enriched with the license-information that have been gathered</returns>
    /// <exception cref="ArgumentNullException">Thrown when any of the parameters is null</exception>
    [CakeMethodAlias]
    [CakeAliasCategory("ExtractLicenses")]
    public async static Task<IEnumerable<PackageReference>> ExtractLicensesAsync(this ICakeContext context, FilePath targetFile)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (targetFile == null) throw new ArgumentNullException(nameof(targetFile));

        var settings = new ExtractLicensesToolSettings();
        var packageReferences = await ExtractLicensesAsync(context, targetFile, settings);
        return packageReferences;
    }

    /// <summary>
    ///     Extract licenses for the provided <paramref name="targetFile"/> with the provided settings
    /// </summary>
    /// <param name="context">The cake context to execute on</param>
    /// <param name="targetFile">The target-file to analyze. This can be a .csproj,.fsproj,.sln</param>
    /// <param name="settings">The settings for the execution</param>
    /// <returns>The package-references enriched with the license-information that have been gathered</returns>
    /// <exception cref="ArgumentNullException">Thrown when any of the parameters is null</exception>
    [CakeMethodAlias]
    [CakeAliasCategory("ExtractLicenses")]
    public async static Task<IEnumerable<PackageReference>> ExtractLicensesAsync(
        this ICakeContext context, 
        FilePath targetFile,
        ExtractLicensesToolSettings settings)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (targetFile == null) throw new ArgumentNullException(nameof(targetFile));
        if (settings == null) throw new ArgumentNullException(nameof(settings));

        var extractLicenses = GetExtractLicensesInstance(context, targetFile, settings);

        var packageReferences = await extractLicenses.ExtractAsync();
        return packageReferences;
    }

    private static IExtractLicenses GetExtractLicensesInstance(
        ICakeContext context,
        Path targetFile, 
        ExtractLicensesToolSettings settings)
    {
        var loggerProvider = new CakeLoggerProvider(context);
        var extractLicensesSettings = new ExtractLicensesSettings(targetFile.FullPath)
        {
            IncludeTransitiveDependencies = settings.IncludeTransitiveDependencies,
            // just use the lowest one, because the log level comes from cake itself
            LogLevel = LogLevel.Trace,
            SuppressPrintDetails = settings.SuppressPrintDetails,
            SuppressPrintIssues = settings.SuppressPrintIssues
        };

        var extractLicensesFactory = new ExtractLicensesFactory();
        var extractLicenses = extractLicensesFactory.Create(extractLicensesSettings, loggerProvider);
        return extractLicenses;
    }
}
