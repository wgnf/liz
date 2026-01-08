using ArrangeContext.Moq;
using AwesomeAssertions;
using Liz.Core.License.Sources.LicenseType;
using Xunit;

namespace Liz.Core.Tests.License.Sources.LicenseType;

public class SpecialLicenseTypeDefinitionProviderTests
{
    [Fact]
    public void Always_Provides_Something()
    {
        var context = ArrangeContext<SpecialLicenseTypeDefinitionProvider>.Create();
        var sut = context.Build();

        var result = sut.Get();

        result
            .Should()
            .NotBeNullOrEmpty();
    }
}
