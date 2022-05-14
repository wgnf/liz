using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.Preparation.Contracts;
using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;
using System.Text.Json;

namespace Liz.Core.Preparation;

internal sealed class DeserializeLicenseTypeBlacklistPreprocessor : IPreprocessor
{
    private readonly ExtractLicensesSettingsBase _settings;
    private readonly ILogger _logger;
    private readonly IFileContentProvider _fileContentProvider;

    public DeserializeLicenseTypeBlacklistPreprocessor(
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
        var targetFilePath = _settings.LicenseTypeBlacklistFilePath;
        if (string.IsNullOrWhiteSpace(targetFilePath)) return;

        try
        {
            _logger.LogDebug($"Preprocess: getting license-type-blacklist from file {targetFilePath}...");

            var fileContent = await _fileContentProvider.GetFileContentAsync(targetFilePath).ConfigureAwait(false);
            var licenseTypeBlacklistFromFile = DeserializeLicenseTypeBlacklist(fileContent);
            AddToSettings(licenseTypeBlacklistFromFile);
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Preparing license-type-blacklist file {targetFilePath} failed", exception);
        }
    }

    private static IEnumerable<string> DeserializeLicenseTypeBlacklist(string fileContent)
    {
        var licenseTypeBlacklist = JsonSerializer.Deserialize<List<string>>(fileContent);

        return licenseTypeBlacklist?.Where(entry => !string.IsNullOrWhiteSpace(entry)) ?? Enumerable.Empty<string>();
    }

    private void AddToSettings(IEnumerable<string> licenseTypeBlacklistFromFile)
    {
        _settings.LicenseTypeBlacklist.AddRange(licenseTypeBlacklistFromFile);
    }
}
