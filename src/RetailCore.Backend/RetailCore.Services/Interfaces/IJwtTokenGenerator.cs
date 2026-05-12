using RetailCore.Repositories.Entities;

namespace RetailCore.Services.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(AppUser user, IList<string> roles);
    RefreshToken GenerateRefreshToken();
}
