using Ardalis.GuardClauses;
using DotnetNugetLicenses.Core.Contracts.Services;
using DotnetNugetLicenses.Core.Models;
using JetBrains.Annotations;
using SlnParser.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace DotnetNugetLicenses.Core.Services
{
    public sealed class DefaultGetProjects : IGetProjects
    {
        private readonly IFileSystem _fileSystem;
        private readonly ISolutionParser _solutionParser;

        public DefaultGetProjects(
            [NotNull] ISolutionParser solutionParser,
            [NotNull] IFileSystem fileSystem)
        {
            _solutionParser = Guard.Against.Null(solutionParser, nameof(solutionParser));
            _fileSystem = Guard.Against.Null(fileSystem, nameof(fileSystem));
        }

        public IEnumerable<Project> GetFromFile(IFileInfo targetFile)
        {
            Guard.Against.Null(targetFile, nameof(targetFile));

            CheckProvidedTargetFile(targetFile);

            var projects = GetProjects(targetFile);
            return projects;
        }

        private static void CheckProvidedTargetFile(IFileSystemInfo targetFile)
        {
            if (!targetFile.Exists)
                throw new FileNotFoundException($"The provided target-file '{targetFile.FullName}' could not be found");

            if (targetFile.Extension is not (".sln" or ".csproj" or ".fsproj"))
                throw new ArgumentException(
                    $"The provided target-file '{targetFile.FullName}' is not of the right type " +
                    "(only 'sln', 'csproj' and 'fsproj' is supported");
        }

        private IEnumerable<Project> GetProjects(IFileInfo targetFile)
        {
            return targetFile.Extension switch
            {
                ".csproj" or ".fsproj" => GetProjectFromProjectFile(targetFile),
                ".sln" => GetProjectFromSolutionFile(targetFile),
                _ => throw new ArgumentOutOfRangeException(nameof(targetFile),
                    $"Target-file-extension was expected to be 'csproj', 'fsproj' or 'sln' but found '{targetFile.Extension}'")
            };
        }

        private IEnumerable<Project> GetProjectFromProjectFile(IFileInfo targetFile)
        {
            // NOTE: we just assume that the Project-Name == File-Name here (it's not all too important, because it'll only be used for logs) 
            var projectName = _fileSystem.Path.GetFileNameWithoutExtension(targetFile.FullName);

            // NOTE: we only return the target-file, because there is nothing else to gather in this case...
            return new[] { new Project(projectName, targetFile) };
        }

        private IEnumerable<Project> GetProjectFromSolutionFile(IFileSystemInfo fileInfo)
        {
            var solution = _solutionParser.Parse(fileInfo.FullName);
            var projects = solution
                .AllProjects
                .OfType<SolutionProject>()
                .Where(project => project.File.Extension is ".csproj" or ".fsproj")
                .Where(project => _fileSystem.File.Exists(project.File.FullName))
                .Select(project => new Project(project.Name, _fileSystem.FileInfo.FromFileName(project.File.FullName)))
                .ToList();

            return projects;
        }
    }
}
