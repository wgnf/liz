using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;
using Liz.Core.License.Sources.LicenseInformation;
using Liz.Core.License.Sources.LicenseType;
using Moq;
using Xunit;

namespace Liz.Core.Tests.License.Sources.LicenseInformation;

public class LicenseTypeFromTextLicenseInformationSourceTests
{
    [Fact]
    public void Order_Is_At_The_Start()
    {
        var context = ArrangeContext<LicenseTypeFromTextLicenseInformationSource>.Create();
        var sut = context.Build();

        sut
            .Order
            .Should()
            .BeGreaterThan(5);
    }

    [Fact]
    public async Task GetInformation_Fails_On_Invalid_Parameters()
    {
        var context = ArrangeContext<LicenseTypeFromTextLicenseInformationSource>.Create();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetInformationAsync(null!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetInformation_Does_Nothing_When_No_License_Text(string licenseText)
    {
        var context = ArrangeContext<LicenseTypeFromTextLicenseInformationSource>.Create();
        
        var licenseTypeDefinition = new LicenseTypeDefinition("ABC", "");
        var licenseTypeProvider = new Mock<ILicenseTypeDefinitionProvider>();
        licenseTypeProvider
            .Setup(provider => provider.Get())
            .Returns(new[] { licenseTypeDefinition });

        context.Use((IEnumerable<ILicenseTypeDefinitionProvider>)new[] { licenseTypeProvider.Object });

        var sut = context.Build();

        var licenseInformationContext = new GetLicenseInformationContext
        {
            LicenseInformation = { Text = licenseText }
        };

        await sut.GetInformationAsync(licenseInformationContext);

        licenseInformationContext
            .LicenseInformation
            .Types
            .Should()
            .BeEmpty();
    }
    
    [Fact]
    public async Task GetInformation_Does_Nothing_When_Webpage()
    {
        const string licenseText = "<!DOCTYPE html> something something";

        var context = ArrangeContext<LicenseTypeFromTextLicenseInformationSource>.Create();
        
        var licenseTypeDefinition = new LicenseTypeDefinition("ABC", "something");
        var licenseTypeProvider = new Mock<ILicenseTypeDefinitionProvider>();
        licenseTypeProvider
            .Setup(provider => provider.Get())
            .Returns(new[] { licenseTypeDefinition });

        context.Use((IEnumerable<ILicenseTypeDefinitionProvider>)new[] { licenseTypeProvider.Object });
        
        var sut = context.Build();
        
        var licenseInformationContext = new GetLicenseInformationContext
        {
            LicenseInformation = { Text = licenseText }
        };

        await sut.GetInformationAsync(licenseInformationContext);

        licenseInformationContext
            .LicenseInformation
            .Types
            .Should()
            .NotContain(licenseTypeDefinition.LicenseType);
    }

    [Fact]
    public async Task GetInformation_Gets_License_Type_From_Definition()
    {
        const string licenseText = "123";

        var licenseTypeDefinition1 = new LicenseTypeDefinition("ABC", licenseText);
        var licenseTypeDefinition2 = new LicenseTypeDefinition("DEF", licenseText);
        
        var context = ArrangeContext<LicenseTypeFromTextLicenseInformationSource>.Create();
        var licenseTypeProvider = new Mock<ILicenseTypeDefinitionProvider>();
        licenseTypeProvider
            .Setup(provider => provider.Get())
            .Returns(new[] { licenseTypeDefinition1, licenseTypeDefinition2 });

        context.Use((IEnumerable<ILicenseTypeDefinitionProvider>)new[] { licenseTypeProvider.Object });
        
        var sut = context.Build();
        
        var licenseInformationContext = new GetLicenseInformationContext
        {
            LicenseInformation = { Text = licenseText }
        };
        
        licenseInformationContext
            .LicenseInformation
            .AddLicenseType(licenseTypeDefinition1.LicenseType);

        await sut.GetInformationAsync(licenseInformationContext);

        licenseInformationContext
            .LicenseInformation
            .Types
            .Should()
            .Contain(new[] { licenseTypeDefinition1.LicenseType, licenseTypeDefinition2.LicenseType });
    }

    [Fact]
    public async Task GetInformation_Excludes_Non_Fitting_Types()
    {
        var licenseTypeToInclude = new LicenseTypeDefinition("ABC", "123");
        var licenseTypeToNotInclude = new LicenseTypeDefinition("DEF", "123")
        {
            ExclusionTextSnippets = new[] { "4" }
        };
        
        var context = ArrangeContext<LicenseTypeFromTextLicenseInformationSource>.Create();
        var licenseTypeProvider = new Mock<ILicenseTypeDefinitionProvider>();
        licenseTypeProvider
            .Setup(provider => provider.Get())
            .Returns(new[] { licenseTypeToInclude, licenseTypeToNotInclude });

        context.Use((IEnumerable<ILicenseTypeDefinitionProvider>)new[] { licenseTypeProvider.Object });
        
        var sut = context.Build();
        
        var licenseInformationContext = new GetLicenseInformationContext
        {
            LicenseInformation = { Text = "1234" }
        };
        
        await sut.GetInformationAsync(licenseInformationContext);

        licenseInformationContext
            .LicenseInformation
            .Types
            .Should()
            .OnlyContain(type => type == licenseTypeToInclude.LicenseType);
    }
}
