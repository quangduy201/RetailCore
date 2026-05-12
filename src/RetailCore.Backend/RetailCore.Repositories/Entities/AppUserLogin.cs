using Microsoft.AspNetCore.Identity;

namespace RetailCore.Repositories.Entities;

public class AppUserLogin : IdentityUserLogin<Guid>
{
    public AppUser User { get; set; } = default!;
}
