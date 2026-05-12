using Microsoft.AspNetCore.Identity;
using RetailCore.Repositories.Entities;
using RetailCore.Services.Interfaces;
using RetailCore.Services.Options;
using RetailCore.Shared.Constants;

namespace RetailCore.Services.Implementations;

public class IdentitySeederService : IIdentitySeederService
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly AdminOptions _adminOptions;

    public IdentitySeederService(
        RoleManager<AppRole> roleManager,
        UserManager<AppUser> userManager,
        AdminOptions adminOptions)
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

        var adminEmail = _adminOptions.Email;

        var adminUser = await _userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                Email = adminEmail,
                UserName = adminEmail,
                FullName = _adminOptions.FullName,
                AvatarUrl = _adminOptions.AvatarUrl,
            };

            var result = await _userManager.CreateAsync(adminUser, _adminOptions.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, RoleConstants.Admin);
            }
        }
    }
}
