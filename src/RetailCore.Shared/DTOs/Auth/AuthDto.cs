using RetailCore.Shared.DTOs.User;

namespace RetailCore.Shared.DTOs.Auth;

public class AuthDto
{
    public UserDto User { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime AccessTokenExpiresAtUtc { get; set; }
    public DateTime RefreshTokenExpiresAtUtc { get; set; }
}
