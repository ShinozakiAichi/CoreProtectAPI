using CoreProtect.Api.Configuration;
using CoreProtect.Api.Exceptions;
using CoreProtect.Application.Common;

namespace CoreProtect.Api.Models;

public class SimplePaginationRequest
{
    public int? Limit { get; init; }
    public int? Offset { get; init; }

    protected virtual void ValidateSpecific()
    {
    }

    protected Pagination BuildPagination(ApiSettings settings)
    {
        ValidateSpecific();
        var limit = Limit ?? settings.DefaultLimit;
        if (limit < 1 || limit > settings.MaxLimit)
        {
            throw new ValidationException($"Limit must be between 1 and {settings.MaxLimit}.");
        }

        var offset = Offset ?? 0;
        if (offset < 0)
        {
            throw new ValidationException("Offset must be non-negative.");
        }

        return new Pagination(limit, offset);
    }

    public Pagination Build(ApiSettings settings) => BuildPagination(settings);
}
