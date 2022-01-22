using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.License.Contracts.Models;
using Liz.Core.License.Sources;
using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace Liz.Core.Tests.License.Source;

public class LicenseElementLicenseInformationSourceTests
{
    [Fact]
    public void Order_Is_At_The_Start()
    {
        var context = ArrangeContext<LicenseElementLicenseInformationSource>.Create();
        var sut = context.Build();

        sut
            .Order
            .Should()
            .BeLessThan(5);
    }

    [Fact]
    public async Task GetInformation_Fails_On_Invalid_Parameters()
    {
        var context = ArrangeContext<LicenseElementLicenseInformationSource>.Create();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetInformationAsync(null!));
    }

    [Fact]
    public async Task GetInformation_Does_Nothing_When_No_Nuspec_File_Is_Defined()
    {
        var context = ArrangeContext<LicenseElementLicenseInformationSource>.Create();
        var sut = context.Build();

        var licenseContext = new GetLicenseInformationContext { NugetSpecificationFileXml = null };

        await sut.GetInformationAsync(licenseContext);
        
        licenseContext
            .LicenseInformation
            .Text
            .Should()
            .BeNullOrEmpty();
        licenseContext
            .LicenseInformation
            .Type
            .Should()
            .BeNullOrEmpty();
    }

    [Fact]
    public async Task GetInformation_Does_Nothing_When_Nuspec_Does_Not_Contain_License_Element()
    {
        var context = ArrangeContext<LicenseElementLicenseInformationSource>.Create();
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
            .Text
            .Should()
            .BeNullOrEmpty();
        licenseContext
            .LicenseInformation
            .Type
            .Should()
            .BeNullOrEmpty();
    }

    [Fact]
    public async Task GetInformation_Is_Robust_Against_License_Element_Without_Attribute()
    {
        var context = ArrangeContext<LicenseElementLicenseInformationSource>.Create();
        var sut = context.Build();

        const string content = @"
<package xmlns=""http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"">
    <metadata>
        <license>...</license>
    </metadata>
</package>";
        var nuspecXml = XDocument.Parse(content);
        
        var licenseContext = new GetLicenseInformationContext { NugetSpecificationFileXml = nuspecXml };

        await sut.GetInformationAsync(licenseContext);
        
        licenseContext
            .LicenseInformation
            .Text
            .Should()
            .BeNullOrEmpty();
        licenseContext
            .LicenseInformation
            .Type
            .Should()
            .BeNullOrEmpty();
    }

    [Fact]
    public async Task GetInformation_Handles_License_Element_With_Expression_Attribute()
    {
        var context = ArrangeContext<LicenseElementLicenseInformationSource>.Create();
        var sut = context.Build();

        const string content = @"
<package xmlns=""http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"">
    <metadata>
        <license type=""expression"">MIT</license>
    </metadata>
</package>";
        var nuspecXml = XDocument.Parse(content);
        
        var licenseContext = new GetLicenseInformationContext { NugetSpecificationFileXml = nuspecXml };

        await sut.GetInformationAsync(licenseContext);

        licenseContext
            .LicenseInformation
            .Type
            .Should()
            .Be("MIT");
    }

    [Fact]
    public async Task
        GetInformation_Handles_License_Element_With_File_Attribute_And_Is_Robust_Against_Not_Existing_File()
    {
        const string artifactDirectoryPath = "C:/dl/some package/";
        var mockFileSystem = new MockFileSystem();
        
        mockFileSystem.AddDirectory(artifactDirectoryPath);
        var artifactDirectory = mockFileSystem.DirectoryInfo.FromDirectoryName(artifactDirectoryPath);
        
        var context = ArrangeContext<LicenseElementLicenseInformationSource>.Create();
        context.Use<IFileSystem>(mockFileSystem);

        var sut = context.Build();

        const string content = @"
<package xmlns=""http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"">
    <metadata>
        <license type=""file"">licenses/LICENSE.md</license>
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
            .Text
            .Should()
            .BeNullOrEmpty();
    }

    [Fact]
    public async Task GetInformation_Handles_License_Element_With_File_Attribute()
    {
        const string artifactDirectoryPath = "C:/dl/some package/";
        const string licenseFileContent = "abc";
        
        var mockFileSystem = new MockFileSystem();
        
        mockFileSystem.AddDirectory(artifactDirectoryPath);
        var artifactDirectory = mockFileSystem.DirectoryInfo.FromDirectoryName(artifactDirectoryPath);
        
        // NOTE: matches the (relative) file path in the XML
        mockFileSystem.AddFile($"{artifactDirectory}/licenses/LICENSE.md", new MockFileData(licenseFileContent));
        
        var context = ArrangeContext<LicenseElementLicenseInformationSource>.Create();
        context.Use<IFileSystem>(mockFileSystem);

        var sut = context.Build();

        const string content = @"
<package xmlns=""http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"">
    <metadata>
        <license type=""file"">licenses/LICENSE.md</license>
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
            .Text
            .Should()
            .Be(licenseFileContent);
    }
}
