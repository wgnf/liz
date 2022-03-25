using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.License.Sources.LicenseType;
using Xunit;

namespace Liz.Core.Tests.License.Sources.LicenseType;

public class LicenseTypeDefinitionProviderTests
{
    [Fact]
    public void Always_Provides_Something()
    {
        var context = ArrangeContext<LicenseTypeDefinitionProvider>.Create();
        var sut = context.Build();

        var result = sut.Get();

        result
            .Should()
            .NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("MIT")]
    [InlineData("GPL-3.0")]
    [InlineData("Apache-2.0")]
    [InlineData("BSD-2-Clause")]
    [InlineData("BSD-3-Clause")]
    [InlineData("LGPL-3.0")]
    [InlineData("MS-PL")]
    [InlineData("Unlicense")]
    public void Provides_Definitions_For_Widely_Used_License_Types(string licenseType)
    {
        var context = ArrangeContext<LicenseTypeDefinitionProvider>.Create();
        var sut = context.Build();
        
        var result = sut.Get();

        result
            .Select(definition => definition.LicenseType)
            .Should()
            .Contain(licenseType);
    }
}
