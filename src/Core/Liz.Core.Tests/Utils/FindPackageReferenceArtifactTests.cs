using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Utils;
using Liz.Core.Utils.Contracts;
using Moq;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Liz.Core.Tests.Utils;

public class FindPackageReferenceArtifactTests
{
    [Fact]
    public async Task Fails_On_Invalid_Parameters()
    {
        var context = ArrangeContext<FindPackageReferenceArtifact>.Create();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.TryGetArtifactAsync(null!));
    }

    [Fact]
    public async Task Succeeds_To_Find_Artifact_Directory()
    {
        const string nugetCache = "C:/nuget/cache";
        var packageReference = new PackageReference("Test", "net6.0", "1.2.3");

        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddDirectory($"{nugetCache}/{packageReference.Name.ToLower()}/{packageReference.Version.ToLower()}");
        
        var context = ArrangeContext<FindPackageReferenceArtifact>.Create();
        context.Use<IFileSystem>(mockFileSystem);

        context
            .For<IProvideNugetCacheDirectories>()
            .Setup(provider => provider.GetAsync())
            .ReturnsAsync(new[] { nugetCache });
        
        var sut = context.Build();

        var result = await sut.TryGetArtifactAsync(packageReference);

        result
            .HasResult
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public async Task Fails_To_Find_Artifact_Directory_When_The_Directory_Does_Not_Exist()
    {
        const string nugetCache = "C:/nuget/cache";
        var packageReference = new PackageReference("Test", "net6.0", "1.2.3");

        var mockFileSystem = new MockFileSystem();
        // this is another directory as expected
        mockFileSystem.AddDirectory($"{nugetCache}/{packageReference.Name.ToLower()}/something/{packageReference.Version.ToLower()}");
        
        var context = ArrangeContext<FindPackageReferenceArtifact>.Create();
        context.Use<IFileSystem>(mockFileSystem);

        context
            .For<IProvideNugetCacheDirectories>()
            .Setup(provider => provider.GetAsync())
            .ReturnsAsync(new[] { nugetCache });
        
        var sut = context.Build();

        var result = await sut.TryGetArtifactAsync(packageReference);

        result
            .HasResult
            .Should()
            .BeFalse();
    }
}
