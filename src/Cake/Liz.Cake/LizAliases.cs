using Cake.Core;
using Cake.Core.Annotations;
using Liz.Cake.Logging;
using Liz.Core;
using Liz.Core.Extract.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Settings;
using System.Diagnostics.CodeAnalysis;

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
    ///     Extract licenses for the provided settings
    /// </summary>
    /// <param name="context">The cake context to execute on</param>
    /// <param name="settings">The settings for the execution</param>
    /// <returns>The package-references enriched with the license-information that have been gathered</returns>
    /// <exception cref="ArgumentNullException">Thrown when any of the parameters is null</exception>
    [CakeMethodAlias]
    [CakeAliasCategory("ExtractLicenses")]
    public async static Task<IEnumerable<PackageReference>> ExtractLicensesAsync(
        this ICakeContext context, 
        ExtractLicensesSettings settings)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (settings == null) throw new ArgumentNullException(nameof(settings));

        var extractLicenses = GetExtractLicensesInstance(context, settings);

        var packageReferences = await extractLicenses.ExtractAsync().ConfigureAwait(false);
        return packageReferences;
    }

    private static IExtractLicenses GetExtractLicensesInstance(ICakeContext context, ExtractLicensesSettingsBase settings)
    {
        var loggerProvider = new CakeLoggerProvider(context);
        var extractLicensesFactory = new ExtractLicensesFactory();
        var extractLicenses = extractLicensesFactory.Create(settings, loggerProvider);
        return extractLicenses;
    }
}
