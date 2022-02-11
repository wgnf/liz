using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liz.Core.Utils;

internal sealed class PackageReferencePrinter : IPackageReferencePrinter
{
    private readonly ExtractLicensesSettings _settings;
    private readonly ILogger _logger;

    public PackageReferencePrinter(ExtractLicensesSettings settings, ILogger logger)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public void PrintPackageReferences(IEnumerable<PackageReference> packageReferences)
    {
        ArgumentNullException.ThrowIfNull(packageReferences);

        if (_settings.SuppressPrintDetails) return;

        var packageReferencesList = packageReferences.ToList();
        
        if (!packageReferencesList.Any()) return;
        
        // to make some visual space to the rest of the output
        _logger.LogInformation("\n\n\n");

        foreach (var packageReference in packageReferencesList)
            PrintPackageReference(packageReference);
    }

    public void PrintPackageReferencesIssues(IEnumerable<PackageReference> packageReferences)
    {
        ArgumentNullException.ThrowIfNull(packageReferences);

        if (_settings.SuppressPrintIssues) return;
        
        var packageReferencesList = packageReferences.ToList();
        
        if (!packageReferencesList.Any()) return;

        var issuesMessage = GatherIssuesMessage(packageReferencesList);
        if (string.IsNullOrWhiteSpace(issuesMessage)) return;
        
        _logger.LogWarning($"\n\n\n--- Issues ---\n{issuesMessage}");
    }

    private void PrintPackageReference(PackageReference packageReference)
    {
        var namePortion = packageReference.Name;
        var versionPortion = packageReference.Version;
        var licenseTypePortion = $"Type={(string.IsNullOrWhiteSpace(packageReference.LicenseInformation.Text) ? "-" : packageReference.LicenseInformation.Type)}";
        var licenseUrlPortion = $"URL={(string.IsNullOrWhiteSpace(packageReference.LicenseInformation.Url) ? "-" : packageReference.LicenseInformation.Url)}";

        var licenseText = string.IsNullOrWhiteSpace(packageReference.LicenseInformation.Text) ? "-" : "[...]";
        var licenseTextPortion = $"Text={licenseText}";
        
        _logger.LogInformation($"> {namePortion} ({versionPortion}): {licenseTypePortion}, {licenseUrlPortion}, {licenseTextPortion}");
    }

    private static string GatherIssuesMessage(IEnumerable<PackageReference> packageReferences)
    {
        var issueMessageStringBuilder = new StringBuilder();

        foreach (var packageReference in packageReferences)
            GatherLicenseInformationIssuesMessage(packageReference, issueMessageStringBuilder);

        return issueMessageStringBuilder.ToString();
    }

    private static void GatherLicenseInformationIssuesMessage(
        PackageReference packageReference,
        StringBuilder stringBuilder)
    {
        var issues = new List<string>();
        
        if (string.IsNullOrWhiteSpace(packageReference.LicenseInformation.Type))
            issues.Add("Type");
        
        if (string.IsNullOrWhiteSpace(packageReference.LicenseInformation.Url))
            issues.Add("URL");
        
        if (string.IsNullOrWhiteSpace(packageReference.LicenseInformation.Text))
            issues.Add("Text");

        if (!issues.Any()) return;

        stringBuilder.AppendLine(
            $"> {packageReference.Name} ({packageReference.Version}): " +
            $"Following license-information could not be determined: {string.Join(", ", issues)}");
    }
}
