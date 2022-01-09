using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.Settings;
using Liz.Core.Utils;
using System.Collections.Generic;
using System.IO.Abstractions;
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

        var context = ArrangeContext<ProvideTemporaryDirectory>.Create();
        context.Use(settings);
        context.Use<IFileSystem>(mockFileSystem);

        var sut = context.Build();

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
