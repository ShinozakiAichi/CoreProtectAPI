namespace CoreProtect.Domain.Entities;

public sealed record UserResolution(int Id, string CurrentUser, string? Uuid);
