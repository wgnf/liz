using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.PackageReferences.NuGetCli;
using Moq;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Liz.Core.Tests.PackageReferences.NuGetCli;

public class ParsePackagesConfigFileTests
{
    private const string ExamplePackagesConfigContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<packages>
  <package id=""AutoMapper"" version=""4.0.4"" targetFramework=""net40"" />
  <package id=""Microsoft.Bcl"" version=""1.1.9"" targetFramework=""net40"" />
  <package id=""Microsoft.Bcl.Build"" version=""1.0.14"" targetFramework=""net40"" />
</packages>";
    
    [Fact]
    public void GetPackageReferences_Fails_On_Invalid_Parameters()
    {
        var context = ArrangeContext<ParsePackagesConfigFile>.Create();
        var sut = context.Build();

        Assert.Throws<ArgumentNullException>(() => sut.GetPackageReferences(null!));

        var packagesConfigFileMock = new Mock<IFileInfo>();
        packagesConfigFileMock
            .SetupGet(file => file.Exists)
            .Returns(false);

        Assert.Throws<ArgumentException>(() => sut.GetPackageReferences(packagesConfigFileMock.Object));
    }

    [Fact]
    public void GetPackageReferences_From_File()
    {
        var mockFileSystem = new MockFileSystem();
        
        mockFileSystem.AddFile("packages.config", new MockFileData(ExamplePackagesConfigContent));
        var packagesConfigFile = mockFileSystem.FileInfo.FromFileName("packages.config");
        
        var context = ArrangeContext<ParsePackagesConfigFile>.Create();

        var expectedResult = new[]
        {
            new PackageReference("AutoMapper", "net40", "4.0.4"),
            new PackageReference("Microsoft.Bcl", "net40", "1.1.9"),
            new PackageReference("Microsoft.Bcl.Build", "net40", "1.0.14")
        };

        var sut = context.Build();

        var result = sut.GetPackageReferences(packagesConfigFile);

        result
            .Should()
            .BeEquivalentTo(expectedResult);
    }
}
