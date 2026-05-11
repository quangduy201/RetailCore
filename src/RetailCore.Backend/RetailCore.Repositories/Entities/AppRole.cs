using Microsoft.AspNetCore.Identity;

namespace RetailCore.Repositories.Entities;

public class AppRole : IdentityRole<Guid>
{
    public string? Description { get; set; }

    public ICollection<AppUserRole> UserRoles { get; set; } = [];
    public ICollection<AppRoleClaim> Claims { get; set; } = [];
}
