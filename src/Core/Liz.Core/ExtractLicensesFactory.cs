using Liz.Core.CliTool;
using Liz.Core.Extract;
using Liz.Core.License;
using Liz.Core.License.Sources;
using Liz.Core.Logging;
using Liz.Core.Logging.Null;
using Liz.Core.PackageReferences;
using Liz.Core.PackageReferences.DotnetCli;
using Liz.Core.Projects;
using Liz.Core.Settings;
using Liz.Core.Utils;
using SlnParser;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace Liz.Core;

[ExcludeFromCodeCoverage] // Factory
public sealed class ExtractLicensesFactory : IExtractLicensesFactory
{
    public IExtractLicenses Create(ExtractLicensesSettings settings, ILoggerProvider loggerProvider = null)
    {
        var logger = GetLogger(settings, loggerProvider);
        var fileSystem = new FileSystem();
        var cliToolExecutor = new DefaultCliToolExecutor(logger);

        var getProjects = new GetProjectsViaSlnParser(new SolutionParser(), fileSystem);
        var parseDotnetListPackage = new ParseDotnetListPackageResult();

        var getPackageReferencesDotnetCli = new GetPackageReferencesViaDotnetCli(cliToolExecutor, parseDotnetListPackage);
        var getPackageReferences = new GetPackageReferencesFacade(logger, getPackageReferencesDotnetCli);

        var provideTemporaryDirectory = new ProvideTemporaryDirectory(settings, fileSystem);
        var downloadPackageReference = new DownloadPackageReferenceViaDotnetAddCli(
            fileSystem, 
            provideTemporaryDirectory,
            cliToolExecutor);

        var enrichLicenseInformation = new IEnrichLicenseInformationResult[]
        {
            new EnrichLicenseInformationFromLicenseElement(logger, fileSystem),
            new EnrichLicenseInformationFromLicenseUrlElement(logger, fileSystem)
        };

        var getLicenseInformationFromArtifact = new GetLicenseInformationFromArtifact(
            fileSystem, 
            logger, 
            enrichLicenseInformation);
        
        var getLicenseInformation = new GetLicenseInformation(
            downloadPackageReference, 
            getLicenseInformationFromArtifact, 
            logger);

        var extractLicenses = new ExtractLicenses(
            settings, 
            logger, 
            getProjects, 
            getPackageReferences, 
            getLicenseInformation, 
            provideTemporaryDirectory);
        
        return extractLicenses;
    }

    private static ILogger GetLogger(
        ExtractLicensesSettings settings, 
        ILoggerProvider loggerProvider)
    {
        loggerProvider ??= new NullLoggerProvider();

        var logger = loggerProvider.Get(settings.LogLevel);
        return logger;
    }
}
