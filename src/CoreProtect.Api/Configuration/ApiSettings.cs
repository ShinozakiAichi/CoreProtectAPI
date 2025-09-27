namespace CoreProtect.Api.Configuration;

public sealed class ApiSettings
{
    public const string SectionName = "API";

    public int DefaultLimit { get; init; } = 10;
    public int MaxLimit { get; init; } = 500;
    public bool EnableApiKey { get; init; }
    public string? ApiKey { get; init; }
    public CorsSettings Cors { get; init; } = new();

    public sealed class CorsSettings
    {
        public string[] Origins { get; init; } = Array.Empty<string>();
    }
}
