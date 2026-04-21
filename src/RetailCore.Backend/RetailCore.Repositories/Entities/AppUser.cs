using RetailCore.Repositories.Common;

namespace RetailCore.Repositories.Entities;

public class AppUser : Entity
{
    public string Email { get; set; } = default!;
    public string? FullName { get; set; }
}