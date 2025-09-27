namespace CoreProtect.Infrastructure.Configuration;

public sealed class CoreProtectDatabaseOptions
{
    public const string SectionName = "ConnectionStrings";
    public string CoreProtect { get; init; } = string.Empty;
    public TimeSpan CommandTimeout { get; init; } = TimeSpan.FromSeconds(5);
}
