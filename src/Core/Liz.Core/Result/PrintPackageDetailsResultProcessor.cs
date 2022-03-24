using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Result.Contracts;
using Liz.Core.Settings;
using System.Text;

namespace Liz.Core.Result;

internal sealed class PrintPackageDetailsResultProcessor : IResultProcessor
{
    private readonly ExtractLicensesSettingsBase _settings;
    private readonly ILogger _logger;

    public PrintPackageDetailsResultProcessor(ExtractLicensesSettingsBase settings, ILogger logger)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public Task ProcessResultsAsync(IEnumerable<PackageReference> packageReferences)
    {
        if (packageReferences == null) throw new ArgumentNullException(nameof(packageReferences));
        if (_settings.SuppressPrintDetails) return Task.CompletedTask;

        var packageReferencesList = packageReferences
            .OrderBy(packageReference => packageReference.Name)
            .ToList();
        
        if (!packageReferencesList.Any()) return Task.CompletedTask;

        var messageBuilder = new StringBuilder();
        
        // to make some visual space to the rest of the output
        messageBuilder.AppendLine();
        messageBuilder.AppendLine();
        messageBuilder.AppendLine();
        
        messageBuilder.AppendLine($"{"Name",-60} {"Version",-15} {"Text?",-5} {"Types",-20} URL");

        messageBuilder.AppendLine();

        foreach (var packageReference in packageReferencesList)
        {
            var name = packageReference.Name;
            var version = packageReference.Version;
            var hasText = string.IsNullOrWhiteSpace(packageReference.LicenseInformation.Text) ? "No" : "Yes";
            var types = string.Join(", ", packageReference.LicenseInformation.Types);
            var url = packageReference.LicenseInformation.Url;

            messageBuilder.AppendLine($"{name,-60} {version,-15} {hasText,-5} {types,-20} {url}");
        }
        
        _logger.LogInformation(messageBuilder.ToString());
        
        return Task.CompletedTask;
    }
}
