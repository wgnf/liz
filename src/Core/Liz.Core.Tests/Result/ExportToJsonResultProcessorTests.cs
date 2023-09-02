using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Result;
using Liz.Core.Settings;
using Moq;
using Xunit;

namespace Liz.Core.Tests.Result;

public sealed class ExportToJsonResultProcessorTests
{
    [Fact]
    public async Task Throws_On_Invalid_Parameter()
    {
        var context = ArrangeContext<ExportToJsonResultProcessor>.Create();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ProcessResultsAsync(null!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Does_Nothing_When_Setting_Not_Set(string? exportJsonFile)
    {
        var mockFileSystem = new MockFileSystem();
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        
        var context = ArrangeContext<ExportToJsonResultProcessor>.Create();
        context.Use<IFileSystem>(mockFileSystem);
        context.Use(settings);

        settings.ExportJsonFile = exportJsonFile;
        
        var sut = context.Build();
        
        var packageReference = new PackageReference("Something", "net5.0", "1.0.0");
        await sut.ProcessResultsAsync(new[] { packageReference });

        mockFileSystem
            .AllFiles
            .Should()
            .BeEmpty();
    }

    [Fact]
    public async Task Writes_Package_References_To_Json_File()
    {
        var mockFileSystem = new MockFileSystem();
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        
        var context = ArrangeContext<ExportToJsonResultProcessor>.Create();
        context.Use<IFileSystem>(mockFileSystem);
        context.Use(settings);

        const string jsonExportFilePath = @"./somewhere/export.json";
        settings.ExportJsonFile = jsonExportFilePath;
        
        mockFileSystem.AddFile(jsonExportFilePath, new MockFileData(string.Empty));
        
        var sut = context.Build();
        
        var packageReference = new PackageReference("Something", "net5.0", "1.0.0");
        await sut.ProcessResultsAsync(new[] { packageReference });

        var jsonFile = mockFileSystem.GetFile(jsonExportFilePath);

        jsonFile
            .TextContents
            .Should()
            .NotBeNullOrEmpty();
    }
}
