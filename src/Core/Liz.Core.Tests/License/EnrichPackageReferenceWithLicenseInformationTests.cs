using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.License;
using Liz.Core.PackageReferences;
using Moq;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
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
    public async Task Enrich_Sets_License_Information_From_Downloaded_Artifact()
    {
        var context = ArrangeContext<EnrichPackageReferenceWithLicenseInformation>.Create();
        var sut = context.Build();

        var packageReference = new PackageReference("Something", "net472", "1.0.0");
        
        var downloadedDirectory = Mock.Of<IDirectoryInfo>();
        context
            .For<IDownloadPackageReference>()
            .Setup(download => download.DownloadAsync(packageReference))
            .ReturnsAsync(downloadedDirectory);

        var licenseInformation = new LicenseInformation();
        context
            .For<IGetLicenseInformationFromArtifact>()
            .Setup(getLicenseInformation =>
                getLicenseInformation.GetFromDownloadedPackageReferenceAsync(downloadedDirectory))
            .ReturnsAsync(licenseInformation);

        await sut.EnrichAsync(packageReference);

        packageReference
            .LicenseInformation
            .Should()
            .Be(licenseInformation);
    }
}
