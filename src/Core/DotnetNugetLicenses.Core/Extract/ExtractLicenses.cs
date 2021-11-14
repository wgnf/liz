using DotnetNugetLicenses.Core.Logging;
using DotnetNugetLicenses.Core.Projects;
using DotnetNugetLicenses.Core.Settings;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotnetNugetLicenses.Core.Extract;

internal sealed class ExtractLicenses : IExtractLicenses
{
    private readonly ExtractLicensesSettings _settings;
    private readonly IGetProjects _getProjects;
    private readonly ILogger _logger;

    public ExtractLicenses(
        [NotNull] ExtractLicensesSettings settings,
        [NotNull] IGetProjects getProjects,
        [NotNull] ILogger logger)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _getProjects = getProjects ?? throw new ArgumentNullException(nameof(getProjects));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Extract()
    {
        try
        {
            var projects = GetProjects(_settings.TargetFile);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Error occured while extracting licenses for '{_settings.TargetFile}'", ex);
        }


        /*
         * -- TODO --
         * 
         * filter projects (by name with Regex)
         *
         * get dependencies (including/excluding transitive)
         *      SDK-Style csproj: dotnet list package (--include-transitive) and parse it's output
         *      Old-Style csproj: packages.config (which is already including transitive dependencies - warning when transitive=false!!) without development-dependencies!
         * include manual packages (with URL/License-Text)
         * filter dependencies (by name/version with Regex)
         * make dependencies unique (according to settings) 
         *
         * get information for dependencies (however this works...) including raw text if requested
         * include manual License-URL to License Mappings
         *
         * validate dependencies with their licenses (with the provided allowed licenses)
         *
         *
         * in Tool (according to parameters):
         * - print Package-Name + License-Type + License-URL
         * - export License-Texts (package-name as the File-Name) into a specified directory
         * - export Package + License-Details in a JSON-Format (including/excluding Raw Text)
         * 
         */
    }

    private IEnumerable<Project> GetProjects(string targetFile)
    {
        try
        {
            _logger.LogDebug($"Trying to get projects from {targetFile}...");

            var projects = _getProjects.GetFromFile(targetFile).ToList();

            var foundProjectsLogString = string.Join("\n", projects.Select(project => $"\t- {project.File.FullName}"));
            _logger.LogDebug($"Found following projects:\n{foundProjectsLogString}");

            return projects;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Could not determine projects for '{targetFile}'", ex);
        }
    }
}
