using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.PackageReferences.Contracts.NuGetCli;
using System.IO.Abstractions;
using System.Xml.Linq;

namespace Liz.Core.PackageReferences.NuGetCli;

internal sealed class ParsePackagesConfigFile : IParsePackagesConfigFile
{
    public IEnumerable<PackageReference> GetPackageReferences(IFileInfo packagesConfigFile)
    {
        if (packagesConfigFile == null) throw new ArgumentNullException(nameof(packagesConfigFile));

        if (!packagesConfigFile.Exists)
            throw new ArgumentException("The provided file does not exist", nameof(packagesConfigFile));

        using var stream = packagesConfigFile.OpenRead();
        var packagesConfigXml = XDocument.Load(stream);

        var packageReferences = GetPackageReferencesFromXml(packagesConfigXml);
        return packageReferences;
    }

    private static IEnumerable<PackageReference> GetPackageReferencesFromXml(XContainer packagesConfigXml)
    {
        var packagesElements = packagesConfigXml.Descendants("package");

        var packageReferences = new List<PackageReference>();

        foreach (var packageElement in packagesElements)
        {
            if (!TryGetPackageReferenceFromPackageElement(packageElement, out var packageReference) || packageReference == null)
                break;
            
            packageReferences.Add(packageReference);
        }

        return packageReferences;
    }

    private static bool TryGetPackageReferenceFromPackageElement(XElement packageElement, out PackageReference? packageReference)
    {
        packageReference = null;
        
        var packageNameAttribute = packageElement.Attribute("id");
        var packageVersionAttribute = packageElement.Attribute("version");
        var packageTargetFrameworkAttribute = packageElement.Attribute("targetFramework");

        if (string.IsNullOrWhiteSpace(packageNameAttribute?.Value)) return false;
        if (string.IsNullOrWhiteSpace(packageVersionAttribute?.Value)) return false;
        if (string.IsNullOrWhiteSpace(packageTargetFrameworkAttribute?.Value)) return false;
        
        packageReference = new PackageReference(
            packageNameAttribute.Value,
            packageTargetFrameworkAttribute.Value,
            packageVersionAttribute.Value);
        return true;
    }
}
