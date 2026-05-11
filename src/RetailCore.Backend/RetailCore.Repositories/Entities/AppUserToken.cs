using Microsoft.AspNetCore.Identity;

namespace RetailCore.Repositories.Entities;

public class AppUserToken : IdentityUserToken<Guid>
{
    public AppUser User { get; set; } = default!;
}
