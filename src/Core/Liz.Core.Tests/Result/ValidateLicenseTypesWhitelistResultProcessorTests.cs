using ArrangeContext.Moq;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Result;
using Liz.Core.Result.Contracts.Exceptions;
using Liz.Core.Settings;
using Moq;
using Xunit;

namespace Liz.Core.Tests.Result;

public class ValidateLicenseTypesWhitelistResultProcessorTests
{
    [Fact]
    public async Task Throws_On_Invalid_Parameters()
    {
        var context = ArrangeContext<ValidateLicenseTypesWhitelistResultProcessor>.Create();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ProcessResultsAsync(null!));
    }

    [Fact]
    public async Task Does_Nothing_When_No_Whitelist_Entries()
    {
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.LicenseTypeWhitelist = Enumerable.Empty<string>().ToList();
        
        var context = ArrangeContext<ValidateLicenseTypesWhitelistResultProcessor>.Create();
        context.Use(settings);
        
        var sut = context.Build();

        var packageReference = new PackageReference("SomePackage", "net6.0", "1.2.3");
        packageReference.LicenseInformation.AddLicenseType("MIT");

        var packageReferences = new List<PackageReference> { packageReference };

        await sut.ProcessResultsAsync(packageReferences);
        
        // nothing should throw, because there's no whitelist
    }

    [Fact]
    public async Task Does_Nothing_When_No_Package_Reference_Is_Invalid()
    {
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.LicenseTypeWhitelist = new List<string> { "MIT", "Unlicense" };
        
        var context = ArrangeContext<ValidateLicenseTypesWhitelistResultProcessor>.Create();
        context.Use(settings);
        
        var sut = context.Build();

        var packageReferenceMit = new PackageReference("MitPackage", "net6.0", "1.2.3");
        packageReferenceMit.LicenseInformation.AddLicenseType("MIT");

        var packageReferenceUnlicense = new PackageReference("UnlicensePackage", "net6.0", "5.0.0");
        packageReferenceUnlicense.LicenseInformation.AddLicenseType("Unlicense");

        var packageReferences = new List<PackageReference> { packageReferenceMit, packageReferenceUnlicense };

        await sut.ProcessResultsAsync(packageReferences);
        
        // nothing should throw, because both packages license-type is withing the whitelist
    }

    [Fact]
    public async Task Throws_When_Package_Reference_Is_Invalid()
    {
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.LicenseTypeWhitelist = new List<string> { "MIT" };
        
        var context = ArrangeContext<ValidateLicenseTypesWhitelistResultProcessor>.Create();
        context.Use(settings);
        
        var sut = context.Build();

        var packageReferenceMit = new PackageReference("MitPackage", "net6.0", "1.2.3");
        packageReferenceMit.LicenseInformation.AddLicenseType("MIT");

        var packageReferenceUnlicense = new PackageReference("UnlicensePackage", "net6.0", "5.0.0");
        packageReferenceUnlicense.LicenseInformation.AddLicenseType("Unlicense");

        var packageReferences = new List<PackageReference> { packageReferenceMit, packageReferenceUnlicense };

        // because "Unlicense" is not whitelisted
        await Assert.ThrowsAsync<LicenseTypesInvalidException>(() => sut.ProcessResultsAsync(packageReferences));
    }
}
