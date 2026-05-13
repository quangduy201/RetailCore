using RetailCore.Shared.DTOs.Auth;
using RetailCore.Shared.DTOs.User;
using RetailCore.Shared.Requests.Auth;

namespace RetailCore.Services.Interfaces;

public interface IAuthService
{
    Task<AuthDto> RegisterAsync(RegisterRequest request);
    Task<AuthDto> LoginAsync(LoginRequest request);
    Task<AuthDto> RefreshTokenAsync(RefreshTokenRequest request);
    Task LogoutAsync(LogoutRequest request);
    Task LogoutAllAsync(Guid userId);
    Task<UserDto> GetCurrentUserAsync(Guid userId);
    Task<UserDto> UpdateCurrentUserAsync(Guid userId, UpdateProfileRequest request);
}
