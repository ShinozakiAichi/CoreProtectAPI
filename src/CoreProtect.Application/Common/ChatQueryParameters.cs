namespace CoreProtect.Application.Common;

public sealed record ChatQueryParameters(LogQueryParameters Base, string? MessageLike);
