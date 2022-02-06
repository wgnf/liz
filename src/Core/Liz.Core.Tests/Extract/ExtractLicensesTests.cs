using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.Extract;
using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Exceptions;
using Liz.Core.PackageReferences.Contracts;
using Liz.Core.PackageReferences.Contracts.Exceptions;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Projects.Contracts;
using Liz.Core.Projects.Contracts.Exceptions;
using Liz.Core.Projects.Contracts.Models;
using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;
using Moq;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Xunit;

namespace Liz.Core.Tests.Extract;

public class ExtractLicensesTests
{
    [Fact]
    public async Task Extract_Fails_When_Projects_Could_Not_Be_Determined()
    {
        var context = CreateContext();
        var sut = context.Build();

        context
            .For<IGetProjects>()
            .Setup(getProjects => getProjects.GetFromFile(It.IsAny<string>()))
            .Throws<InvalidOperationException>();

        await Assert.ThrowsAsync<GetProjectsFailedException>(() => sut.ExtractAsync());
    }

    [Fact]
    public async Task Extract_Fails_When_Package_References_Could_Not_Be_Determined()
    {
        var context = CreateContext();
        var sut = context.Build();

        var projects = new List<Project>
        {
            new("Something", Mock.Of<IFileInfo>(), ProjectFormatStyle.SdkStyle),
            new("Something Else", Mock.Of<IFileInfo>(), ProjectFormatStyle.NonSdkStyle)
        };
        context
            .For<IGetProjects>()
            .Setup(getProjects => getProjects.GetFromFile(It.IsAny<string>()))
            .Returns(projects);

        context
            .For<IGetPackageReferences>()
            .Setup(getPackageReferences =>
                getPackageReferences.GetFromProjectAsync(It.IsAny<Project>(), It.IsAny<bool>()))
            .Throws<InvalidOperationException>();

        await Assert.ThrowsAsync<GetPackageReferencesFailedException>(() => sut.ExtractAsync());
    }
    
    [Fact]
    public async Task Extract_Fails_When_License_Information_Could_Not_Be_Determined()
    {
        var context = CreateContext();
        var sut = context.Build();

        var project1 = new Project("Something", Mock.Of<IFileInfo>(), ProjectFormatStyle.SdkStyle);
        var project2 = new Project("Something Else", Mock.Of<IFileInfo>(), ProjectFormatStyle.NonSdkStyle);
        var projects = new List<Project> { project1, project2 };
        
        context
            .For<IGetProjects>()
            .Setup(getProjects => getProjects.GetFromFile(It.IsAny<string>()))
            .Returns(projects);

        var packageReference1 = new PackageReference("Something", "net5.0", "1.1.0");
        var packageReference2 = new PackageReference("Something.Else", "net6.0", "2.0.0");

        context
            .For<IGetPackageReferences>()
            .Setup(getPackageReferences =>
                getPackageReferences.GetFromProjectAsync(project1, It.IsAny<bool>()))
            .ReturnsAsync(new[] { packageReference1 });
        
        context
            .For<IGetPackageReferences>()
            .Setup(getPackageReferences =>
                getPackageReferences.GetFromProjectAsync(project2, It.IsAny<bool>()))
            .ReturnsAsync(new[] { packageReference2 });

        context
            .For<IEnrichPackageReferenceWithLicenseInformation>()
            .Setup(enrich => enrich.EnrichAsync(It.IsAny<PackageReference>()))
            .Throws<InvalidOperationException>();

        await Assert.ThrowsAsync<GetLicenseInformationFailedException>(() => sut.ExtractAsync());
    }
    
    [Fact]
    public async Task Extract()
    {
        var context = CreateContext();
        var sut = context.Build();

        var project1 = new Project("Something", Mock.Of<IFileInfo>(), ProjectFormatStyle.SdkStyle);
        var project2 = new Project("Something Else", Mock.Of<IFileInfo>(), ProjectFormatStyle.NonSdkStyle);
        var projects = new List<Project> { project1, project2 };
        
        context
            .For<IGetProjects>()
            .Setup(getProjects => getProjects.GetFromFile(It.IsAny<string>()))
            .Returns(projects);

        var packageReference1 = new PackageReference("Something", "net5.0", "1.1.0");
        var packageReference2 = new PackageReference("Something.Else", "net6.0", "2.0.0");

        context
            .For<IGetPackageReferences>()
            .Setup(getPackageReferences =>
                getPackageReferences.GetFromProjectAsync(project1, It.IsAny<bool>()))
            .ReturnsAsync(new[] { packageReference1 });
        
        context
            .For<IGetPackageReferences>()
            .Setup(getPackageReferences =>
                getPackageReferences.GetFromProjectAsync(project2, It.IsAny<bool>()))
            .ReturnsAsync(new[] { packageReference2 });

        context
            .For<IEnrichPackageReferenceWithLicenseInformation>()
            .Setup(enrich => enrich.EnrichAsync(It.IsAny<PackageReference>()))
            .Returns(Task.CompletedTask);

        var result = await sut.ExtractAsync();

        result
            .Should()
            .Contain(new[] { packageReference1, packageReference2 });
    }

    [Fact]
    // https://github.com/wgnf/liz/issues/8
    public async Task Extract_Makes_Package_References_Distinct()
    {
        var context = CreateContext();
        var sut = context.Build();

        var project = new Project("Something", Mock.Of<IFileInfo>(), ProjectFormatStyle.SdkStyle);

        context
            .For<IGetProjects>()
            .Setup(getProjects => getProjects.GetFromFile(It.IsAny<string>()))
            .Returns(new[] { project });

        var packageReference1 = new PackageReference("Something", "net5.0", "1.1.0");
        var packageReference2 = new PackageReference("Something", "net5.0", "1.1.0");

        context
            .For<IGetPackageReferences>()
            .Setup(getPackageReferences =>
                getPackageReferences.GetFromProjectAsync(project, It.IsAny<bool>()))
            .ReturnsAsync(new[] { packageReference1, packageReference2 });

        context
            .For<IEnrichPackageReferenceWithLicenseInformation>()
            .Setup(enrich => enrich.EnrichAsync(It.IsAny<PackageReference>()))
            .Returns(Task.CompletedTask);

        var expectedResult = new[] { new PackageReference("Something", "net5.0", "1.1.0") };

        var result = await sut.ExtractAsync();

        result
            .Should()
            .BeEquivalentTo(expectedResult);
    }

    private static ArrangeContext<ExtractLicenses> CreateContext()
    {
        var settings = new ExtractLicensesSettings("TargetFile.csproj");
        
        var context = ArrangeContext<ExtractLicenses>.Create();
        context.Use(settings);

        context
            .For<IProvideTemporaryDirectories>()
            .Setup(provideTemporaryDirectory => provideTemporaryDirectory.GetRootDirectory())
            .Returns(Mock.Of<IDirectoryInfo>());

        return context;
    }
}
