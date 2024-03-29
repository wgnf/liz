﻿using Liz.Core.Settings;
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
        var lizTemporaryDirectory = _fileSystem.Path.Combine(targetFileDirectory, ".liz_tmp");
        var lizTemporaryDirectoryInfo = _fileSystem.DirectoryInfo.FromDirectoryName(lizTemporaryDirectory);

        return lizTemporaryDirectoryInfo;
    }

    public IDirectoryInfo GetDownloadDirectory()
    {
        var rootDirectory = GetRootDirectory();
        var temporaryDownloadDirectoryName = _fileSystem.Path.Combine(rootDirectory.FullName, "download");
        var temporaryDownloadDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(temporaryDownloadDirectoryName);

        return temporaryDownloadDirectory;
    }
}
