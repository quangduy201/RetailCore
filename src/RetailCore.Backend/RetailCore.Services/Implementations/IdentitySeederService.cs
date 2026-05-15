using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RetailCore.Repositories.Entities;
using RetailCore.Services.Interfaces;
using RetailCore.Services.Options;
using RetailCore.Shared.Constants;

namespace RetailCore.Services.Implementations;

public class IdentitySeederService : IIdentitySeederService
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IOptions<AdminOptions> _adminOptions;

    public IdentitySeederService(
        RoleManager<AppRole> roleManager,
        UserManager<AppUser> userManager,
        IOptions<AdminOptions> adminOptions)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _adminOptions = adminOptions;
    }

    public async Task SeedAsync()
    {
        if (!await _roleManager.RoleExistsAsync(RoleConstants.Admin))
        {
            await _roleManager.CreateAsync(new AppRole
            {
                Name = RoleConstants.Admin,
                Description = "System administrator role"
            });
        }

        if (!await _roleManager.RoleExistsAsync(RoleConstants.Customer))
        {
            await _roleManager.CreateAsync(new AppRole
            {
                Name = RoleConstants.Customer,
                Description = "Customer role"
            });
        }

        var adminEmail = _adminOptions.Value.Email;

        var adminUser = await _userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                Email = adminEmail,
                UserName = adminEmail,
                FullName = _adminOptions.Value.FullName,
                AvatarUrl = _adminOptions.Value.AvatarUrl,
            };

            var result = await _userManager.CreateAsync(adminUser, _adminOptions.Value.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, RoleConstants.Admin);
            }
        }
    }
}
