using Microsoft.AspNetCore.Identity;

namespace RetailCore.Repositories.Entities;

public class AppUserClaim : IdentityUserClaim<Guid>
{
    public AppUser User { get; set; } = default!;
}
