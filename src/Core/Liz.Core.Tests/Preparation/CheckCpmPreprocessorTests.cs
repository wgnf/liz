using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.Logging.Contracts;
using Liz.Core.Preparation;
using Liz.Core.Preparation.Contracts.Models;
using Liz.Core.Settings;
using Moq;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Liz.Core.Tests.Preparation;

public sealed class CheckCpmPreprocessorTests
{
    [Fact]
    public async Task Gets_Not_Enabled_When_No_Props_File()
    {
        const string targetFile = "C:/some/directory/target.file";
        var sourceInfo = new SourceInfo();
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { targetFile, new MockFileData("") }
        });

        var context = ArrangeContext<CheckCpmPreprocessor>.Create();
        context.Use(sourceInfo);
        context.Use<IFileSystem>(mockFileSystem);

        context
            .For<ExtractLicensesSettingsBase>()
            .Setup(settings => settings.GetTargetFile())
            .Returns(targetFile);

        var sut = context.Build();

        await sut.PreprocessAsync();

        sourceInfo
            .IsCpmEnabled
            .Should()
            .BeFalse();
    }

    [Fact]
    public async Task Gets_Not_Enabled_When_No_Enabling_Element()
    {
        const string propsFileContent = @"
<Project>
    <ItemGroup>
        <PackageVersion Include=""NewtonSoft.Json"" Version=""1.3.3"" />
    </ItemGroup>
</Project>";
        
        const string targetFile = "C:/some/directory/target.file";
        var sourceInfo = new SourceInfo();
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { targetFile, new MockFileData("") }, 
            { "C:/some/Directory.Packages.props", new MockFileData(propsFileContent) }
        });
        
        var context = ArrangeContext<CheckCpmPreprocessor>.Create();
        context.Use(sourceInfo);
        context.Use<IFileSystem>(mockFileSystem);

        context
            .For<ExtractLicensesSettingsBase>()
            .Setup(settings => settings.GetTargetFile())
            .Returns(targetFile);

        var sut = context.Build();

        await sut.PreprocessAsync();

        sourceInfo
            .IsCpmEnabled
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public async Task Gets_Not_Enabled_When_Enabling_Elements_Value_Is_False()
    {
        const string propsFileContent = @"
<Project>
    <PropertyGroup>
        <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
    </PropertyGroup>
    
    <ItemGroup>
        <PackageVersion Include=""NewtonSoft.Json"" Version=""1.3.3"" />
    </ItemGroup>
</Project>";
        
        const string targetFile = "C:/some/directory/target.file";
        var sourceInfo = new SourceInfo();
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { targetFile, new MockFileData("") }, 
            { "C:/some/Directory.Packages.props", new MockFileData(propsFileContent) }
        });
        
        var context = ArrangeContext<CheckCpmPreprocessor>.Create();
        context.Use(sourceInfo);
        context.Use<IFileSystem>(mockFileSystem);

        context
            .For<ExtractLicensesSettingsBase>()
            .Setup(settings => settings.GetTargetFile())
            .Returns(targetFile);

        var sut = context.Build();

        await sut.PreprocessAsync();

        sourceInfo
            .IsCpmEnabled
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public async Task Gets_Enabled_When_Enabling_Elements_Value_Is_True()
    {
        const string propsFileContent = @"
<Project>
    <PropertyGroup>
        <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    </PropertyGroup>
    
    <ItemGroup>
        <PackageVersion Include=""NewtonSoft.Json"" Version=""1.3.3"" />
    </ItemGroup>
</Project>";
        
        const string targetFile = "C:/some/directory/target.file";
        var sourceInfo = new SourceInfo();
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { targetFile, new MockFileData("") }, 
            { "C:/some/Directory.Packages.props", new MockFileData(propsFileContent) }
        });
        
        var context = ArrangeContext<CheckCpmPreprocessor>.Create();
        context.Use(sourceInfo);
        context.Use<IFileSystem>(mockFileSystem);

        context
            .For<ExtractLicensesSettingsBase>()
            .Setup(settings => settings.GetTargetFile())
            .Returns(targetFile);

        var sut = context.Build();

        await sut.PreprocessAsync();

        sourceInfo
            .IsCpmEnabled
            .Should()
            .BeTrue();
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Gets_Enabled_Status_From_Imported_File(bool enabledInImportedFile)
    {
        const string propsFileContent = @"
<Project>
    <Import Project=""../Directory.Packages.props""/>
    
    <ItemGroup>
        <PackageVersion Include=""NewtonSoft.Json"" Version=""1.3.3"" />
    </ItemGroup>
</Project>";
        
        var importedPropsFileContent = $@"
<Project>
    <PropertyGroup>
        <ManagePackageVersionsCentrally>{enabledInImportedFile}</ManagePackageVersionsCentrally>
    </PropertyGroup>
</Project>";
        
        const string targetFile = "C:/some/directory/target.file";
        var sourceInfo = new SourceInfo();
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { targetFile, new MockFileData("") }, 
            { "C:/some/Directory.Packages.props", new MockFileData(propsFileContent) },
            { "C:/Directory.Packages.props", new MockFileData(importedPropsFileContent) }
        });
        
        var context = ArrangeContext<CheckCpmPreprocessor>.Create();
        context.Use(sourceInfo);
        context.Use<IFileSystem>(mockFileSystem);

        context
            .For<ExtractLicensesSettingsBase>()
            .Setup(settings => settings.GetTargetFile())
            .Returns(targetFile);

        var sut = context.Build();

        await sut.PreprocessAsync();

        sourceInfo
            .IsCpmEnabled
            .Should()
            .Be(enabledInImportedFile);
    }
    
    [Fact]
    public async Task Gets_Enabled_When_Enabling_Elements_Value_Is_True_And_Logs_Warning_When_Override_Disabled()
    {
        const string propsFileContent = @"
<Project>
    <PropertyGroup>
        <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
        <EnablePackageVersionOverride>false</EnablePackageVersionOverride>
    </PropertyGroup>
    
    <ItemGroup>
        <PackageVersion Include=""NewtonSoft.Json"" Version=""1.3.3"" />
    </ItemGroup>
</Project>";
        
        const string targetFile = "C:/some/directory/target.file";
        var sourceInfo = new SourceInfo();
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { targetFile, new MockFileData("") }, 
            { "C:/some/Directory.Packages.props", new MockFileData(propsFileContent) }
        });
        
        var context = ArrangeContext<CheckCpmPreprocessor>.Create();
        context.Use(sourceInfo);
        context.Use<IFileSystem>(mockFileSystem);

        context
            .For<ExtractLicensesSettingsBase>()
            .Setup(settings => settings.GetTargetFile())
            .Returns(targetFile);

        var sut = context.Build();

        await sut.PreprocessAsync();

        sourceInfo
            .IsCpmEnabled
            .Should()
            .BeTrue();
        
        context
            .For<ILogger>()
            .Verify(logger => logger.Log(LogLevel.Warning, It.IsAny<string>(), null), Times.Once);
    }
}
