using ArrangeContext.Moq;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Result;
using Liz.Core.Result.Contracts.Exceptions;
using Liz.Core.Settings;
using Moq;
using Xunit;

namespace Liz.Core.Tests.Result;

public class ValidateLicenseTypesBlacklistResultProcessorTests
{
    [Fact]
    public async Task Throws_On_Invalid_Parameters()
    {
        var context = ArrangeContext<ValidateLicenseTypesBlacklistResultProcessor>.Create();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ProcessResultsAsync(null!));
    }

    [Fact]
    public async Task Does_Nothing_When_No_Blacklist_Entries()
    {
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.LicenseTypeBlacklist = Enumerable.Empty<string>().ToList();

        var context = ArrangeContext<ValidateLicenseTypesBlacklistResultProcessor>.Create();
        context.Use(settings);
        
        var sut = context.Build();

        var packageReference = new PackageReference("SomePackage", "net6.0", "1.2.3");
        packageReference.LicenseInformation.AddLicenseType("MIT");

        var packageReferences = new List<PackageReference> { packageReference };

        await sut.ProcessResultsAsync(packageReferences);
        
        // nothing should throw, because there's no blacklist
    }

    [Fact]
    public async Task Does_Nothing_When_No_Package_Reference_Is_Invalid()
    {
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.LicenseTypeBlacklist = new List<string> { "GPL-3.0" };

        var context = ArrangeContext<ValidateLicenseTypesBlacklistResultProcessor>.Create();
        context.Use(settings);

        var sut = context.Build();
        
        var packageReferenceMit = new PackageReference("MitPackage", "net6.0", "1.2.3");
        packageReferenceMit.LicenseInformation.AddLicenseType("MIT");

        var packageReferenceUnlicense = new PackageReference("UnlicensePackage", "net6.0", "5.0.0");
        packageReferenceUnlicense.LicenseInformation.AddLicenseType("Unlicense");

        var packageReferences = new List<PackageReference> { packageReferenceMit, packageReferenceUnlicense };

        await sut.ProcessResultsAsync(packageReferences);
        
        // nothing should throw, because both package's license-type is not in the blacklist
    }
    
    [Fact]
    public async Task Throws_When_Package_Reference_Is_Invalid()
    {
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.LicenseTypeBlacklist = new List<string> { "GPL-3.0" };

        var context = ArrangeContext<ValidateLicenseTypesBlacklistResultProcessor>.Create();
        context.Use(settings);

        var sut = context.Build();
        
        var packageReferenceMit = new PackageReference("MitPackage", "net6.0", "1.2.3");
        packageReferenceMit.LicenseInformation.AddLicenseType("MIT");

        var packageReferenceUnlicense = new PackageReference("UnlicensePackage", "net6.0", "5.0.0");
        packageReferenceUnlicense.LicenseInformation.AddLicenseType("Unlicense");

        var packageReferenceGpl = new PackageReference("GplPackage", "net6.0", "1.0.0");
        packageReferenceGpl.LicenseInformation.AddLicenseType("GPL-3.0");

        var packageReferences = new List<PackageReference>
        {
            packageReferenceMit, 
            packageReferenceUnlicense, 
            packageReferenceGpl
        };

        // because "GPL-3.0" is blacklisted
        await Assert.ThrowsAsync<LicenseTypesInvalidException>(() => sut.ProcessResultsAsync(packageReferences));
    }
}
