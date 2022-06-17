using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.Extract;
using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Exceptions;
using Liz.Core.License.Contracts.Models;
using Liz.Core.PackageReferences.Contracts;
using Liz.Core.PackageReferences.Contracts.Exceptions;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Preparation.Contracts;
using Liz.Core.Preparation.Contracts.Exceptions;
using Liz.Core.Progress;
using Liz.Core.Projects.Contracts;
using Liz.Core.Projects.Contracts.Exceptions;
using Liz.Core.Projects.Contracts.Models;
using Liz.Core.Result.Contracts;
using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;
using Moq;
using System.IO.Abstractions;
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
        var resultProcessor = new Mock<IResultProcessor>();
        
        var context = CreateContext();
        context.Use<IEnumerable<IResultProcessor>>(new[] { resultProcessor.Object });
        
        var sut = context.Build();

        var project1 = new Project("Something", Mock.Of<IFileInfo>(), ProjectFormatStyle.SdkStyle);
        var project2 = new Project("Something Else", Mock.Of<IFileInfo>(), ProjectFormatStyle.NonSdkStyle);
        var projects = new List<Project> { project1, project2 };
        
        context
            .For<IGetProjects>()
            .Setup(getProjects => getProjects.GetFromFile(It.IsAny<string>()))
            .Returns(projects);

        var packageReference1 = new PackageReference("Something", "net5.0", "1.1.0")
        {
            LicenseInformation = new LicenseInformation { Url = "something" }
        };
        var packageReference2 = new PackageReference("Something.Else", "net6.0", "2.0.0")
        {
            LicenseInformation = new LicenseInformation { Url = "something" }
        };

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
        
        resultProcessor
            .Verify(processor => 
                processor.ProcessResultsAsync(It.IsAny<IEnumerable<PackageReference>>()), Times.Once);
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

        var packageReference1 = new PackageReference("Something", "net5.0", "1.1.0")
        {
            LicenseInformation = new LicenseInformation { Url = "something" }
        };
        var packageReference2 = new PackageReference("Something", "net5.0", "1.1.0")
        {
            LicenseInformation = new LicenseInformation { Url = "something" }
        };

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
    
    [Fact]
    public async Task Extract_Notifies_About_Progress()
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

        _ = await sut.ExtractAsync();

        context
            .For<IProgressHandler>()
            .Verify(progressHandler => 
                progressHandler.InitializeMainProcess(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        
        context
            .For<IProgressHandler>()
            .Verify(progressHandler => progressHandler.FinishMainProcess(), Times.Once);
        
        context
            .For<IProgressHandler>()
            .Verify(progressHandler => progressHandler.TickMainProcess(It.IsAny<string>()), Times.AtLeastOnce);
        
        context
            .For<IProgressHandler>()
            .Verify(progressHandler => progressHandler.TickCurrentSubProcess(It.IsAny<string>()), Times.AtLeastOnce);
    }
    
    [Fact]
    // https://github.com/wgnf/liz/issues/43
    public async Task Extract_Gets_Rid_Of_Internal_Project_References()
    {
        var context = CreateContext();
        var sut = context.Build();

        var project = new Project("Something", Mock.Of<IFileInfo>(), ProjectFormatStyle.SdkStyle);

        context
            .For<IGetProjects>()
            .Setup(getProjects => getProjects.GetFromFile(It.IsAny<string>()))
            .Returns(new[] { project });

        var packageReference1 = new PackageReference("Something", "net5.0", "1.1.0")
        {
            LicenseInformation = new LicenseInformation
            {
                // everything empty!
                Text = string.Empty,
                Url = string.Empty
            }
        };

        context
            .For<IGetPackageReferences>()
            .Setup(getPackageReferences =>
                getPackageReferences.GetFromProjectAsync(project, It.IsAny<bool>()))
            .ReturnsAsync(new[] { packageReference1});

        context
            .For<IEnrichPackageReferenceWithLicenseInformation>()
            .Setup(enrich => enrich.EnrichAsync(It.IsAny<PackageReference>()))
            .Returns(Task.CompletedTask);

        var result = await sut.ExtractAsync();

        result
            .Should()
            .BeEmpty();
    }
    
    [Fact]
    public async Task Extract_Calls_Preprocessors()
    {
        var preprocessorMock = new Mock<IPreprocessor>();
        
        var context = CreateContext();
        context.Use<IEnumerable<IPreprocessor>>(new[] { preprocessorMock.Object });
        
        var sut = context.Build();

        var project = new Project("Something", Mock.Of<IFileInfo>(), ProjectFormatStyle.SdkStyle);

        context
            .For<IGetProjects>()
            .Setup(getProjects => getProjects.GetFromFile(It.IsAny<string>()))
            .Returns(new[] { project });

        _ = await sut.ExtractAsync();

        preprocessorMock
            .Verify(preprocessor => preprocessor.PreprocessAsync(), Times.Once);
    }
    
    [Fact]
    public async Task Extract_Calls_Preprocessors_And_Throws_When_Something_Happens()
    {
        var preprocessorMock = new Mock<IPreprocessor>();
        preprocessorMock
            .Setup(preprocessor => preprocessor.PreprocessAsync())
            .Throws<Exception>();
        
        var context = CreateContext();
        context.Use<IEnumerable<IPreprocessor>>(new[] { preprocessorMock.Object });
        
        var sut = context.Build();

        var project = new Project("Something", Mock.Of<IFileInfo>(), ProjectFormatStyle.SdkStyle);

        context
            .For<IGetProjects>()
            .Setup(getProjects => getProjects.GetFromFile(It.IsAny<string>()))
            .Returns(new[] { project });

        await Assert.ThrowsAsync<PreparationFailedException>(() => sut.ExtractAsync());
    }
    
    [Fact]
    public async Task Extract_Fails_When_Artifact_Directory_Could_Not_Be_Determined()
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
            .For<IEnrichPackageReferenceWithArtifactDirectory>()
            .Setup(enrich => enrich.EnrichAsync(It.IsAny<PackageReference>()))
            .Throws<InvalidOperationException>();

        await Assert.ThrowsAsync<GetArtifactDirectoryFailedException>(() => sut.ExtractAsync());
    }

    [Fact]
    public async Task Extract_Deletes_Temporary_Directory()
    {
        var context = CreateContext();

        var temporaryDirectory = new Mock<IDirectoryInfo>();
        temporaryDirectory
            .SetupGet(directory => directory.Exists)
            .Returns(true);
        
        context
            .For<IProvideTemporaryDirectories>()
            .Setup(provideTemporaryDirectory => provideTemporaryDirectory.GetRootDirectory())
            .Returns(temporaryDirectory.Object);
        
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
        
        var sut = context.Build();

        await sut.ExtractAsync();
        
        temporaryDirectory.Verify(directory => directory.Delete(true), Times.Once);
    }

    private static ArrangeContext<ExtractLicenses> CreateContext()
    {
        var settingsMock = new Mock<ExtractLicensesSettingsBase>();
        settingsMock
            .Setup(settings => settings.GetTargetFile())
            .Returns("TargetFile.csproj");
        
        var resultProcessor = Mock.Of<IResultProcessor>();
        var preprocessor = Mock.Of<IPreprocessor>();

        var context = ArrangeContext<ExtractLicenses>.Create();
        
        context.Use(settingsMock.Object);
        context.Use<IEnumerable<IResultProcessor>>(new[] { resultProcessor });
        context.Use<IEnumerable<IPreprocessor>>(new[] { preprocessor });

        context
            .For<IProvideTemporaryDirectories>()
            .Setup(provideTemporaryDirectory => provideTemporaryDirectory.GetRootDirectory())
            .Returns(Mock.Of<IDirectoryInfo>());

        return context;
    }
}
