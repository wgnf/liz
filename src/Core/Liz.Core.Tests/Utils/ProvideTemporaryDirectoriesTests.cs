using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.Settings;
using Liz.Core.Utils;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Liz.Core.Tests.Utils;

public class ProvideTemporaryDirectoriesTests
{
    [Fact]
    public void GetRootDirectory_Provides_A_Temporary_Directory_Next_To_The_Target_File()
    {
        const string targetFile = "TargetFile.csproj";

        var settings = new ExtractLicensesSettings(targetFile);
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { { targetFile, "" } });

        var context = ArrangeContext<ProvideTemporaryDirectories>.Create();
        context.Use(settings);
        context.Use<IFileSystem>(mockFileSystem);

        var sut = context.Build();

        var temporaryDirectory = sut.GetRootDirectory();

        temporaryDirectory
            .Should()
            .NotBeNull();
        temporaryDirectory
            .FullName
            .Should()
            .Contain("liz_tmp");
    }
    
    [Fact]
    public void GetDownloadDirectory_Provides_A_Temporary_Download_Directory_Underneath_The_Root_Directory()
    {
        const string targetFile = "TargetFile.csproj";

        var settings = new ExtractLicensesSettings(targetFile);
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { { targetFile, "" } });

        var context = ArrangeContext<ProvideTemporaryDirectories>.Create();
        context.Use(settings);
        context.Use<IFileSystem>(mockFileSystem);

        var sut = context.Build();

        var temporaryDownloadDirectory = sut.GetDownloadDirectory();

        temporaryDownloadDirectory
            .Should()
            .NotBeNull();
        temporaryDownloadDirectory
            .FullName
            .Should()
            .Contain($"liz_tmp{Path.PathSeparator}download");
    }
}
