using ArrangeContext.Moq;
using Liz.Core.License.Contracts.Models;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Result;
using Liz.Core.Settings;
using Moq;
using Xunit;

namespace Liz.Core.Tests.Result;

public class PrintPackageDetailsResultProcessorTests
{
    [Fact]
    public async Task PrintPackageReferences_Fails_On_Invalid_Parameters()
    {
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.SuppressPrintDetails = false;
        
        var context = ArrangeContext<PrintPackageDetailsResultProcessor>.Create();
        context.Use(settings);
        
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ProcessResultsAsync(null!));
    }

    [Fact]
    public async Task PrintPackageReferences_Does_Nothing_When_Disabled()
    {
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.SuppressPrintDetails = true;
        
        var context = ArrangeContext<PrintPackageDetailsResultProcessor>.Create();
        context.Use(settings);
        
        var sut = context.Build();

        var packageReference = new PackageReference("SomePackage", "net6.0", "1.2.5")
        {
            LicenseInformation = new LicenseInformation
            {
                Text = "Something something", 
                Type = "MIT", 
                Url = "https://example.org/"
            }
        };

        await sut.ProcessResultsAsync(new[] { packageReference });

        context
            .For<ILogger>()
            .Verify(logger => 
                logger.Log(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
    }
    
    [Fact]
    public async Task PrintPackageReferences_Does_Nothing_When_No_Package_References()
    {
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.SuppressPrintDetails = false;
        
        var context = ArrangeContext<PrintPackageDetailsResultProcessor>.Create();
        context.Use(settings);
        
        var sut = context.Build();
        
        await sut.ProcessResultsAsync(Enumerable.Empty<PackageReference>());

        context
            .For<ILogger>()
            .Verify(logger => 
                logger.Log(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public async Task PrintPackageReferences()
    {
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.SuppressPrintDetails = false;
        
        var context = ArrangeContext<PrintPackageDetailsResultProcessor>.Create();
        context.Use(settings);
        
        var sut = context.Build();
        
        var packageReference = new PackageReference("SomePackage", "net6.0", "1.2.5")
        {
            LicenseInformation = new LicenseInformation
            {
                Text = "Something something", 
                Type = "MIT", 
                Url = "https://example.org/"
            }
        };

        await sut.ProcessResultsAsync(new[] { packageReference });

        context
            .For<ILogger>()
            .Verify(logger =>
                    logger.Log(
                        It.Is<LogLevel>(level => level == LogLevel.Information),
                        It.Is<string>(message =>
                            message.Contains(packageReference.Name) && 
                            message.Contains(packageReference.Version) &&
                            message.Contains(packageReference.LicenseInformation.Url) &&
                            message.Contains(packageReference.LicenseInformation.Type)),
                        It.Is<Exception>(exception => exception == null)),
                Times.Once);
    }
}
