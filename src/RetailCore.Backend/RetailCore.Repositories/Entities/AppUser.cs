using Microsoft.AspNetCore.Identity;
using RetailCore.Repositories.Common;

namespace RetailCore.Repositories.Entities;

public class AppUser : IdentityUser<Guid>, IAuditable
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<AppUserRole> UserRoles { get; set; } = [];
    public ICollection<AppUserClaim> Claims { get; set; } = [];
    public ICollection<AppUserLogin> Logins { get; set; } = [];
    public ICollection<AppUserToken> Tokens { get; set; } = [];
}
