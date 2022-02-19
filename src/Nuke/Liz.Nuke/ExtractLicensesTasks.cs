using Liz.Core;
using Liz.Core.Extract.Contracts;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Settings;
using Liz.Nuke.Logging;
using Nuke.Common.Tooling;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Liz.Nuke;

/// <summary>
///     Tasks for extracting licenses provided by Liz
/// </summary>
[ExcludeFromCodeCoverage] // not sure how to test this and if this even makes sense (in the future for integration-test, yes)
public static class ExtractLicensesTasks
{
    /// <summary>
    ///     Extract licenses based on the provided settings
    /// </summary>
    /// <param name="configureSettings">Configuration of the needed <see cref="ExtractLicensesSettings"/></param>
    /// <returns>The package-references enriched with the license-information that have been gathered</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configureSettings"/> is null</exception>
    public async static Task<IEnumerable<PackageReference>> ExtractLicensesAsync(Configure<ExtractLicensesSettings> configureSettings)
    {
        if (configureSettings == null) throw new ArgumentNullException(nameof(configureSettings));
        
        var settings = configureSettings.Invoke(new ExtractLicensesSettings());
        var extractLicenses = GetExtractLicensesInstance(settings);

        var packageReferences = await extractLicenses.ExtractAsync();
        return packageReferences;
    }

    private static IExtractLicenses GetExtractLicensesInstance(ExtractLicensesSettings settings)
    {
        if (settings.TargetFile == null)
            throw new InvalidOperationException(
                $"{nameof(settings.TargetFile)} of {nameof(ExtractLicensesSettings)} cannot be null");

        var nukeLoggerProvider = new NukeLoggerProvider();
        var extractLicensesFactory = new ExtractLicensesFactory();
        var extractLicenses = extractLicensesFactory.Create(settings, nukeLoggerProvider);
        return extractLicenses;
    }
}
