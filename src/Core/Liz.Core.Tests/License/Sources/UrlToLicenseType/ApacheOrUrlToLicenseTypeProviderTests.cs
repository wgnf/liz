using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.License.Sources.UrlToLicenseType;
using Xunit;

namespace Liz.Core.Tests.License.Sources.UrlToLicenseType;

public class ApacheOrUrlToLicenseTypeProviderTests
{
    [Fact]
    public void Provides_Mappings()
    {
        var context = ArrangeContext<ApacheOrUrlToLicenseTypeProvider>.Create();
        var sut = context.Build();

        var result = sut.Get();

        result
            .Should()
            .NotBeNullOrEmpty();
    }
}
