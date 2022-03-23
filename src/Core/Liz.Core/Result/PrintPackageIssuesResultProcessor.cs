using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Result.Contracts;
using Liz.Core.Settings;
using System.Text;

namespace Liz.Core.Result;

internal sealed class PrintPackageIssuesResultProcessor : IResultProcessor
{
    private readonly ExtractLicensesSettingsBase _settings;
    private readonly ILogger _logger;

    public PrintPackageIssuesResultProcessor(ExtractLicensesSettingsBase settings, ILogger logger)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    
    public Task ProcessResultsAsync(IEnumerable<PackageReference> packageReferences)
    {
        if (packageReferences == null) throw new ArgumentNullException(nameof(packageReferences));
        if (_settings.SuppressPrintIssues) return Task.CompletedTask;

        var packageReferencesList = packageReferences
            .OrderBy(packageReference => packageReference.Name)
            .ToList();
        
        if (!packageReferencesList.Any()) return Task.CompletedTask;
        
        var messageBuilder = new StringBuilder();
        
        // to make some visual space to the rest of the output
        messageBuilder.AppendLine();
        messageBuilder.AppendLine();
        messageBuilder.AppendLine();

        messageBuilder.AppendLine("====== Issues ======");
        
        messageBuilder.AppendLine();

        var issuesMessage = GatherLicenseInformationIssuesMessage(packageReferencesList);
        if (string.IsNullOrWhiteSpace(issuesMessage)) return Task.CompletedTask;

        messageBuilder.AppendLine(issuesMessage);
        
        _logger.LogWarning(messageBuilder.ToString());

        return Task.CompletedTask;
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
