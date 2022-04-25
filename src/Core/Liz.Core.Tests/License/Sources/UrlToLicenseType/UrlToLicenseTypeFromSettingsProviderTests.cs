using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.License.Sources.UrlToLicenseType;
using Liz.Core.Settings;
using Moq;
using Xunit;

namespace Liz.Core.Tests.License.Sources.UrlToLicenseType;

public class UrlToLicenseTypeFromSettingsProviderTests
{
    [Fact]
    public void Provides_Definitions_From_The_Settings()
    {
        var mappings = new Dictionary<string, string>
        {
            { "a", "a" }, 
            { "b", "b" }, 
            { "c", "c" }
        };
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.UrlToLicenseTypeMapping = mappings;

        var context = ArrangeContext<UrlToLicenseTypeFromSettingsProvider>.Create();
        context.Use(settings);

        var sut = context.Build();

        var result = sut.Get();

        result
            .Should()
            .BeEquivalentTo(mappings);
    }
}
