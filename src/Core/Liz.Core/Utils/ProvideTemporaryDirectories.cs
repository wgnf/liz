using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;
using System.IO.Abstractions;

namespace Liz.Core.Utils;

internal sealed class ProvideTemporaryDirectories : IProvideTemporaryDirectories
{
    private readonly ExtractLicensesSettingsBase _settings;
    private readonly IFileSystem _fileSystem;

    public ProvideTemporaryDirectories(ExtractLicensesSettingsBase settings, IFileSystem fileSystem)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }
    
    public IDirectoryInfo GetRootDirectory()
    {
        var targetFileDirectory = _fileSystem.Path.GetDirectoryName(_settings.GetTargetFile());
        if (targetFileDirectory == null)
        {
            throw new InvalidOperationException($"{nameof(targetFileDirectory)} cannot be null.");
        }
        
        var lizTemporaryDirectory = _fileSystem.Path.Combine(targetFileDirectory, ".liz_tmp");
        var lizTemporaryDirectoryInfo = _fileSystem.DirectoryInfo.New(lizTemporaryDirectory);

        return lizTemporaryDirectoryInfo;
    }

    public IDirectoryInfo GetDownloadDirectory()
    {
        var rootDirectory = GetRootDirectory();
        var temporaryDownloadDirectoryName = _fileSystem.Path.Combine(rootDirectory.FullName, "download");
        var temporaryDownloadDirectory = _fileSystem.DirectoryInfo.New(temporaryDownloadDirectoryName);

        return temporaryDownloadDirectory;
    }
}
