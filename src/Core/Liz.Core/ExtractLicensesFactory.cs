using Liz.Core.CliTool;
using Liz.Core.Extract;
using Liz.Core.Extract.Contracts;
using Liz.Core.License;
using Liz.Core.License.Contracts;
using Liz.Core.License.Sources;
using Liz.Core.Logging.Contracts;
using Liz.Core.Logging.Null;
using Liz.Core.PackageReferences;
using Liz.Core.PackageReferences.DotnetCli;
using Liz.Core.PackageReferences.NuGetCli;
using Liz.Core.Progress;
using Liz.Core.Projects;
using Liz.Core.Settings;
using Liz.Core.Utils;
using Liz.Core.Utils.Wrappers;
using SlnParser;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace Liz.Core;

/// <inheritdoc />
[ExcludeFromCodeCoverage] // Factory
public sealed class ExtractLicensesFactory : IExtractLicensesFactory
{
    /// <inheritdoc />
    public IExtractLicenses Create(
        ExtractLicensesSettings settings, 
        ILoggerProvider? loggerProvider = null,
        IProgressHandler? progressHandler = null)
    {
        var logger = GetLogger(settings, loggerProvider);
        var fileSystem = new FileSystem();
        var cliToolExecutor = new DefaultCliToolExecutor(logger);
        var httpClient = new HttpClientWrapper();

        var getProjects = new GetProjectsViaSlnParser(new SolutionParser(), fileSystem);
        var parseDotnetListPackage = new ParseDotnetListPackageResult();
        var parsePackagesConfigFile = new ParsePackagesConfigFile();

        var getPackageReferencesDotnetCli = new GetPackageReferencesViaDotnetCli(cliToolExecutor, parseDotnetListPackage);
        var getPackageReferencesPackagesConfig = new GetPackageReferencesViaPackagesConfig(logger, fileSystem, parsePackagesConfigFile);
        
        var getPackageReferences = new GetPackageReferencesFacade(logger, getPackageReferencesDotnetCli, getPackageReferencesPackagesConfig);

        var provideTemporaryDirectories = new ProvideTemporaryDirectories(settings, fileSystem);

        var downloadPackageReferencesDotnet = new DownloadPackageReferencesViaDotnetCli(cliToolExecutor);
        var downloadPackageReferencesNuget = new DownloadPackageReferencesViaNugetCli(cliToolExecutor);

        var downloadPackageReferences = new DownloadPackageReferencesFacade(
            provideTemporaryDirectories,
            logger,
            downloadPackageReferencesDotnet,
            downloadPackageReferencesNuget);

        var enrichLicenseInformation = new ILicenseInformationSource[]
        {
            new LicenseElementLicenseInformationSource(logger, fileSystem),
            new LicenseFileLicenseInformationSource(logger),
            new LicenseUrlElementLicenseInformationSource(logger, fileSystem, httpClient)
        };

        var getLicenseInformationFromArtifact = new GetLicenseInformationFromArtifact(
            fileSystem, 
            logger, 
            enrichLicenseInformation);

        var getDownloadedPackageReferenceArtifact = new GetDownloadedPackageReferenceArtifact(
            provideTemporaryDirectories, 
            fileSystem);
        
        var enrichPackageReferenceWithLicenseInformation = new EnrichPackageReferenceWithLicenseInformation(
            getLicenseInformationFromArtifact, 
            logger,
            getDownloadedPackageReferenceArtifact);

        var packageReferencePrinter = new PackageReferencePrinter(settings, logger);

        var extractLicenses = new ExtractLicenses(
            settings,
            logger,
            progressHandler,
            getProjects,
            getPackageReferences,
            enrichPackageReferenceWithLicenseInformation,
            provideTemporaryDirectories,
            downloadPackageReferences,
            packageReferencePrinter);
        
        return extractLicenses;
    }

    private static ILogger GetLogger(
        ExtractLicensesSettings settings, 
        ILoggerProvider? loggerProvider)
    {
        loggerProvider ??= new NullLoggerProvider();

        var logger = loggerProvider.Get(settings.LogLevel);
        return logger;
    }
}
