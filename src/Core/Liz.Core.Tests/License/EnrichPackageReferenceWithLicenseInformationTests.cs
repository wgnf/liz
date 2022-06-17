using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.License;
using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;
using Liz.Core.PackageReferences.Contracts.Models;
using Moq;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Liz.Core.Tests.License;

public class EnrichPackageReferenceWithLicenseInformationTests
{
    [Fact]
    public async Task Enrich_Fails_When_Parameters_Invalid()
    {
        var context = ArrangeContext<EnrichPackageReferenceWithLicenseInformation>.Create();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.EnrichAsync(null!));
    }

    [Fact]
    public async Task Enrich_Sets_License_Information_From_Downloaded_Artifact_When_Artifact_Could_Be_Found()
    {
        var context = ArrangeContext<EnrichPackageReferenceWithLicenseInformation>.Create();
        context.Use<IFileSystem>(new MockFileSystem());
        
        var sut = context.Build();

        const string artifactDirectory = "C:/some/directory";
        var packageReference = new PackageReference("Something", "net472", "1.0.0")
        {
            ArtifactDirectory = artifactDirectory
        };

        var licenseInformation = new LicenseInformation { Text = "abc", Url = "abc.de" };
        licenseInformation.AddLicenseType("MIT");
        
        context
            .For<IGetLicenseInformationFromArtifact>()
            .Setup(getLicenseInformation =>
                getLicenseInformation.GetFromDownloadedPackageReferenceAsync(It.IsAny<IDirectoryInfo>()))
            .ReturnsAsync(licenseInformation);

        await sut.EnrichAsync(packageReference);

        packageReference
            .LicenseInformation
            .Should()
            .Be(licenseInformation);
    }

    [Fact]
    public async Task Enrich_Does_Not_Do_Anything_When_Artifact_Could_Not_Be_Found()
    {
        var context = ArrangeContext<EnrichPackageReferenceWithLicenseInformation>.Create();
        context.Use<IFileSystem>(new MockFileSystem());
        
        var sut = context.Build();

        var packageReference = new PackageReference("Something", "net472", "1.0.0")
        {
            ArtifactDirectory = null
        };

        await sut.EnrichAsync(packageReference);

        context
            .For<IGetLicenseInformationFromArtifact>()
            .Verify(
                getLicenseInformation =>
                    getLicenseInformation.GetFromDownloadedPackageReferenceAsync(It.IsAny<IDirectoryInfo>()), Times.Never);
    }
}
