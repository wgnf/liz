using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.License.Contracts.Models;
using Liz.Core.License.Sources.LicenseType;
using Liz.Core.Settings;
using Moq;
using Xunit;

namespace Liz.Core.Tests.License.Sources.LicenseType;

public class LicenseTypeDefinitionFromSettingsProviderTests
{
    [Fact]
    public void Provides_The_Definitions_From_The_Settings()
    {
        var licenseTypes = new List<LicenseTypeDefinition>
        {
            new ("A", "A"),
            new ("B", "B"),
            new ("C", "C")
        };
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.LicenseTypeDefinitions = licenseTypes;
        
        var context = ArrangeContext<LicenseTypeDefinitionFromSettingsProvider>.Create();
        context.Use(settings);
        
        var sut = context.Build();

        var result = sut.Get();

        result
            .Should()
            .BeEquivalentTo(licenseTypes);
    }
}
