using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.Logging.Contracts;
using Liz.Core.Preparation;
using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;
using Moq;
using Xunit;

namespace Liz.Core.Tests.Preparation;

public class DeserializeUrlToLicenseTypeMappingPreprocessorTests
{
    [Fact]
    public async Task Does_Nothing_When_Setting_Not_Set()
    {
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.UrlToLicenseTypeMappingFilePath = null;

        var context = ArrangeContext<DeserializeUrlToLicenseTypeMappingPreprocessor>.Create();
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
        settings.UrlToLicenseTypeMappingFilePath = "something";

        var context = ArrangeContext<DeserializeUrlToLicenseTypeMappingPreprocessor>.Create();
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
    public async Task Logs_Warning_Json_Cannot_Be_Deserialized()
    {
        // this is a list of dictionaries...
        const string json = @"[ { ""gibberish"": ""something"" } ]";
        
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.UrlToLicenseTypeMappingFilePath = "something";

        var context = ArrangeContext<DeserializeUrlToLicenseTypeMappingPreprocessor>.Create();
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
    public async Task Only_Uses_Correct_Mappings()
    {
        const string json = @"
{
   """": ""this is not correct as it has no key"",
   ""this is not correct as it has no value"": """",
   ""correct"": ""correct""
}";

        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.UrlToLicenseTypeMappingFilePath = "something";

        var context = ArrangeContext<DeserializeUrlToLicenseTypeMappingPreprocessor>.Create();
        context.Use(settings);

        var sut = context.Build();

        context
            .For<IFileContentProvider>()
            .Setup(fileContentProvider => fileContentProvider.GetFileContentAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(json));

        await sut.PreprocessAsync();

        var expectedMappings = new Dictionary<string, string> { { "correct", "correct" } };

        settings
            .UrlToLicenseTypeMapping
            .Should()
            .BeEquivalentTo(expectedMappings);
    }
    
    [Fact]
    public async Task Adds_File_Definitions_To_Already_Existing_Definitions()
    {
        const string json = @"
{
   ""correct"": ""correct""
}";

        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        
        settings.UrlToLicenseTypeMapping.Add("something", "something");
        settings.UrlToLicenseTypeMappingFilePath = "something";

        var context = ArrangeContext<DeserializeUrlToLicenseTypeMappingPreprocessor>.Create();
        context.Use(settings);

        var sut = context.Build();

        context
            .For<IFileContentProvider>()
            .Setup(fileContentProvider => fileContentProvider.GetFileContentAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(json));

        await sut.PreprocessAsync();

        var expectedMappings = new Dictionary<string, string>
        {
            { "something", "something" }, 
            { "correct", "correct" }
        };

        settings
            .UrlToLicenseTypeMapping
            .Should()
            .BeEquivalentTo(expectedMappings);
    }
}
