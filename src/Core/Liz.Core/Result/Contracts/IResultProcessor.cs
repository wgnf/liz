using Liz.Core.PackageReferences.Contracts.Models;

namespace Liz.Core.Result.Contracts;

internal interface IResultProcessor
{
    Task ProcessResultsAsync(IEnumerable<PackageReference> packageReferences);
}
