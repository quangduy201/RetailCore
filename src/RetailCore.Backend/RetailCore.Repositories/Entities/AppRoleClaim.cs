using Microsoft.AspNetCore.Identity;

namespace RetailCore.Repositories.Entities;

public class AppRoleClaim : IdentityRoleClaim<Guid>
{
    public AppRole Role { get; set; } = default!;
}
