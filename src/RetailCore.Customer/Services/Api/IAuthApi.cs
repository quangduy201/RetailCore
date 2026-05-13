using Refit;
using RetailCore.Shared.DTOs.Auth;
using RetailCore.Shared.DTOs.User;
using RetailCore.Shared.Requests.Auth;

namespace RetailCore.Customer.Services.Api;

public interface IAuthApi
{
    [Post("/auth/login")]
    Task<AuthDto> LoginAsync(LoginRequest request);

    [Post("/auth/register")]
    Task<AuthDto> RegisterAsync(RegisterRequest request);

    [Post("/auth/refresh")]
    Task<AuthDto> RefreshAsync([Body] RefreshTokenRequest request);

    [Post("/auth/logout")]
    Task LogoutAsync([Body] LogoutRequest request);

    [Get("/auth/me")]
    Task<UserDto> MeAsync();

    [Put("/auth/me")]
    Task<UserDto> UpdateProfileAsync([Body] UpdateProfileRequest request);
}
