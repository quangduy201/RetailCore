namespace RetailCore.Services.Options;

public class JwtOptions
{
    public string SecretKey { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public int ExpirationInSeconds { get; set; }
}
