using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core;
using Liz.Core.Extract.Contracts;
using Liz.Core.Logging.Contracts;
using Liz.Core.Progress;
using Liz.Core.Settings;
using Liz.Tool.CommandLine;
using Liz.Tool.Contracts.CommandLine;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Liz.Tool.Tests.CommandLine;

public sealed class CommandRunnerTests
{
    [Fact]
    public void Should_Have_Correct_Interface()
    {
        var sut = new ArrangeContext<CommandRunner>().Build();

        sut
            .Should()
            .BeAssignableTo<ICommandRunner>();
    }

    [Fact]
    public async Task Should_Fail_To_Run_When_Invalid_Parameters()
    {
        var sut = new ArrangeContext<CommandRunner>().Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.RunAsync(
            null!, 
            LogLevel.Information,
            true, 
            true, 
            true,
            true,
            null));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Forward_Execution_To_Extract_Licenses_With_Settings(bool booleanParameter)
    {
        var extractLicenses = new Mock<IExtractLicenses>();
        var extractLicensesFactory = new Mock<IExtractLicensesFactory>();
        
        extractLicensesFactory
            .Setup(factory => factory.Create(
                It.IsAny<ExtractLicensesSettingsBase>(), 
                It.IsAny<ILoggerProvider>(),
                It.IsAny<LogLevel>(),
                It.IsAny<IProgressHandler?>()))
            .Returns(extractLicenses.Object);

        var sut = new CommandRunner(extractLicensesFactory.Object);

        var fileInfo = new FileInfo("some/file.txt");

        await sut.RunAsync(
            fileInfo, 
            LogLevel.Information,
            booleanParameter, 
            booleanParameter, 
            booleanParameter,
            booleanParameter,
            null);

        extractLicenses.Verify(e => e.ExtractAsync(), Times.Once());
    }

    [Fact]
    public async Task Should_Throw_When_Provided_License_Type_Definitions_File_Does_Not_Exist()
    {
        var extractLicenses = new Mock<IExtractLicenses>();
        var extractLicensesFactory = new Mock<IExtractLicensesFactory>();
        
        extractLicensesFactory
            .Setup(factory => factory.Create(
                It.IsAny<ExtractLicensesSettingsBase>(), 
                It.IsAny<ILoggerProvider>(),
                It.IsAny<LogLevel>(),
                It.IsAny<IProgressHandler?>()))
            .Returns(extractLicenses.Object);

        // no files, so the provided file cannot exist
        var mockFileSystem = new MockFileSystem();

        var sut = new CommandRunner(extractLicensesFactory.Object, mockFileSystem);
        
        var targetFileInfo = new FileInfo("some/file.txt");
        var typeDefinitionsFile = new FileInfo("abc.json");

        await Assert.ThrowsAsync<FileNotFoundException>(() => sut.RunAsync(
            targetFileInfo,
            LogLevel.Information,
            true,
            true,
            true,
            true,
            typeDefinitionsFile));
    }
    
    [Fact]
    public async Task Should_Throw_When_Provided_License_Type_Definitions_File_Is_Not_Json_File()
    {
        var extractLicenses = new Mock<IExtractLicenses>();
        var extractLicensesFactory = new Mock<IExtractLicensesFactory>();
        
        extractLicensesFactory
            .Setup(factory => factory.Create(
                It.IsAny<ExtractLicensesSettingsBase>(), 
                It.IsAny<ILoggerProvider>(),
                It.IsAny<LogLevel>(),
                It.IsAny<IProgressHandler?>()))
            .Returns(extractLicenses.Object);

        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "abc.something", MockFileData.NullObject }
        }, Environment.CurrentDirectory);

        var sut = new CommandRunner(extractLicensesFactory.Object, mockFileSystem);
        
        var targetFileInfo = new FileInfo("some/file.txt");
        var typeDefinitionsFile = new FileInfo("abc.something");

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.RunAsync(
            targetFileInfo,
            LogLevel.Information,
            true,
            true,
            true,
            true,
            typeDefinitionsFile));
    }
    
    [Fact]
    public async Task Should_Throw_When_Provided_License_Type_Definitions_File_Contains_Wrong_Data()
    {
        var extractLicenses = new Mock<IExtractLicenses>();
        var extractLicensesFactory = new Mock<IExtractLicensesFactory>();
        
        extractLicensesFactory
            .Setup(factory => factory.Create(
                It.IsAny<ExtractLicensesSettingsBase>(), 
                It.IsAny<ILoggerProvider>(),
                It.IsAny<LogLevel>(),
                It.IsAny<IProgressHandler?>()))
            .Returns(extractLicenses.Object);
        
        const string json = @"{ ""gibberish"": ""something"" }";

        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "abc.json", json }
        }, Environment.CurrentDirectory);

        var sut = new CommandRunner(extractLicensesFactory.Object, mockFileSystem);
        
        var targetFileInfo = new FileInfo("some/file.txt");
        var typeDefinitionsFile = new FileInfo("abc.json");

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.RunAsync(
            targetFileInfo,
            LogLevel.Information,
            true,
            true,
            true,
            true,
            typeDefinitionsFile));
    }
    
    [Fact]
    public async Task Should_Convert_Type_Defintions_And_Only_Use_Correct_Data()
    {
        var extractLicenses = new Mock<IExtractLicenses>();
        var extractLicensesFactory = new Mock<IExtractLicensesFactory>();
        
        extractLicensesFactory
            .Setup(factory => factory.Create(
                It.IsAny<ExtractLicensesSettingsBase>(), 
                It.IsAny<ILoggerProvider>(),
                It.IsAny<LogLevel>(),
                It.IsAny<IProgressHandler?>()))
            .Returns(extractLicenses.Object);
        
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

        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "abc.json", json }
        }, Environment.CurrentDirectory);

        var sut = new CommandRunner(extractLicensesFactory.Object, mockFileSystem);
        
        var targetFileInfo = new FileInfo("some/file.txt");
        var typeDefinitionsFile = new FileInfo("abc.json");

        await sut.RunAsync(
            targetFileInfo,
            LogLevel.Information,
            true,
            true,
            true,
            true,
            typeDefinitionsFile);

        extractLicensesFactory
            .Verify(factory => factory.Create(It.Is<ExtractLicensesSettingsBase>(
                    settings => settings.LicenseTypeDefinitions.Count == 2 &&
                                settings.LicenseTypeDefinitions.All(definition =>
                                    definition.LicenseType == "LIZ-PL-1.0" ||
                                    definition.LicenseType == "LIZ-PL-2.0")),
                It.IsAny<ILoggerProvider?>(), It.IsAny<LogLevel>(), It.IsAny<IProgressHandler?>()), 
                Times.Once);
    }
}
