namespace RetailCore.Services.Options;

public class JwtOptions
{
    public string SecretKey { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public int AccessTokenExpirationInSeconds { get; set; }
    public int RefreshTokenExpirationInSeconds { get; set; }
}
