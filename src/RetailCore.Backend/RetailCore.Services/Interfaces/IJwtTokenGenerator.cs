using RetailCore.Repositories.Entities;

namespace RetailCore.Services.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(AppUser user, IList<string> roles);
}
