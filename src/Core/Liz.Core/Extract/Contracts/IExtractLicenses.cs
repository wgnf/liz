using Liz.Core.License.Contracts.Exceptions;
using Liz.Core.License.Contracts.Models;
using Liz.Core.PackageReferences.Contracts.Exceptions;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Projects.Contracts.Exceptions;
using Liz.Core.Projects.Contracts.Models;

namespace Liz.Core.Extract.Contracts;

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
