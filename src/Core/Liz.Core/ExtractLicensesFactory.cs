using Liz.Core.CliTool;
using Liz.Core.Extract;
using Liz.Core.Extract.Contracts;
using Liz.Core.License;
using Liz.Core.License.Contracts;
using Liz.Core.License.Sources.LicenseInformation;
using Liz.Core.License.Sources.LicenseType;
using Liz.Core.License.Sources.UrlToLicenseType;
using Liz.Core.Logging.Contracts;
using Liz.Core.Logging.Null;
using Liz.Core.PackageReferences;
using Liz.Core.PackageReferences.DotnetCli;
using Liz.Core.PackageReferences.NuGetCli;
using Liz.Core.Preparation;
using Liz.Core.Preparation.Contracts;
using Liz.Core.Progress;
using Liz.Core.Projects;
using Liz.Core.Result;
using Liz.Core.Result.Contracts;
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
        ExtractLicensesSettingsBase settings, 
        ILoggerProvider? loggerProvider = null,
        LogLevel logLevel = LogLevel.Information,
        IProgressHandler? progressHandler = null)
    {
        settings.EnsureValidity();
        
        var logger = GetLogger(logLevel, loggerProvider);
        var fileSystem = new FileSystem();
        var cliToolExecutor = new DefaultCliToolExecutor(logger);
        var httpClient = new HttpClientWrapper();
        var fileContentProvider = new FileContentProvider(fileSystem, httpClient);

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

        var licenseInformationSources = new ILicenseInformationSource[]
        {
            new LicenseElementLicenseInformationSource(logger, fileSystem),
            new LicenseFileLicenseInformationSource(logger),
            new LicenseUrlElementLicenseInformationSource(logger, fileSystem, httpClient),
            new LicenseTypeFromTextLicenseInformationSource(
                new ILicenseTypeDefinitionProvider[]
                {
                    new LicenseTypeDefinitionFromSettingsProvider(settings),
                    new PopularLicensesLicenseTypeDefinitionProvider(),
                    new SpecialLicenseTypeDefinitionProvider(),
                    
                    new ApacheLicenseTypeDefinitionProvider(),
                    new BsdLicenseTypeDefinitionProvider(),
                    new CddlLicenseTypeDefinitionProvider(),
                    new EplLicenseTypeDefinitionProvider(),
                    new EuplLicenseTypeDefinitionProvider(),
                    new GnuLicenseTypeDefinitionProvider(),
                    new MicrosoftLicenseTypeDefinitionProvider(),
                    new MplLicenseTypeDefinitionProvider(),
                    new NplLicenseTypeDefinitionProvider()
                }, 
                logger),
            new UrlToLicenseTypeMappingLicenseInformationSource(
                new IUrlToLicenseTypeMappingProvider[]
                {
                    new UrlToLicenseTypeFromSettingsProvider(settings),
                    
                    new ChooseALicenseUrlToLicenseTypeProvider(),
                    new OpenSourceOrgUrlToLicenseTypeProvider(),
                    new MicrosoftUrlToLicenseTypeProvider(),
                    new ApacheOrUrlToLicenseTypeProvider()
                },
                logger)
        };

        var resultProcessors = new IResultProcessor[]
        {
            new PrintPackageDetailsResultProcessor(settings, logger),
            new PrintPackageIssuesResultProcessor(settings, logger)
        };

        var getLicenseInformationFromArtifact = new GetLicenseInformationFromArtifact(
            fileSystem, 
            logger, 
            licenseInformationSources);

        var getDownloadedPackageReferenceArtifact = new GetDownloadedPackageReferenceArtifact(
            provideTemporaryDirectories, 
            fileSystem);
        
        var enrichPackageReferenceWithLicenseInformation = new EnrichPackageReferenceWithLicenseInformation(
            getLicenseInformationFromArtifact, 
            logger,
            getDownloadedPackageReferenceArtifact);

        var preprocessors = new IPreprocessor[]
        {
            new DeserializeLicenseTypeDefinitionsPreprocessor(settings, logger, fileContentProvider),
            new DeserializeUrlToLicenseTypeMappingPreprocessor(settings, logger, fileContentProvider)
        };

        var extractLicenses = new ExtractLicenses(
            settings,
            logger,
            progressHandler,
            getProjects,
            getPackageReferences,
            enrichPackageReferenceWithLicenseInformation,
            provideTemporaryDirectories,
            downloadPackageReferences,
            resultProcessors,
            preprocessors);
        
        return extractLicenses;
    }

    private static ILogger GetLogger(
        LogLevel logLevel, 
        ILoggerProvider? loggerProvider)
    {
        loggerProvider ??= new NullLoggerProvider();

        var logger = loggerProvider.Get(logLevel);
        return logger;
    }
}
