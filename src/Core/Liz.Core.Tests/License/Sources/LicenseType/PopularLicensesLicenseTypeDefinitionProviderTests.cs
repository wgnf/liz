﻿using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.License.Sources.LicenseType;
using Xunit;

// ReSharper disable StringLiteralTypo

namespace Liz.Core.Tests.License.Sources.LicenseType;

public class PopularLicensesLicenseTypeDefinitionProviderTests
{
    [Fact]
    public void Always_Provides_Something()
    {
        var context = ArrangeContext<PopularLicensesLicenseTypeDefinitionProvider>.Create();
        var sut = context.Build();

        var result = sut.Get();

        result
            .Should()
            .NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("MIT")]
    [InlineData("Unlicense")]
    [InlineData("WTFPL")]
    [InlineData("ICU")]
    [InlineData("CPL-1.0")]
    public void Provides_Definitions_For_Widely_Used_License_Types(string licenseType)
    {
        var context = ArrangeContext<PopularLicensesLicenseTypeDefinitionProvider>.Create();
        var sut = context.Build();
        
        var result = sut.Get();

        result
            .Select(definition => definition.LicenseType)
            .Should()
            .Contain(licenseType);
    }
}
