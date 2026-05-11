using RetailCore.Shared.DTOs.Auth;
using RetailCore.Shared.Requests.Auth;

namespace RetailCore.Services.Interfaces;

public interface IAuthService
{
    Task<AuthDto> RegisterAsync(RegisterRequest request);
    Task<AuthDto> LoginAsync(LoginRequest request);
    Task<AuthDto> GetCurrentUserAsync(Guid userId);
}
