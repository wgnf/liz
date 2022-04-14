using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.License.Sources.LicenseType;
using Liz.Core.Logging.Contracts;
using Liz.Core.Preparation;
using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;
using Moq;
using Xunit;

namespace Liz.Core.Tests.Preparation;

public class DeserializeLicenseTypeDefinitionsPreprocessorTests
{
    [Fact]
    public async Task Does_Nothing_When_Setting_Not_Set()
    {
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.LicenseTypeDefinitionsFilePath = null;

        var context = ArrangeContext<DeserializeLicenseTypeDefinitionsPreprocessor>.Create();
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
        settings.LicenseTypeDefinitionsFilePath = "something";

        var context = ArrangeContext<DeserializeLicenseTypeDefinitionsPreprocessor>.Create();
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
        const string json = @"{ ""gibberish"": ""something"" }";
        
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.LicenseTypeDefinitionsFilePath = "something";

        var context = ArrangeContext<DeserializeLicenseTypeDefinitionsPreprocessor>.Create();
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
    public async Task Only_Uses_Correct_Definitions()
    {
        const string json = @"
[
  {
    ""inclusiveText"": [ ""not be used because no type"" ]
  },
  {
    ""type"": ""something"",
    ""exclusiveText"": [ ""not used because no inclusiveText"" ] 
  },
  {
    ""type"": ""LIZ-PL-1.0"",
    ""inclusiveText"": [ ""abc"" ]
  },
  {
    ""type"": ""LIZ-PL-2.0"",
    ""inclusiveText"": [ ""def"" ],
    ""exclusiveText"": [ ""abc"" ]
  }
]";

        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.LicenseTypeDefinitionsFilePath = "something";

        var context = ArrangeContext<DeserializeLicenseTypeDefinitionsPreprocessor>.Create();
        context.Use(settings);

        var sut = context.Build();

        context
            .For<IFileContentProvider>()
            .Setup(fileContentProvider => fileContentProvider.GetFileContentAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(json));

        await sut.PreprocessAsync();
        
        var expectedDefinitions = new List<LicenseTypeDefinition>
        {
            new("LIZ-PL-1.0", "abc"),
            new("LIZ-PL-2.0", "def") { ExclusiveTextSnippets = new[] { "abc" } }
        };

        settings
            .LicenseTypeDefinitions
            .Should()
            .BeEquivalentTo(expectedDefinitions);
    }
    
    [Fact]
    public async Task Adds_File_Definitions_To_Already_Existing_Definitions()
    {
        const string json = @"
[
  {
    ""type"": ""LIZ-PL-1.0"",
    ""inclusiveText"": [ ""abc"" ]
  }
]";

        var alreadyExistingDefintion = new LicenseTypeDefinition("SOMETHING", "else");

        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        
        settings.LicenseTypeDefinitions.Add(alreadyExistingDefintion);
        settings.LicenseTypeDefinitionsFilePath = "something";

        var context = ArrangeContext<DeserializeLicenseTypeDefinitionsPreprocessor>.Create();
        context.Use(settings);

        var sut = context.Build();

        context
            .For<IFileContentProvider>()
            .Setup(fileContentProvider => fileContentProvider.GetFileContentAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(json));

        await sut.PreprocessAsync();

        var expectedDefinitions = new List<LicenseTypeDefinition>
        {
            alreadyExistingDefintion, 
            new("LIZ-PL-1.0", "abc")
        };

        settings
            .LicenseTypeDefinitions
            .Should()
            .BeEquivalentTo(expectedDefinitions);
    }
}
