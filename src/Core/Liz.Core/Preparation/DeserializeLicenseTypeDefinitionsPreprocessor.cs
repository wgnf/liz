using Liz.Core.License.Sources.LicenseType;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.Preparation.Contracts;
using Liz.Core.Preparation.Contracts.Models;
using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;
using System.Text.Json;

namespace Liz.Core.Preparation;

internal sealed class DeserializeLicenseTypeDefinitionsPreprocessor : IPreprocessor
{
    private readonly ExtractLicensesSettingsBase _settings;
    private readonly ILogger _logger;
    private readonly IFileContentProvider _fileContentProvider;

    public DeserializeLicenseTypeDefinitionsPreprocessor(
        ExtractLicensesSettingsBase settings,
        ILogger logger, 
        IFileContentProvider fileContentProvider)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileContentProvider = fileContentProvider ?? throw new ArgumentNullException(nameof(fileContentProvider));
    }
    
    public async Task PreprocessAsync()
    {
        var targetFilePath = _settings.LicenseTypeDefinitionsFilePath;
        if (string.IsNullOrWhiteSpace(targetFilePath)) return;

        try
        {
            _logger.LogDebug($"Preprocess: getting license-type-definitions from file {targetFilePath}...");

            var fileContent = await _fileContentProvider.GetFileContentAsync(targetFilePath);
            var licenseTypeDefinitionsFromFile = DeserializeLicenseTypeDefinitions(fileContent);
            AddToSettings(licenseTypeDefinitionsFromFile);
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Preparing license-type-definitions file {targetFilePath} failed", exception);
        }
    }

    private static IEnumerable<LicenseTypeDefinition> DeserializeLicenseTypeDefinitions(string fileContent)
    {
        var typeDefinitions = JsonSerializer.Deserialize<List<JsonLicenseTypeDefinition>>(fileContent);

        return typeDefinitions?
            .Where(typeDefinition => !string.IsNullOrWhiteSpace(typeDefinition.LicenseType))
            .Where(typeDefinition => typeDefinition.InclusiveTextSnippets.Any())
            .Select(typeDefinition => new LicenseTypeDefinition(
                typeDefinition.LicenseType,
                typeDefinition.InclusiveTextSnippets.ToArray())
            {
                ExclusiveTextSnippets = typeDefinition.ExclusiveTextSnippets
            }) ?? Enumerable.Empty<LicenseTypeDefinition>();
    }

    private void AddToSettings(IEnumerable<LicenseTypeDefinition> licenseTypeDefinitions)
    {
        _settings.LicenseTypeDefinitions.AddRange(licenseTypeDefinitions);
    }
}
