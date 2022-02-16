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
class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Secret] readonly string? CodeCovToken;
    
    [GitVersion] readonly GitVersion? GitVersion;
    [Solution] readonly Solution? Solution;

    static AbsolutePath SourceDirectory => RootDirectory / "src";
    static AbsolutePath OutputDirectory => RootDirectory / "output";
    static AbsolutePath PackageOutputDirectory => OutputDirectory / "packages";

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
                    .EnableNoBuild()
                    .EnableNoRestore());
        });

    Target Pack => _ => _
        .DependsOn(Test)
        .DependsOn(Compile)
        .Executes(() =>
        {
            if (Solution == null)
                throw new InvalidOperationException("Solution cannot be null!");
            if (GitVersion == null)
                throw new InvalidOperationException("GitVersion cannot be null!");

            var packableProjects = Solution
                .AllProjects
                .Where(project => project.GetProperty<bool>("IsPackable"));

            foreach (var project in packableProjects)
            {
                Serilog.Log.Information("Packaging project... {ProjectName}", project.Name);
                
                DotNetPack(settings => settings
                    .SetProject(project)
                    .SetOutputDirectory(PackageOutputDirectory)
                    .SetConfiguration(Configuration)
                    .EnableNoBuild()
                    .EnableNoRestore()
                    .SetVersion(GitVersion.NuGetVersionV2)
                    .SetAssemblyVersion(GitVersion.AssemblySemVer)
                    .SetFileVersion(GitVersion.AssemblySemFileVer)
                    .SetInformationalVersion(GitVersion.InformationalVersion));
            }
        });

    public static int Main() => Execute<Build>(x => x.Test);
}