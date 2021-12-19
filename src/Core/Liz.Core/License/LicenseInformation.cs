using Liz.Core.PackageReferences;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.License;

[ExcludeFromCodeCoverage]
internal sealed record LicenseInformation(string LicenseType, string RawLicenseText, PackageReference Package);
