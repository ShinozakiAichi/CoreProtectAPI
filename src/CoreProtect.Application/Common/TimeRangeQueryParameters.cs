namespace CoreProtect.Application.Common;

public sealed record TimeRangeQueryParameters(long? From, long? To, Pagination Pagination);
