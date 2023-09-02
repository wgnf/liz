using System.IO.Abstractions;
using System.Text.Json;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Result.Contracts;
using Liz.Core.Settings;

namespace Liz.Core.Result;

internal sealed class ExportToJsonResultProcessor : IResultProcessor
{
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;
    private readonly ExtractLicensesSettingsBase _settings;

    public ExportToJsonResultProcessor(
        ExtractLicensesSettingsBase settings,
        IFileSystem fileSystem,
        ILogger logger)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ProcessResultsAsync(IEnumerable<PackageReference> packageReferences)
    {
        if (packageReferences == null)
        {
            throw new ArgumentNullException(nameof(packageReferences));
        }

        if (string.IsNullOrWhiteSpace(_settings.ExportJsonFile))
        {
            return;
        }

        var json = GetReferencesAsJson(packageReferences);
        await WriteJsonToFile(json).ConfigureAwait(false);

        _logger.LogInformation($"Exported as JSON to '{_settings.ExportJsonFile}'!");
    }

    private static string GetReferencesAsJson(IEnumerable<PackageReference> packageReferences)
    {
        var json = JsonSerializer.Serialize(packageReferences, new JsonSerializerOptions { WriteIndented = true });
        return json;
    }

    private async Task WriteJsonToFile(string json)
    {
        var destinationFile = _fileSystem.FileInfo.FromFileName(_settings.ExportJsonFile);

        // this will basically 'overwrite' the file if it exists
        if (destinationFile.Exists)
        {
            destinationFile.Delete();
        }

        // make sure the enclosing directory(s) exist
        _fileSystem.Directory.CreateDirectory(destinationFile.Directory.FullName);
        await _fileSystem.File.WriteAllTextAsync(destinationFile.FullName, json);
    }
}
