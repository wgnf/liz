using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.Utils;

internal sealed class PackageReferencePrinter : IPackageReferencePrinter
{
    private readonly ExtractLicensesSettings _settings;
    private readonly ILogger _logger;

    public PackageReferencePrinter(
        [NotNull] ExtractLicensesSettings settings,
        [NotNull] ILogger logger)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public void PrintPackageReferences(IEnumerable<PackageReference> packageReferences)
    {
        ArgumentNullException.ThrowIfNull(packageReferences);

        if (_settings.SuppressPrintDetails) return;
        
        // to make some visual space to the rest of the output
        _logger.LogInformation("\n\n\n");

        foreach (var packageReference in packageReferences)
            PrintPackageReference(packageReference);
    }

    private void PrintPackageReference(PackageReference packageReference)
    {
        var namePortion = packageReference.Name;
        var versionPortion = packageReference.Version;
        var licenseTypePortion = $"Type={packageReference.LicenseInformation?.Type ?? "-"}";
        var licenseUrlPortion = $"URL={packageReference.LicenseInformation?.Url ?? "-"}";

        var licenseText = string.IsNullOrWhiteSpace(packageReference.LicenseInformation?.Text) ? "-" : "[...]";
        var licenseTextPortion = $"Text={licenseText}";
        
        _logger.LogInformation($"> {namePortion} ({versionPortion}): {licenseTypePortion}, {licenseUrlPortion}, {licenseTextPortion}");
    }
}
