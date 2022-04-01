using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.License.Sources.LicenseType;
using Xunit;

namespace Liz.Core.Tests.License.Sources.LicenseType;

public class BsdLicenseTypeDefinitionProviderTests
{
    [Fact]
    public void Always_Provides_Something()
    {
        var context = ArrangeContext<BsdLicenseTypeDefinitionProvider>.Create();
        var sut = context.Build();

        var result = sut.Get();

        result
            .Should()
            .NotBeNullOrEmpty();
    }

    [Fact]
    public void Provides_Types()
    {
        var context = ArrangeContext<BsdLicenseTypeDefinitionProvider>.Create();
        var sut = context.Build();

        var result = sut.Get();

        result
            .Should()
            .AllSatisfy(definition => definition
                .LicenseType
                .Should()
                .ContainAll("BSD", "Clause"));
    }
}
