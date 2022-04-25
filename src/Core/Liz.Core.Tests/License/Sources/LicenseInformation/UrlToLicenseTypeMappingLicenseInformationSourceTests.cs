using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;
using Liz.Core.License.Sources.LicenseInformation;
using Moq;
using Xunit;

namespace Liz.Core.Tests.License.Sources.LicenseInformation;

public class UrlToLicenseTypeMappingLicenseInformationSourceTests
{
    [Fact]
    public void Order_Is_Somewhere_At_The_End()
    {
        var context = ArrangeContext<UrlToLicenseTypeMappingLicenseInformationSource>.Create();
        
        var urlMappingProvider = new Mock<IUrlToLicenseTypeMappingProvider>();
        urlMappingProvider
            .Setup(provider => provider.Get())
            .Returns(new Dictionary<string, string> { { "something", "something" } });

        context.Use((IEnumerable<IUrlToLicenseTypeMappingProvider>)new[] { urlMappingProvider.Object });
        
        var sut = context.Build();

        sut
            .Order
            .Should()
            .BeGreaterThan(10);
    }

    [Fact]
    public async Task GetInformation_Fails_On_Invalid_Parameters()
    {
        var context = ArrangeContext<UrlToLicenseTypeMappingLicenseInformationSource>.Create();
        
        var urlMappingProvider = new Mock<IUrlToLicenseTypeMappingProvider>();
        urlMappingProvider
            .Setup(provider => provider.Get())
            .Returns(new Dictionary<string, string> { { "something", "something" } });

        context.Use((IEnumerable<IUrlToLicenseTypeMappingProvider>)new[] { urlMappingProvider.Object });
        
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetInformationAsync(null!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetInformation_Does_Nothing_When_No_License_Url(string licenseUrl)
    {
        var context = ArrangeContext<UrlToLicenseTypeMappingLicenseInformationSource>.Create();

        var urlMappingProvider = new Mock<IUrlToLicenseTypeMappingProvider>();
        urlMappingProvider
            .Setup(provider => provider.Get())
            .Returns(new Dictionary<string, string> { { "something", "something" } });

        context.Use((IEnumerable<IUrlToLicenseTypeMappingProvider>)new[] { urlMappingProvider.Object });

        var sut = context.Build();

        var licenseInformationContext = new GetLicenseInformationContext
        {
            LicenseInformation = { Url = licenseUrl }
        };

        await sut.GetInformationAsync(licenseInformationContext);

        licenseInformationContext
            .LicenseInformation
            .Types
            .Should()
            .BeEmpty();
    }

    [Fact]
    public async Task GetInformation_Gets_License_Type_From_Mapping()
    {
        const string licenseUrl = "https://example.org/license/mit";

        var mappings = new Dictionary<string, string>
        {
            { licenseUrl, "MIT" }, 
            { "https://goole.com/goo-gl", "GOO-GL" }
        };
        
        var context = ArrangeContext<UrlToLicenseTypeMappingLicenseInformationSource>.Create();
        
        var urlMappingProvider = new Mock<IUrlToLicenseTypeMappingProvider>();
        urlMappingProvider
            .Setup(provider => provider.Get())
            .Returns(mappings);
        
        context.Use((IEnumerable<IUrlToLicenseTypeMappingProvider>)new[] { urlMappingProvider.Object });
        
        var sut = context.Build();

        var licenseInformationContext = new GetLicenseInformationContext
        {
            LicenseInformation = { Url = licenseUrl }
        };

        await sut.GetInformationAsync(licenseInformationContext);

        licenseInformationContext
            .LicenseInformation
            .Types
            .Should()
            .OnlyContain(type => type == "MIT");
    }
}
