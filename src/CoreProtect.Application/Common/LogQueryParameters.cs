namespace CoreProtect.Application.Common;

public sealed record LogQueryParameters(
    string? UserExact,
    string? UserLike,
    string? World,
    long? From,
    long? To,
    CoordinateFilter Coordinates,
    SortDirection SortDirection,
    Pagination Pagination);
