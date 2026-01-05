namespace InsightMed.Infrastructure.Options;

public sealed class JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public ushort ExpiresInDays { get; set; }
}