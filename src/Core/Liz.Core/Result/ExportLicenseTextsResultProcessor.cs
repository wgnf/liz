using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Result.Contracts;
using Liz.Core.Settings;
using System.IO.Abstractions;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;

namespace Liz.Core.Result;

internal sealed class ExportLicenseTextsResultProcessor : IResultProcessor
{
    private readonly ExtractLicensesSettingsBase _settings;
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;

    public ExportLicenseTextsResultProcessor(
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

        if (string.IsNullOrWhiteSpace(_settings.ExportLicenseTextsDirectory))
        {
            return;
        }

        var directory = EnsureDirectoryExists();
        await WriteLicenseTextsToFilesAsync(directory, packageReferences).ConfigureAwait(false);
        
        _logger.LogInformation($"Exported license-texts to '{_settings.ExportLicenseTextsDirectory}'!");
    }

    private IDirectoryInfo EnsureDirectoryExists()
    {
        var directory = _fileSystem.DirectoryInfo.FromDirectoryName(_settings.ExportLicenseTextsDirectory);
        directory.Create();
        
        return directory;
    }
    
    private async Task WriteLicenseTextsToFilesAsync(IFileSystemInfo directory, IEnumerable<PackageReference> packageReferences)
    {
        foreach (var packageReference in packageReferences)
        {
            await WriteLicenseTextToFileAsync(directory, packageReference).ConfigureAwait(false);
        }
    }

    private async Task WriteLicenseTextToFileAsync(IFileSystemInfo directory, PackageReference packageReference)
    {
        var licenseText = packageReference.LicenseInformation.Text;
        if (string.IsNullOrWhiteSpace(licenseText))
        {
            return;
        }

        var fileExtension = licenseText.Contains("<!DOCTYPE html>")
            ? "html"
            : "txt";

        var fileName = $"{packageReference.Name}-{packageReference.Version}.{fileExtension}";
        var fullFileName = _fileSystem.Path.Combine(directory.FullName, fileName);

        // NOTE: the default-behavior is overwriting any file that already exists!
        await _fileSystem.File.WriteAllTextAsync(fullFileName, licenseText).ConfigureAwait(false);
    }
}
