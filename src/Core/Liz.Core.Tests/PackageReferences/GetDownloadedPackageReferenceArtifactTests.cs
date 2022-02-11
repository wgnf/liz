using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.PackageReferences;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Utils.Contracts;
using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Liz.Core.Tests.PackageReferences;

public class GetDownloadedPackageReferenceArtifactTests
{
    [Fact]
    public void TryGetFor_Fails_On_Invalid_Parameters()
    {
        var context = ArrangeContext<GetDownloadedPackageReferenceArtifact>.Create();
        var sut = context.Build();

        Assert.Throws<ArgumentNullException>(() => sut.TryGetFor(null!, out _));
    }

    [Fact]
    public void TryGetFor_Returns_Nothing_When_No_Candidate_Found()
    {
        var mockFileSystem = new MockFileSystem();
        const string dotnetDownloadDirectoryName = "./download/something";
        mockFileSystem.AddDirectory(dotnetDownloadDirectoryName);

        var downloadDirectory = mockFileSystem.DirectoryInfo.FromDirectoryName("./download");

        var context = ArrangeContext<GetDownloadedPackageReferenceArtifact>.Create();
        context.Use<IFileSystem>(mockFileSystem);

        context
            .For<IProvideTemporaryDirectories>()
            .Setup(provideTempDirectories => provideTempDirectories.GetDownloadDirectory())
            .Returns(downloadDirectory);
        
        var sut = context.Build();

        var success = sut.TryGetFor(new PackageReference("p", "p", "1.0.0"), out _);

        success
            .Should()
            .BeFalse();
    }

    [Fact]
    public void TryGetFor_Returns_Dotnet_Style_Candidate()
    {
        var packageReference = new PackageReference("Something", "net6.0", "1.0.0-Preview1");
        
        var mockFileSystem = new MockFileSystem();
        var dotnetDownloadDirectoryName = $"./download/dotnet-dl/{packageReference.Name.ToLower()}/{packageReference.Version.ToLower()}";
        mockFileSystem.AddDirectory(dotnetDownloadDirectoryName);

        var downloadDirectory = mockFileSystem.DirectoryInfo.FromDirectoryName("./download");

        var context = ArrangeContext<GetDownloadedPackageReferenceArtifact>.Create();
        context.Use<IFileSystem>(mockFileSystem);

        context
            .For<IProvideTemporaryDirectories>()
            .Setup(provideTempDirectories => provideTempDirectories.GetDownloadDirectory())
            .Returns(downloadDirectory);
        
        var sut = context.Build();

        var success = sut.TryGetFor(packageReference, out var result);
        Assert.NotNull(result);

        success
            .Should()
            .BeTrue();

        result?
            .FullName
            .Should()
            .ContainAll(packageReference.Name.ToLower(), packageReference.Version.ToLower());
    }
    
    [Fact]
    public void TryGetFor_Returns_Nuget_Style_Candidate()
    {
        var packageReference = new PackageReference("Something", "net6.0", "1.0.0-Preview1");
        
        var mockFileSystem = new MockFileSystem();
        var nugetDownloadDirectory = $"./download/nuget-dl/{packageReference.Name}.{packageReference.Version}";
        mockFileSystem.AddDirectory(nugetDownloadDirectory);

        var downloadDirectory = mockFileSystem.DirectoryInfo.FromDirectoryName("./download");

        var context = ArrangeContext<GetDownloadedPackageReferenceArtifact>.Create();
        context.Use<IFileSystem>(mockFileSystem);

        context
            .For<IProvideTemporaryDirectories>()
            .Setup(provideTempDirectories => provideTempDirectories.GetDownloadDirectory())
            .Returns(downloadDirectory);
        
        var sut = context.Build();

        var success = sut.TryGetFor(packageReference, out var result);
        Assert.NotNull(result);

        success
            .Should()
            .BeTrue();

        result?
            .FullName
            .Should()
            .ContainAll(packageReference.Name, packageReference.Version);
    }
}
