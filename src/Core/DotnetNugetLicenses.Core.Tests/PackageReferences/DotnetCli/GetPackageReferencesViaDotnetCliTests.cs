﻿using ArrangeContext.Moq;
using DotnetNugetLicenses.Core.CliTool;
using DotnetNugetLicenses.Core.PackageReferences;
using DotnetNugetLicenses.Core.PackageReferences.DotnetCli;
using DotnetNugetLicenses.Core.Projects;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Xunit;

namespace DotnetNugetLicenses.Core.Tests.PackageReferences.DotnetCli;

public class GetPackageReferencesViaDotnetCliTests
{
    [Fact]
    public async Task GetFromProject_Should_Throw_On_Invalid()
    {
        var context = new ArrangeContext<GetPackageReferencesViaDotnetCli>();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetFromProjectAsync(null!, false));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetFromProject(bool includeTransitive)
    {
        var context = new ArrangeContext<GetPackageReferencesViaDotnetCli>();
        var sut = context.Build();

        var expectedPackageReferences = new List<PackageReference> { new("something", "something", "something") };

        context
            .For<IParseDotnetListPackageResult>()
            .Setup(parser => parser.Parse(It.IsAny<string>()))
            .Returns(expectedPackageReferences);

        var project = new Project("Something", Mock.Of<IFileInfo>(), ProjectFormatStyle.SdkStyle);
        var packageReferences = await sut.GetFromProjectAsync(project, includeTransitive);

        packageReferences
            .Should()
            .BeEquivalentTo(expectedPackageReferences);

        context
            .For<ICliToolExecutor>()
            .Verify(executor => executor.ExecuteAsync(
                    It.Is<string>(fileName => fileName == "dotnet"),
                    It.Is<string>(arguments => arguments.Contains("restore"))),
                Times.Once);

        context
            .For<ICliToolExecutor>()
            .Verify(executor => executor.ExecuteWithResultAsync(
                    It.Is<string>(fileName => fileName == "dotnet"),
                    It.Is<string>(arguments =>
                        arguments.Contains("list") &&
                        arguments.Contains("package") &&
                        (!includeTransitive || arguments.Contains("include-transitive")))),
                Times.Once);
    }
}