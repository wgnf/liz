﻿using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Liz.Core.License;

internal interface IGetLicenseInformationFromArtifact
{
    Task<LicenseInformation> GetFromDownloadedPackageReferenceAsync(IDirectoryInfo downloadDirectory);
}