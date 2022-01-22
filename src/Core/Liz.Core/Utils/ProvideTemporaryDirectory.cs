using JetBrains.Annotations;
using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;
using System;
using System.IO.Abstractions;

namespace Liz.Core.Utils;

internal sealed class ProvideTemporaryDirectory : IProvideTemporaryDirectory
{
    private readonly ExtractLicensesSettings _settings;
    private readonly IFileSystem _fileSystem;

    public ProvideTemporaryDirectory(
        [NotNull] ExtractLicensesSettings settings,
        [NotNull] IFileSystem fileSystem)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }
    
    public IDirectoryInfo Get()
    {
        var targetFileDirectory = _fileSystem.Path.GetDirectoryName(_settings.TargetFile);
        var lizTemporaryDirectory = _fileSystem.Path.Combine(targetFileDirectory, ".liz_tmp");
        var lizTemporaryDirectoryInfo = _fileSystem.DirectoryInfo.FromDirectoryName(lizTemporaryDirectory);

        return lizTemporaryDirectoryInfo;
    }
}
