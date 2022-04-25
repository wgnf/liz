using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.Preparation.Contracts;
using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;
using System.Text.Json;

namespace Liz.Core.Preparation;

internal sealed class DeserializeUrlToLicenseTypeMappingPreprocessor : IPreprocessor
{
    private readonly ExtractLicensesSettingsBase _settings;
    private readonly ILogger _logger;
    private readonly IFileContentProvider _fileContentProvider;

    public DeserializeUrlToLicenseTypeMappingPreprocessor(
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
        var targetFilePath = _settings.UrlToLicenseTypeMappingFilePath;
        if (string.IsNullOrWhiteSpace(targetFilePath)) return;

        try
        {
            _logger.LogDebug($"Preprocess: getting url to license-type mappings from file {targetFilePath}...");

            var fileContent = await _fileContentProvider.GetFileContentAsync(targetFilePath);
            var urlToLicenseTypeMappingsFromFile = DeserializeUrlToLicenseTypeMappings(fileContent);
            AddToSettings(urlToLicenseTypeMappingsFromFile);
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Preparing url to license-type mappings file {targetFilePath} failed", exception);
        }
    }

    private static Dictionary<string, string> DeserializeUrlToLicenseTypeMappings(string fileContent)
    {
        var urlToLicenseMappings = JsonSerializer.Deserialize<Dictionary<string, string>>(fileContent);

        return urlToLicenseMappings?
            .Where(mapping => !string.IsNullOrWhiteSpace(mapping.Key))
            .Where(mapping => !string.IsNullOrWhiteSpace(mapping.Value))
            .ToDictionary(mapping => mapping.Key, mapping => mapping.Value) ?? new Dictionary<string, string>();
    }

    private void AddToSettings(Dictionary<string, string> urlToLicenseMappings)
    {
        // ignoring anything that fails, because duplicate keys should be skipped
        foreach (var mapping in urlToLicenseMappings)
            _ = _settings.UrlToLicenseTypeMapping.TryAdd(mapping.Key, mapping.Value);
    }
}
