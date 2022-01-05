using FluentAssertions;
using Liz.Core.Settings;
using Liz.Core.Utils;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Liz.Core.Tests.Utils;

public class ProvideTemporaryDirectoryTests
{
    [Fact]
    public void Get_Should_Provide_A_Temporary_Directory_Next_To_The_Target_File()
    {
        const string targetFile = "TargetFile.csproj";

        var settings = new ExtractLicensesSettings(targetFile);
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { { targetFile, "" } });

        // TODO: change this once the bug has been fixed in ArrangeContext
        var sut = new ProvideTemporaryDirectory(
            settings,
            mockFileSystem);

        var temporaryDirectory = sut.Get();

        temporaryDirectory
            .Should()
            .NotBeNull();
        temporaryDirectory
            .FullName
            .Should()
            .Contain("liz_tmp");
    }
}
