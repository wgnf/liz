using Liz.Core.License;
using Liz.Core.PackageReferences;
using Liz.Core.Projects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Liz.Core.Extract;

/// <summary>
///     Component to extract licenses
/// </summary>
public interface IExtractLicenses
{
    /// <summary>
    ///     Extract licenses from a given target, provided to the <see cref="ExtractLicensesFactory" />
    /// </summary>
    /// 
    /// <returns>A list of <see cref="PackageReference" />s which contain <see cref="LicenseInformation" /></returns>
    /// 
    /// <exception cref="GetProjectsFailedException">
    ///     Unable to determine the <see cref="Project" />s contained in the target
    /// </exception>
    ///
    /// <exception cref="GetPackageReferencesFailedException">
    ///     Unable to determine the <see cref="PackageReference" />s of the
    ///     determined <see cref="Project" />s
    /// </exception>
    /// 
    /// <exception cref="GetLicenseInformationFailedException">
    ///     Unable to determine the <see cref="LicenseInformation" /> of the
    ///     determined <see cref="PackageReference" />s
    /// </exception>
    Task<IEnumerable<PackageReference>> ExtractAsync();
}
