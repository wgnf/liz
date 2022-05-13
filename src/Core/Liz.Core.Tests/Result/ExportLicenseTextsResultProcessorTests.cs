using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Result;
using Liz.Core.Settings;
using Moq;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Liz.Core.Tests.Result;

public class ExportLicenseTextsResultProcessorTests
{
    [Fact]
    public async Task Throws_On_Invalid_Parameter()
    {
        var context = ArrangeContext<ExportLicenseTextsResultProcessor>.Create();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ProcessResultsAsync(null!));
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Does_Nothing_When_Setting_Not_Set(string? exportDirectory)
    {
        var mockFileSystem = new MockFileSystem();
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        
        var context = ArrangeContext<ExportLicenseTextsResultProcessor>.Create();
        context.Use<IFileSystem>(mockFileSystem);
        context.Use(settings);

        settings.ExportLicenseTextsDirectory = exportDirectory;
        
        var sut = context.Build();

        var packageReference = new PackageReference("Something", "net5.0", "1.0.0");
        await sut.ProcessResultsAsync(new[] { packageReference });

        mockFileSystem
            .AllFiles
            .Should()
            .BeEmpty();
    }

    [Fact]
    public async Task Writes_License_Text_To_Txt_File()
    {
        var mockFileSystem = new MockFileSystem();
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        
        var context = ArrangeContext<ExportLicenseTextsResultProcessor>.Create();
        context.Use<IFileSystem>(mockFileSystem);
        context.Use(settings);

        settings.ExportLicenseTextsDirectory = "./somewhere/license-texts";
        
        var sut = context.Build();

        const string licenseText = "some license text abc";
        var packageReference = new PackageReference("Something", "net5.0", "1.0.0")
        {
            LicenseInformation = { Text = licenseText }
        };

        await sut.ProcessResultsAsync(new[] { packageReference });

        var expectedPath = mockFileSystem
            .Path
            .Combine(settings.ExportLicenseTextsDirectory, $"{packageReference.Name}-{packageReference.Version}.txt");
        
        var textFile = mockFileSystem.GetFile(expectedPath);
        
        textFile
            .TextContents
            .Should()
            .Be(licenseText);
    }
    
    [Fact]
    public async Task Writes_License_Text_To_Html_File()
    {
        var mockFileSystem = new MockFileSystem();
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        
        var context = ArrangeContext<ExportLicenseTextsResultProcessor>.Create();
        context.Use<IFileSystem>(mockFileSystem);
        context.Use(settings);

        settings.ExportLicenseTextsDirectory = "./somewhere/license-texts";
        
        var sut = context.Build();

        const string licenseText = "<!DOCTYPE html> some license text abc";
        var packageReference = new PackageReference("Something", "net5.0", "1.0.0")
        {
            LicenseInformation = { Text = licenseText }
        };

        await sut.ProcessResultsAsync(new[] { packageReference });

        var expectedPath = mockFileSystem
            .Path
            .Combine(settings.ExportLicenseTextsDirectory, $"{packageReference.Name}-{packageReference.Version}.html");
        
        var textFile = mockFileSystem.GetFile(expectedPath);
        
        textFile
            .TextContents
            .Should()
            .Be(licenseText);
    }
    
    [Fact]
    public async Task Writes_Nothing_When_License_Text_Is_Empty()
    {
        var mockFileSystem = new MockFileSystem();
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        
        var context = ArrangeContext<ExportLicenseTextsResultProcessor>.Create();
        context.Use<IFileSystem>(mockFileSystem);
        context.Use(settings);

        settings.ExportLicenseTextsDirectory = "./somewhere/license-texts";
        
        var sut = context.Build();

        var licenseText = string.Empty;
        var packageReference = new PackageReference("Something", "net5.0", "1.0.0")
        {
            LicenseInformation = { Text = licenseText }
        };

        await sut.ProcessResultsAsync(new[] { packageReference });

        mockFileSystem
            .AllFiles
            .Should()
            .BeEmpty();
    }
}
