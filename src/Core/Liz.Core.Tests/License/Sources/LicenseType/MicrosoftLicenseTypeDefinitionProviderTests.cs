using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.License.Sources.LicenseType;
using Xunit;

namespace Liz.Core.Tests.License.Sources.LicenseType;

public class MicrosoftLicenseTypeDefinitionProviderTests
{
    [Fact]
    public void Always_Provides_Something()
    {
        var context = ArrangeContext<MicrosoftLicenseTypeDefinitionProvider>.Create();
        var sut = context.Build();

        var result = sut.Get();

        result
            .Should()
            .NotBeNullOrEmpty();
    }

    [Fact]
    public void Provides_Types()
    {
        var context = ArrangeContext<MicrosoftLicenseTypeDefinitionProvider>.Create();
        var sut = context.Build();

        var result = sut.Get();

        result
            .Should()
            .AllSatisfy(definition => definition
                .LicenseType
                .Should()
                .Contain("MS-"));
    }
}
