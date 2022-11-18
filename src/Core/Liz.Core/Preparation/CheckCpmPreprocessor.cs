using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.Preparation.Contracts;
using Liz.Core.Preparation.Contracts.Models;
using Liz.Core.Settings;
using System.IO.Abstractions;
using System.Xml.Linq;

namespace Liz.Core.Preparation;

/// <summary>
///     preprocessor that checks if the source-code is managed using Central Package Management (CPM) and writes it to the current
///     <see cref="SourceInfo"/> object so that it can be used in other components
/// </summary>
internal sealed class CheckCpmPreprocessor : IPreprocessor
{
    private readonly SourceInfo _sourceInfo;
    private readonly ExtractLicensesSettingsBase _settings;
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;

    public CheckCpmPreprocessor(
        SourceInfo sourceInfo,
        ExtractLicensesSettingsBase settings,
        ILogger logger,
        IFileSystem fileSystem)
    {
        _sourceInfo = sourceInfo ?? throw new ArgumentNullException(nameof(sourceInfo));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }
    
    public Task PreprocessAsync()
    {
        var targetFile = _settings.GetTargetFile();
        var targetFileInfo = _fileSystem.FileInfo.FromFileName(targetFile);
        var currentDirectory = targetFileInfo.Directory;

        while (currentDirectory != null)
        {
            if (IsCpmEnabledInDirectory(currentDirectory))
            {
                _logger.LogInformation("CPM is enabled for this target-file!");
                _sourceInfo.IsCpmEnabled = true;
                break;
            }
            
            currentDirectory = currentDirectory.Parent;
        }
        
        return Task.CompletedTask;
    }

    // c.f.: https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management#enabling-central-package-management
    private bool IsCpmEnabledInDirectory(IDirectoryInfo directoryInfo)
    {
        var directoryFiles = directoryInfo.GetFiles();
        var candidatePackagesProps = directoryFiles.FirstOrDefault(file => file.Name.Equals("Directory.Packages.props"));

        return candidatePackagesProps != null && IsCpmEnabledThroughPropsFile(candidatePackagesProps);
    }

    // if the file was found we have to check if CPM is actually activated
    private bool IsCpmEnabledThroughPropsFile(IFileInfo propsFile)
    {
        var propsFileContent = _fileSystem.File.ReadAllText(propsFile.FullName);
        var xmlDocument = XDocument.Parse(propsFileContent);

        var enablingElement = xmlDocument.Descendants("ManagePackageVersionsCentrally").FirstOrDefault();
        var importElement = xmlDocument.Descendants("Import").FirstOrDefault();
        
        // if we have an enabling element we can check if its available
        if (enablingElement != null) return enablingElement.Value.ToLower().Equals("true");

        // when theres no enabling element and no import (without the proper Attribute), CPM is not enabled!
        var importProject = importElement?.Attribute("Project");
        if (importProject == null || string.IsNullOrWhiteSpace(importProject.Value)) return false;
        
        // we get here when the 'importElement' is not null, so we can follow that to see if something is enabled there...
        // NOTE: the path in the Attribute is most likely a path relative to the currents file directory we're looking at
        var importedFilePath = _fileSystem.Path.Combine(propsFile.Directory.FullName, importProject.Value);
        var importedFile = _fileSystem.FileInfo.FromFileName(importedFilePath);
        
        // ReSharper disable once TailRecursiveCall
        return IsCpmEnabledThroughPropsFile(importedFile);
    }
}
