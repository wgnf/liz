using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.License;
using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;
using Moq;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Liz.Core.Tests.License;

public class GetLicenseInformationFromArtifactTests
{
    [Fact]
    public async Task GetFromDownloadedPackageReference_Fails_On_Invalid_Parameters()
    {
        var (context, _) = CreateContext();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            sut.GetFromDownloadedPackageReferenceAsync(null!));
    }

    [Fact]
    public async Task GetFromDownloadedPackageReference_Does_Not_Fail_On_Missing_Nuspec_File()
    {
        const string downloadDirectoryPath = "C:/dl/some package/";

        var mockFileSystem = new MockFileSystem();

        mockFileSystem.AddDirectory(downloadDirectoryPath);
        var downloadDirectory = mockFileSystem.DirectoryInfo.FromDirectoryName(downloadDirectoryPath);

        var (context, licenseInformation) = CreateContext();
        context.Use<IFileSystem>(mockFileSystem);

        var sut = context.Build();

        var result = await sut.GetFromDownloadedPackageReferenceAsync(downloadDirectory);

        result
            .Should()
            .BeEquivalentTo(licenseInformation);
    }


    /*
     * NOTE / TODO:
     * Unfortunately testing the case with getting the nuspec file from the downloaded directory is a huge pain in the
     * ass right now, because "MockDirectoryInfo"s "EnumerateFiles" does not support "MatchCasing.CaseInsensitive" atm
     * which is used here, and I don't wanna delete it, just because some testing helpers does not support it
     *
     * So unfortunately this test case has to wait until this feature has been implemented
     */

    private static (ArrangeContext<GetLicenseInformationFromArtifact> context, LicenseInformation licenseInformation)
        CreateContext()
    {
        var context = ArrangeContext<GetLicenseInformationFromArtifact>.Create();

        var licenseInformation = new LicenseInformation { Text = "abc", Url = "abc.de" };
        licenseInformation.AddLicenseType("MIT");
        
        var licenseInformationSource = new Mock<ILicenseInformationSource>();
        licenseInformationSource
            .Setup(source => source.GetInformationAsync(It.IsAny<GetLicenseInformationContext>()))
            .Returns<GetLicenseInformationContext>(licenseInformationContext =>
            {
                licenseInformationContext.LicenseInformation.Text = licenseInformation.Text;
                licenseInformationContext.LicenseInformation.Url = licenseInformation.Url;
                
                foreach (var licenseType in licenseInformation.Types)
                    licenseInformationContext.LicenseInformation.AddLicenseType(licenseType);

                return Task.CompletedTask;
            });

        context.Use<IEnumerable<ILicenseInformationSource>>(new[] { licenseInformationSource.Object });

        return (context, licenseInformation);
    }
}
