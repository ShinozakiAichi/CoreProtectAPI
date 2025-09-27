namespace CoreProtect.Application.Common;

public sealed record CommandQueryParameters(LogQueryParameters Base, string? CommandLike);
