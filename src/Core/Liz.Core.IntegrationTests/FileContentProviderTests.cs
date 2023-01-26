using FluentAssertions;
using Liz.Core.Settings;
using Liz.Core.Utils;
using Liz.Core.Utils.Wrappers;
using Moq;
using System.IO.Abstractions;
using Xunit;

namespace Liz.Core.IntegrationTests;

public class FileContentProviderTests
{
    [Fact]
    public async Task Provides_Remote_File_Content()
    {
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        var provider = new FileContentProvider(new FileSystem(), new HttpClientWrapper(settings));

        const string readmeUrl = "https://raw.githubusercontent.com/wgnf/liz/main/README.md";

        var result = await provider.GetFileContentAsync(readmeUrl);

        result
            .Should()
            .Contain("liz");
    }
}
