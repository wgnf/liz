using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.License.Sources.UrlToLicenseType;
using Xunit;

namespace Liz.Core.Tests.License.Sources.UrlToLicenseType;

public class OpenSourceOrgUrlToLicenseTypeProviderTests
{
    [Fact]
    public void Provides_Definitions()
    {
        var context = ArrangeContext<OpenSourceOrgUrlToLicenseTypeProvider>.Create();
        var sut = context.Build();

        var result = sut.Get();

        result
            .Should()
            .NotBeNullOrEmpty();
    }
}
