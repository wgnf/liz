using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.License.Contracts.Models;
using Liz.Core.License.Sources;
using Liz.Core.Utils.Contracts.Wrappers;
using Moq;
using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace Liz.Core.Tests.License.Source;

public class LicenseUrlElementLicenseInformationSourceTests
{
    [Fact]
    public void Order_Is_Somewhere_In_Between()
    {
        var context = ArrangeContext<LicenseUrlElementLicenseInformationSource>.Create();
        var sut = context.Build();

        sut
            .Order
            .Should()
            .BeGreaterThan(0);
    }
    
    [Fact]
    public async Task GetInformation_Fails_On_Invalid_Parameters()
    {
        var context = ArrangeContext<LicenseUrlElementLicenseInformationSource>.Create();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetInformationAsync(null!));
    }
    
    [Fact]
    public async Task GetInformation_Does_Nothing_When_No_Nuspec_File_Is_Defined()
    {
        var context = ArrangeContext<LicenseUrlElementLicenseInformationSource>.Create();
        var sut = context.Build();

        var licenseContext = new GetLicenseInformationContext { NugetSpecificationFileXml = null };

        await sut.GetInformationAsync(licenseContext);
        
        licenseContext
            .LicenseInformation
            .Url
            .Should()
            .BeNullOrEmpty();
        licenseContext
            .LicenseInformation
            .Text
            .Should()
            .BeNullOrEmpty();
    }
    
    [Fact]
    public async Task GetInformation_Does_Nothing_When_Nuspec_Does_Not_Contain_License_Url_Element()
    {
        var context = ArrangeContext<LicenseUrlElementLicenseInformationSource>.Create();
        var sut = context.Build();

        const string content = @"
<package xmlns=""http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"">
    <metadata>
    </metadata>
</package>";
        var nuspecXml = XDocument.Parse(content);
        
        var licenseContext = new GetLicenseInformationContext { NugetSpecificationFileXml = nuspecXml };

        await sut.GetInformationAsync(licenseContext);
        
        licenseContext
            .LicenseInformation
            .Url
            .Should()
            .BeNullOrEmpty();
        licenseContext
            .LicenseInformation
            .Text
            .Should()
            .BeNullOrEmpty();
    }
    
    [Fact]
    public async Task GetInformation_Aborts_Early_When_License_Text_Has_Already_Been_Set()
    {
        var context = ArrangeContext<LicenseUrlElementLicenseInformationSource>.Create();
        var sut = context.Build();

        const string content = @"
<package xmlns=""http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"">
    <metadata>
        <licenseUrl>https://example.org/</licenseUrl>
    </metadata>
</package>";
        var nuspecXml = XDocument.Parse(content);
        
        var licenseContext = new GetLicenseInformationContext
        {
            NugetSpecificationFileXml = nuspecXml,
            LicenseInformation =
            {
                Text = "abc"
            }
        };

        await sut.GetInformationAsync(licenseContext);
        
        licenseContext
            .LicenseInformation
            .Url
            .Should()
            .Be("https://example.org/");
        licenseContext
            .LicenseInformation
            .Text
            .Should()
            .Be("abc");
    }
    
    [Fact]
    public async Task GetInformation_Handles_File_Url_And_Is_Robust_Against_Not_Existing_File()
    {
        const string artifactDirectoryPath = "C:/dl/some package/";
        
        var mockFileSystem = new MockFileSystem();
        
        mockFileSystem.AddDirectory(artifactDirectoryPath);
        var artifactDirectory = mockFileSystem.DirectoryInfo.FromDirectoryName(artifactDirectoryPath);

        var context = ArrangeContext<LicenseUrlElementLicenseInformationSource>.Create();
        context.Use<IFileSystem>(mockFileSystem);
        
        var sut = context.Build();

        const string content = @"
<package xmlns=""http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"">
    <metadata>
        <licenseUrl>licenses/LICENSE.md</licenseUrl>
    </metadata>
</package>";
        var nuspecXml = XDocument.Parse(content);

        var licenseContext = new GetLicenseInformationContext
        {
            NugetSpecificationFileXml = nuspecXml,
            ArtifactDirectory = artifactDirectory
        };

        await sut.GetInformationAsync(licenseContext);
        
        licenseContext
            .LicenseInformation
            .Url
            .Should()
            .Be("licenses/LICENSE.md");
        licenseContext
            .LicenseInformation
            .Text
            .Should()
            .BeNullOrEmpty();
    }
    
    [Fact]
    public async Task GetInformation_Handles_File_Url()
    {
        const string artifactDirectoryPath = "C:/dl/some package/";
        const string licenseFileContent = "abc";
        
        var mockFileSystem = new MockFileSystem();
        
        mockFileSystem.AddDirectory(artifactDirectoryPath);
        var artifactDirectory = mockFileSystem.DirectoryInfo.FromDirectoryName(artifactDirectoryPath);
        
        // NOTE: matches the (relative) file path in the XML
        mockFileSystem.AddFile($"{artifactDirectory}/licenses/LICENSE.md", new MockFileData(licenseFileContent));
        
        var context = ArrangeContext<LicenseUrlElementLicenseInformationSource>.Create();
        context.Use<IFileSystem>(mockFileSystem);
        
        var sut = context.Build();

        const string content = @"
<package xmlns=""http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"">
    <metadata>
        <licenseUrl>licenses/LICENSE.md</licenseUrl>
    </metadata>
</package>";
        var nuspecXml = XDocument.Parse(content);

        var licenseContext = new GetLicenseInformationContext
        {
            NugetSpecificationFileXml = nuspecXml,
            ArtifactDirectory = artifactDirectory
        };

        await sut.GetInformationAsync(licenseContext);
        
        licenseContext
            .LicenseInformation
            .Url
            .Should()
            .Be("licenses/LICENSE.md");
        licenseContext
            .LicenseInformation
            .Text
            .Should()
            .Be(licenseFileContent);
    }
    
    [Fact]
    public async Task GetInformation_Handles_Web_Url_And_Is_Robust_Against_Web_Request_Fail()
    {
        var context = ArrangeContext<LicenseUrlElementLicenseInformationSource>.Create();
        var sut = context.Build();

        context
            .For<IHttpClient>()
            .Setup(httpClient => httpClient.GetStringAsync("https://example.org/"))
            .Throws(new WebException());

        const string content = @"
<package xmlns=""http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"">
    <metadata>
        <licenseUrl>https://example.org/</licenseUrl>
    </metadata>
</package>";
        var nuspecXml = XDocument.Parse(content);

        var licenseContext = new GetLicenseInformationContext { NugetSpecificationFileXml = nuspecXml };

        await sut.GetInformationAsync(licenseContext);
        
        licenseContext
            .LicenseInformation
            .Url
            .Should()
            .Be("https://example.org/");
        licenseContext
            .LicenseInformation
            .Text
            .Should()
            .BeNullOrEmpty();
    }
    
    [Fact]
    public async Task GetInformation_Handles_Web_Url()
    {
        const string webContent = "abc";

        var context = ArrangeContext<LicenseUrlElementLicenseInformationSource>.Create();
        var sut = context.Build();

        context
            .For<IHttpClient>()
            .Setup(httpClient => httpClient.GetStringAsync("https://example.org/"))
            .ReturnsAsync(webContent);

        const string content = @"
<package xmlns=""http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"">
    <metadata>
        <licenseUrl>https://example.org/</licenseUrl>
    </metadata>
</package>";
        var nuspecXml = XDocument.Parse(content);

        var licenseContext = new GetLicenseInformationContext { NugetSpecificationFileXml = nuspecXml };

        await sut.GetInformationAsync(licenseContext);
        
        licenseContext
            .LicenseInformation
            .Url
            .Should()
            .Be("https://example.org/");
        licenseContext
            .LicenseInformation
            .Text
            .Should()
            .Be(webContent);
    }
}
