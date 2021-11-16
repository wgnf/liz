using DotnetNugetLicenses.Core.Extract;
using DotnetNugetLicenses.Core.Logging;
using DotnetNugetLicenses.Core.Logging.Null;
using DotnetNugetLicenses.Core.PackageReferences;
using DotnetNugetLicenses.Core.PackageReferences.DotnetCli;
using DotnetNugetLicenses.Core.Projects;
using DotnetNugetLicenses.Core.Settings;
using SlnParser;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace DotnetNugetLicenses.Core;

[ExcludeFromCodeCoverage] // Factory
public sealed class ExtractLicensesFactory : IExtractLicensesFactory
{
    public IExtractLicenses Create(ExtractLicensesSettings settings, ILoggerProvider loggerProvider = null)
    {
        var logger = GetLogger(settings, loggerProvider);
        var fileSystem = new FileSystem();

        var getProjects = new GetProjectsViaSlnParser(new SolutionParser(), fileSystem);

        var getPackageReferencesDotnetCli = new GetPackageReferencesViaDotnetCli();
        var getPackageReferences = new GetPackageReferencesFacade(logger, fileSystem, getPackageReferencesDotnetCli);
        
        var extractLicenses = new ExtractLicenses(settings, logger, getProjects, getPackageReferences);
        
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
