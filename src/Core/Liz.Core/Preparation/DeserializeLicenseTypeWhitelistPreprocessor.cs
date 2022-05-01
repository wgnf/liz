using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.Preparation.Contracts;
using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;
using System.Text.Json;

namespace Liz.Core.Preparation;

internal sealed class DeserializeLicenseTypeWhitelistPreprocessor : IPreprocessor
{
    private readonly ExtractLicensesSettingsBase _settings;
    private readonly ILogger _logger;
    private readonly IFileContentProvider _fileContentProvider;

    public DeserializeLicenseTypeWhitelistPreprocessor(
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
        var targetFilePath = _settings.LicenseTypeWhitelistFilePath;
        if (string.IsNullOrWhiteSpace(targetFilePath)) return;

        try
        {
            _logger.LogDebug($"Preprocess: getting license-type-whitelist from file {targetFilePath}...");

            var fileContent = await _fileContentProvider.GetFileContentAsync(targetFilePath);
            var licenseTypeWhitelistFromFile = DeserializeLicenseTypeWhitelist(fileContent);
            AddToSettings(licenseTypeWhitelistFromFile);
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Preparing license-type-whitelist file {targetFilePath} failed", exception);
        }
    }

    private static IEnumerable<string> DeserializeLicenseTypeWhitelist(string fileContent)
    {
        var licenseTypeWhitelist = JsonSerializer.Deserialize<List<string>>(fileContent);

        return licenseTypeWhitelist?.Where(entry => !string.IsNullOrWhiteSpace(entry)) ?? Enumerable.Empty<string>();
    }
    
    private void AddToSettings(IEnumerable<string> licenseTypeWhitelistFromFile)
    {
        _settings.LicenseTypeWhitelist.AddRange(licenseTypeWhitelistFromFile);
    }
}
