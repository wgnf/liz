using FluentAssertions;
using Liz.Core.CliTool;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Utils;
using System.IO.Abstractions;
using Xunit;

namespace Liz.Core.IntegrationTests;

public class FindPackageReferenceArtifactTests
{
    [Theory]
    [InlineData("System.IO", "4.3.0", true)]
    [InlineData("abc", "1.33.7", false)]
    public async Task Finds_Package_Reference_Artifact(string packageName, string packageVersion, bool expectedOutcome)
    {
        var logger = new Logging.Null.NullLogger();
        var cliExecutor = new DefaultCliToolExecutor(logger);
        var provideNugetCaches = new ProvideNugetCacheDirectories(cliExecutor);
        var fileSystem = new FileSystem();

        var findPackageReferenceArtifact = new FindPackageReferenceArtifact(provideNugetCaches, fileSystem, logger);

        var packageReference = new PackageReference(packageName, "net6.0", packageVersion);

        var result = await findPackageReferenceArtifact.TryGetArtifactAsync(packageReference);

        result
            .HasResult
            .Should()
            .Be(expectedOutcome);
    }
}
