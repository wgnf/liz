using JetBrains.Annotations;
using Liz.Core.CliTool.Contracts;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Utils.Contracts;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Threading.Tasks;

namespace Liz.Core.PackageReferences.DotnetCli;

internal sealed class DownloadPackageReferenceViaDotnetAddCli : IDownloadPackageReference
{
    private readonly IFileSystem _fileSystem;
    private readonly IProvideTemporaryDirectory _provideTemporaryDirectory;
    private readonly ICliToolExecutor _cliToolExecutor;
    private readonly ILogger _logger;

    public DownloadPackageReferenceViaDotnetAddCli(
        [NotNull] IFileSystem fileSystem,
        [NotNull] IProvideTemporaryDirectory provideTemporaryDirectory,
        [NotNull] ICliToolExecutor cliToolExecutor,
        [NotNull] ILogger logger)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _provideTemporaryDirectory = provideTemporaryDirectory 
                                     ?? throw new ArgumentNullException(nameof(provideTemporaryDirectory));
        _cliToolExecutor = cliToolExecutor ?? throw new ArgumentNullException(nameof(cliToolExecutor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<IDirectoryInfo> DownloadAsync(PackageReference packageReference)
    {
        ArgumentNullException.ThrowIfNull(packageReference);

        var targetDirectory = PrepareDownloadTargetDirectory();
        var dummyProjectFile = await PrepareDummyProjectFileAsync(targetDirectory, packageReference);
        
        await DownloadViaDotnetAddCliAsync(packageReference, dummyProjectFile, targetDirectory);

        var packageReferenceDownloadDirectory = GetPackageReferenceDownloadDirectory(packageReference, targetDirectory);
        return packageReferenceDownloadDirectory;
    }

    private string PrepareDownloadTargetDirectory()
    {
        var temporaryDirectory = _provideTemporaryDirectory.Get();
        var temporaryDownloadDirectory = _fileSystem.Path.Combine(temporaryDirectory.FullName, "download");

        // make sure that the target directory is created
        _fileSystem.Directory.CreateDirectory(temporaryDownloadDirectory);
        return temporaryDownloadDirectory;
    }

    private async Task<string> PrepareDummyProjectFileAsync(string targetDirectory, PackageReference packageReference)
    {
        var dummyProjectFile = _fileSystem.Path.Combine(targetDirectory, "Dummy.csproj");

        // we do not need to delete the file, because it'll overwrite
        var stream = _fileSystem.File.Create(dummyProjectFile);
        await WriteDummyDataToDummyProjectFileAsync(packageReference, stream);

        return dummyProjectFile;
    }

    private async static Task WriteDummyDataToDummyProjectFileAsync(PackageReference packageReference, Stream stream)
    {
        await using var streamWriter = new StreamWriter(stream);

        var dummyProjectContentBuilder = new StringBuilder();
        dummyProjectContentBuilder.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");

        // this is essential or else the dependency graph cannot be created
        dummyProjectContentBuilder.AppendLine(
            $"<PropertyGroup> <TargetFramework>{packageReference.TargetFramework}</TargetFramework> </PropertyGroup>");
        dummyProjectContentBuilder.AppendLine("</Project>");

        await streamWriter.WriteAsync(dummyProjectContentBuilder);
    }

    private async Task DownloadViaDotnetAddCliAsync(
        PackageReference packageReference, 
        string dummyProjectFile,
        string targetDirectory)
    {
        try
        {
            // c.f.: https://github.com/dotnet/docs/blob/main/docs/core/tools/dotnet-add-package.md#dotnet-add-package
            var arguments = $"add \"{dummyProjectFile}\" package {packageReference.Name} -f {packageReference.TargetFramework} " +
                            $"-v {packageReference.Version} --package-directory \"{targetDirectory}\"";
            await _cliToolExecutor.ExecuteAsync("dotnet", arguments);
        }
        catch (Exception ex)
        {
            _logger.LogDebug($"{packageReference} could not be downloaded. Possible causes:\n" +
                             "- A NuGet-Source is missing\n" +
                             "- The reference is caused by a Project-Reference, but is not released as a NuGet-Package\n" +
                             "- The NuGet-Package cannot be downloaded because of any other issue", ex);
        }
    }
    
    private IDirectoryInfo GetPackageReferenceDownloadDirectory(PackageReference packageReference, string targetDirectory)
    {
        var packageReferenceDownloadDirectory = _fileSystem.Path.Combine(
            targetDirectory,
            packageReference.Name.ToLower(), 
            packageReference.Version.ToLower());
        
        var packageReferenceDownloadDirectoryInfo = _fileSystem.DirectoryInfo.FromDirectoryName(packageReferenceDownloadDirectory);
        return packageReferenceDownloadDirectoryInfo;
    }
}
