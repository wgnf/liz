using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;
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

        var packageReferencesList = packageReferences.OrderBy(packageReference => packageReference.Name).ToList();
        
        if (_settings.SuppressPrintDetails) return;
        if (!packageReferencesList.Any()) return;

        var messageBuilder = new StringBuilder();

        // to make some visual space to the rest of the output
        messageBuilder.AppendLine();
        messageBuilder.AppendLine();
        messageBuilder.AppendLine();
        
        messageBuilder.AppendLine($"{"Name",-60} {"Version",-15} {"Text?",-5} {"Type",-20} URL");

        messageBuilder.AppendLine();

        foreach (var packageReference in packageReferencesList)
        {
            var name = packageReference.Name;
            var version = packageReference.Version;
            var hasText = string.IsNullOrWhiteSpace(packageReference.LicenseInformation.Text) ? "No" : "Yes";
            var type = packageReference.LicenseInformation.Type;
            var url = packageReference.LicenseInformation.Url;

            messageBuilder.AppendLine($"{name,-60} {version,-15} {hasText,-5} {type,-20} {url}");
        }
        
        _logger.LogInformation(messageBuilder.ToString());
    }

    public void PrintPackageReferencesIssues(IEnumerable<PackageReference> packageReferences)
    {
        ArgumentNullException.ThrowIfNull(packageReferences);

        var packageReferencesList = packageReferences.OrderBy(packageReference => packageReference.Name).ToList();
        
        if (_settings.SuppressPrintIssues) return;
        if (!packageReferencesList.Any()) return;

        var messageBuilder = new StringBuilder();
        
        // to make some visual space to the rest of the output
        messageBuilder.AppendLine();
        messageBuilder.AppendLine();
        messageBuilder.AppendLine();

        messageBuilder.AppendLine("====== Issues ======");
        
        messageBuilder.AppendLine();

        var issuesMessage = GatherLicenseInformationIssuesMessage(packageReferencesList);
        if (string.IsNullOrWhiteSpace(issuesMessage)) return;

        messageBuilder.AppendLine(issuesMessage);
        
        _logger.LogWarning(messageBuilder.ToString());
    }

    private static string GatherLicenseInformationIssuesMessage(IEnumerable<PackageReference> packageReferences)
    {
        var issueMessageStringBuilder = new StringBuilder();

        foreach (var packageReference in packageReferences)
            GatherLicenseInformationIssuesMessage(packageReference, issueMessageStringBuilder);

        var issueMessageDetails = issueMessageStringBuilder.ToString();
        if (string.IsNullOrWhiteSpace(issueMessageDetails)) return string.Empty;

        var message =
            $"{"Name",-60} {"Version",-15} missing license-information{Environment.NewLine}{Environment.NewLine}" +
            $"{issueMessageDetails}";
        
        return message;
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

        stringBuilder.AppendLine($"{packageReference.Name,-60} {packageReference.Version,-15} {string.Join(", ", issues)}");
    }
}
