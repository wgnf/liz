using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.Utils;

internal sealed class PackageReferencePrinter : IPackageReferencePrinter
{
    private readonly ExtractLicensesSettings _settings;

    public PackageReferencePrinter([NotNull] ExtractLicensesSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }
    
    public void PrintPackageReferences([NotNull] IEnumerable<PackageReference> packageReferences)
    {
        ArgumentNullException.ThrowIfNull(packageReferences);
        
        
    }
}
