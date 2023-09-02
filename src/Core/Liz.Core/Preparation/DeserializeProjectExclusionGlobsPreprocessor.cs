using System.Text.Json;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.Preparation.Contracts;
using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;

namespace Liz.Core.Preparation;

internal sealed class DeserializeProjectExclusionGlobsPreprocessor : IPreprocessor
{
    private readonly ExtractLicensesSettingsBase _settings;
    private readonly ILogger _logger;
    private readonly IFileContentProvider _fileContentProvider;

    public DeserializeProjectExclusionGlobsPreprocessor(
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
        var targetFilePath = _settings.ProjectExclusionGlobsFilePath;
        if (string.IsNullOrWhiteSpace(targetFilePath))
        {
            return;
        }

        try
        {
            _logger.LogDebug($"Preprocess: getting project-exclusion-globs from file {targetFilePath}...");

            var fileContent = await _fileContentProvider.GetFileContentAsync(targetFilePath).ConfigureAwait(false);
            var projectExclusionGlobsFromFile = DeserializeProjectExclusionGlobs(fileContent);
            AddToSettings(projectExclusionGlobsFromFile);
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Preparing project-exclusion-globs file {targetFilePath} failed", exception);
        }
    }

    private static IEnumerable<string> DeserializeProjectExclusionGlobs(string fileContent)
    {
        var projectExclusionGlobs = JsonSerializer.Deserialize<List<string>>(fileContent);

        return projectExclusionGlobs?.Where(entry => !string.IsNullOrWhiteSpace(entry)) ?? Enumerable.Empty<string>();
    }

    private void AddToSettings(IEnumerable<string> projectExclusionGlobsFromFile)
    {
        _settings.ProjectExclusionGlobs.AddRange(projectExclusionGlobsFromFile);
    }
}
