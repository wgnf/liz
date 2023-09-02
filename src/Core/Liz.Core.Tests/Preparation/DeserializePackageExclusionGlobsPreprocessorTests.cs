using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.Logging.Contracts;
using Liz.Core.Preparation;
using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;
using Moq;
using Xunit;

namespace Liz.Core.Tests.Preparation;

public class DeserializePackageExclusionGlobsPreprocessorTests
{
    [Fact]
    public async Task Does_Nothing_When_Setting_Not_Set()
    {
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.PackageExclusionGlobsFilePath = null;

        var context = ArrangeContext<DeserializePackageExclusionGlobsPreprocessor>.Create();
        context.Use(settings);

        var sut = context.Build();

        await sut.PreprocessAsync();

        context
            .For<IFileContentProvider>()
            .Verify(fileContentProvider => fileContentProvider.GetFileContentAsync(It.IsAny<string>()), Times.Never);
    }
    
    [Fact]
    public async Task Logs_Warning_When_Get_Content_Fails()
    {
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.PackageExclusionGlobsFilePath = "something";

        var context = ArrangeContext<DeserializePackageExclusionGlobsPreprocessor>.Create();
        context.Use(settings);

        var sut = context.Build();

        context
            .For<IFileContentProvider>()
            .Setup(fileContentProvider => fileContentProvider.GetFileContentAsync(It.IsAny<string>()))
            .Throws<Exception>();

        await sut.PreprocessAsync();

        context
            .For<ILogger>()
            .Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                    It.IsAny<string>(),
                    It.IsAny<Exception?>()),
                Times.Once);
    }

    [Fact]
    public async Task Logs_Warning_When_Json_Cannot_Be_Deserialized()
    {
        const string json = @"{ ""gibberish"": ""something"" }";

        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.PackageExclusionGlobsFilePath = "something";

        var context = ArrangeContext<DeserializePackageExclusionGlobsPreprocessor>.Create();
        context.Use(settings);

        var sut = context.Build();

        context
            .For<IFileContentProvider>()
            .Setup(fileContentProvider => fileContentProvider.GetFileContentAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(json));

        await sut.PreprocessAsync();

        context
            .For<ILogger>()
            .Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                    It.IsAny<string>(),
                    It.IsAny<Exception?>()),
                Times.Once);
    }

    [Fact]
    public async Task Only_Uses_Correct_Entries()
    {
        const string json = @"
[
  """",
  "" "",
  ""something""
]";

        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.PackageExclusionGlobsFilePath = "something";

        var context = ArrangeContext<DeserializePackageExclusionGlobsPreprocessor>.Create();
        context.Use(settings);

        var sut = context.Build();

        context
            .For<IFileContentProvider>()
            .Setup(fileContentProvider => fileContentProvider.GetFileContentAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(json));

        await sut.PreprocessAsync();
        
        var expectedEntries = new[] { "something" };
        
        settings
            .PackageExclusionGlobs
            .Should()
            .BeEquivalentTo(expectedEntries);
    }

    [Fact]
    public async Task Adds_Entries_To_Already_Existing_Entries()
    {
        const string json = @"
[
  """",
  "" "",
  ""something""
]";

        const string alreadyExistingEntry = "else";

        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        
        settings.PackageExclusionGlobs.Add(alreadyExistingEntry);
        settings.PackageExclusionGlobsFilePath = "something";

        var context = ArrangeContext<DeserializePackageExclusionGlobsPreprocessor>.Create();
        context.Use(settings);

        var sut = context.Build();

        context
            .For<IFileContentProvider>()
            .Setup(fileContentProvider => fileContentProvider.GetFileContentAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(json));

        await sut.PreprocessAsync();

        var expectedEntries = new[] { "something", alreadyExistingEntry };

        settings
            .PackageExclusionGlobs
            .Should()
            .BeEquivalentTo(expectedEntries);
    }
}
