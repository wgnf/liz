using System.Diagnostics.CodeAnalysis;
using Liz.Nuke;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

// ReSharper disable UnusedMember.Local

namespace Liz.Build;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
[SuppressMessage("Performance", "CA1822:Mark members as static")]
class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("NuGet API Key for Liz Packages", Name = "NUGET_API_KEY_LIZ")]
    readonly string? NuGetApiKeyLiz;

    [Parameter("NuGet API Key for the 'Cake.ExtractLicenses' Package", Name = "NUGET_API_KEY_CAKE")]
    readonly string? NuGetApiKeyCake;
    
    const string NuGetSource = "https://api.nuget.org/v3/index.json";
    
    [GitVersion] readonly GitVersion? GitVersion;
    [Solution] readonly Solution? Solution;

    static AbsolutePath SourceDirectory => RootDirectory / "src";
    static AbsolutePath OutputDirectory => RootDirectory / "output";
    static AbsolutePath PackageOutputDirectory => OutputDirectory / "packages";
    static AbsolutePath TestDataDirectory => RootDirectory / "test-data";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory
                .GlobDirectories("**/bin", "**/obj")
                .ForEach(DeleteDirectory);

            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(settings =>
                settings.SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Clean)
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(settings =>
                settings
                    .SetProjectFile(Solution)
                    .SetConfiguration(Configuration)
                    .SetAssemblyVersion(GitVersion?.AssemblySemVer)
                    .SetFileVersion(GitVersion?.AssemblySemFileVer)
                    .SetInformationalVersion(GitVersion?.InformationalVersion)
                    .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(settings =>
                settings
                    .SetProjectFile(Solution)
                    .SetConfiguration(Configuration)
                    .EnableCollectCoverage()
                    .EnableBlameHang()
                    .SetBlameHangTimeout("60s")
                    .SetCoverletOutputFormat(CoverletOutputFormat.cobertura)
                    .SetDataCollector("XPlat Code Coverage")
                    .SetFilter("FullyQualifiedName!~IntegrationTests")
                    .EnableNoBuild()
                    .EnableNoRestore());
        });

    Target Pack => _ => _
        .DependsOn(Test)
        .DependsOn(Compile)
        .Executes(() =>
        {
            var packableProjects = Solution?
                .AllProjects
                .Where(project => project.GetProperty<bool>("IsPackable")) ?? Enumerable.Empty<Project>();

            foreach (var project in packableProjects)
            {
                Serilog.Log.Information("Packaging project '{ProjectName}'...", project.Name);
                
                DotNetPack(settings => settings
                    .SetProject(project)
                    .SetOutputDirectory(PackageOutputDirectory)
                    .SetConfiguration(Configuration)
                    .EnableNoBuild()
                    .EnableNoRestore()
                    .SetVersion(GitVersion?.NuGetVersionV2)
                    .SetAssemblyVersion(GitVersion?.AssemblySemVer)
                    .SetFileVersion(GitVersion?.AssemblySemFileVer)
                    .SetInformationalVersion(GitVersion?.InformationalVersion));
            }
        });

    Target Publish => _ => _
        .DependsOn(Pack)
        .Requires(() => !string.IsNullOrWhiteSpace(NuGetApiKeyLiz))
        .Requires(() => !string.IsNullOrWhiteSpace(NuGetApiKeyCake))
        .Executes(() =>
        {
            var packageFiles = PackageOutputDirectory.GlobFiles("*.nupkg");

            foreach (var packageFile in packageFiles)
            {
                Serilog.Log.Information("Pushing '{PackageName}'...", packageFile.Name);

                var apiKey = packageFile.NameWithoutExtension.StartsWith("Cake.ExtractLicenses") 
                    ? NuGetApiKeyCake 
                    : NuGetApiKeyLiz;

                DotNetNuGetPush(settings => settings
                    .SetApiKey(apiKey)
                    .SetSymbolApiKey(apiKey)
                    
                    .SetTargetPath(packageFile)

                    .SetSource(NuGetSource)
                    .SetSymbolSource(NuGetSource));
            }
        });

    Target ExtractLicenses => _ => _
        .Executes(async () =>
        {
            await ExtractLicensesTasks.ExtractLicensesAsync(settings => settings
                .SetTargetFile(TestDataDirectory / "sln" / "TestingGround.sln")
                .EnableIncludeTransitiveDependencies());
        });

    public static int Main() => Execute<Build>(x => x.Test);
}