using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.PackageReferences;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Utils.Contracts;
using Liz.Core.Utils.Models;
using Moq;
using System.IO.Abstractions;
using Xunit;

namespace Liz.Core.Tests.PackageReferences;

public class EnrichPackageReferenceWithArtifactDirectoryTests
{
    [Fact]
    public async Task Fails_On_Invalid_Parameters()
    {
        var context = ArrangeContext<EnrichPackageReferenceWithArtifactDirectory>.Create();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.EnrichAsync(null!));
    }

    [Fact]
    public async Task Not_Enrich_When_Not_Found()
    {
        var context = ArrangeContext<EnrichPackageReferenceWithArtifactDirectory>.Create();
        var sut = context.Build();

        var packageReference = new PackageReference("Test", "net6.0", "1.2.3");

        context
            .For<IFindPackageReferenceArtifact>()
            .Setup(finder => finder.TryGetArtifactAsync(packageReference))
            .ReturnsAsync(Optional<IDirectoryInfo>.Failure);

        await sut.EnrichAsync(packageReference);

        packageReference
            .ArtifactDirectory
            .Should()
            .BeNull();
    }
    
    [Fact]
    public async Task Enrich_When_Found()
    {
        var context = ArrangeContext<EnrichPackageReferenceWithArtifactDirectory>.Create();
        var sut = context.Build();

        var packageReference = new PackageReference("Test", "net6.0", "1.2.3");
        
        const string artifactDirectory = "C:/some/path";
        var artifactDirectoryInfo = new Mock<IDirectoryInfo>();
        artifactDirectoryInfo
            .SetupGet(directoryInfo => directoryInfo.FullName)
            .Returns(artifactDirectory);

        context
            .For<IFindPackageReferenceArtifact>()
            .Setup(finder => finder.TryGetArtifactAsync(packageReference))
            .ReturnsAsync(Optional<IDirectoryInfo>.Success(artifactDirectoryInfo.Object));

        await sut.EnrichAsync(packageReference);

        packageReference
            .ArtifactDirectory
            .Should()
            .Be(artifactDirectory);
    }
}
