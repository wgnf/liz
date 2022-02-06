using JetBrains.Annotations;
using Liz.Core.PackageReferences.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Utils.Contracts;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace Liz.Core.PackageReferences;

internal sealed class GetDownloadedPackageReferenceArtifact : IGetDownloadedPackageReferenceArtifact
{
    private readonly IProvideTemporaryDirectories _provideTemporaryDirectories;
    private readonly IFileSystem _fileSystem;

    public GetDownloadedPackageReferenceArtifact(
        [NotNull] IProvideTemporaryDirectories provideTemporaryDirectories,
        [NotNull] IFileSystem fileSystem)
    {
        _provideTemporaryDirectories = provideTemporaryDirectories ?? throw new ArgumentNullException(nameof(provideTemporaryDirectories));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }
    
    public bool TryGetFor(PackageReference packageReference, out IDirectoryInfo packageReferenceDownloadDirectory)
    {
        ArgumentNullException.ThrowIfNull(packageReference);
        
        packageReferenceDownloadDirectory = null;

        var downloadDirectory = _provideTemporaryDirectories.GetDownloadDirectory();
        var downloadDirectoryCandidates = GetDownloadDirectoryCandidates(downloadDirectory, packageReference);
        packageReferenceDownloadDirectory = downloadDirectoryCandidates.FirstOrDefault();

        return packageReferenceDownloadDirectory != null;
    }

    private IEnumerable<IDirectoryInfo> GetDownloadDirectoryCandidates(
        IFileSystemInfo downloadDirectory,
        PackageReference packageReference)
    {
        var downloadDirectoryCandidates = new List<IDirectoryInfo>();
        var downloadDirectoryName = downloadDirectory.FullName;

        GetDownloadDirectoryCandidateForDotnet(packageReference, downloadDirectoryName, downloadDirectoryCandidates);
        GetDownloadDirectoryCandidateForNuget(packageReference, downloadDirectoryName, downloadDirectoryCandidates);

        return downloadDirectoryCandidates;
    }

    private void GetDownloadDirectoryCandidateForDotnet(
        PackageReference packageReference, 
        string downloadDirectoryName,
        ICollection<IDirectoryInfo> downloadDirectoryCandidates)
    {
        var dotnetStyleCandidateName = _fileSystem
            .Path
            .Combine(
                downloadDirectoryName,
                "dotnet-dl",
                packageReference.Name.ToLower(),
                packageReference.Version.ToLower());
        var dotnetStyleCandidate = _fileSystem.DirectoryInfo.FromDirectoryName(dotnetStyleCandidateName);
        
        if (dotnetStyleCandidate.Exists)
            downloadDirectoryCandidates.Add(dotnetStyleCandidate);
    }
    
    private void GetDownloadDirectoryCandidateForNuget(
        PackageReference packageReference, 
        string downloadDirectoryName,
        ICollection<IDirectoryInfo> downloadDirectoryCandidates)
    {
        var nugetStyleCandidateName = _fileSystem
            .Path
            .Combine(
                downloadDirectoryName,
                "nuget-dl",
                $"{packageReference.Name}.{packageReference.Version}");
        var nugetStyleCandidate = _fileSystem.DirectoryInfo.FromDirectoryName(nugetStyleCandidateName);
        
        if (nugetStyleCandidate.Exists)
            downloadDirectoryCandidates.Add(nugetStyleCandidate);
    }
}
