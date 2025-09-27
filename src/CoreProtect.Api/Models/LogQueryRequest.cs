using System;
using CoreProtect.Api.Configuration;
using CoreProtect.Api.Exceptions;
using CoreProtect.Application.Common;
using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public class LogQueryRequest
{
    public string? User { get; init; }
    public string? UserLike { get; init; }
    public string? World { get; init; }
    public long? From { get; init; }
    public long? To { get; init; }
    public int? XMin { get; init; }
    public int? XMax { get; init; }
    public int? YMin { get; init; }
    public int? YMax { get; init; }
    public int? ZMin { get; init; }
    public int? ZMax { get; init; }
    public string? Sort { get; init; }
    public int? Limit { get; init; }
    public int? Offset { get; init; }

    protected virtual void ValidateSpecific()
    {
    }

    public virtual LogQueryParameters Build(ApiSettings settings)
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

        var sort = string.Equals(Sort, "asc", StringComparison.OrdinalIgnoreCase) ? SortDirection.Ascending : SortDirection.Descending;

        var coordinateFilter = new CoordinateFilter(XMin, XMax, YMin, YMax, ZMin, ZMax);
        return new LogQueryParameters(
            NormalizeUser(User),
            NormalizeUserLike(UserLike),
            NormalizeWorld(World),
            From,
            To,
            coordinateFilter,
            sort,
            new Pagination(limit, offset));
    }

    private static string? NormalizeUser(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? NormalizeUserLike(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return $"%{value.Trim()}%";
    }

    private static string? NormalizeWorld(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
