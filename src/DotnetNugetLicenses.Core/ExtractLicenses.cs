using Ardalis.GuardClauses;
using DotnetNugetLicenses.Core.Contracts;
using DotnetNugetLicenses.Core.Contracts.Services;
using DotnetNugetLicenses.Core.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace DotnetNugetLicenses.Core
{
	public sealed class ExtractLicenses : IExtractLicenses
	{
        private readonly IGetProjects _getProjects;
        private readonly ILogger<ExtractLicenses> _logger;

        public ExtractLicenses(
            [NotNull] IGetProjects getProjects,
            [NotNull] ILogger<ExtractLicenses> logger)
        {
            _getProjects = getProjects ?? throw new ArgumentNullException(nameof(getProjects));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
		public void Extract(ExtractSettings settings)
		{
			Guard.Against.Null(settings, nameof(settings));

            var projects = GetProjects(settings.TargetFile);


            /*
             * -- TODO --
             * 
             * filter projects (by name with Regex)
             *
             * get dependencies (including/excluding transitive)
             *      SDK-Style csproj: DependencyGraphSpec (+ Lock-File if transitive)
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

        private IEnumerable<Project> GetProjects(IFileInfo targetFile)
        {
            _logger.LogDebug("Trying to get projects from {TargetFile}...", targetFile);

            var projects = _getProjects.GetFromFile(targetFile).ToList();
            
            _logger.LogDebug("Found following projects:\n{Projects}",
                string.Join("\n", projects.Select(project => $"\t-{project.File.FullName}")));

            return projects;
        }
    }
}
