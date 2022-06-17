using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.Utils.Models;

[ExcludeFromCodeCoverage] // simple dto
internal sealed class Optional<TEntity>
{
    private Optional(bool hasResult, TEntity? result)
    {
        HasResult = hasResult;
        Result = result;
    }
    
    public bool HasResult { get; }
    
    public TEntity? Result { get; }

    public static Optional<TEntity> Success(TEntity result)
    {
        return new Optional<TEntity>(true, result);
    }

    public static Optional<TEntity> Failure()
    {
        return new Optional<TEntity>(false, default);
    }
}
