using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetailCore.Customer.Services;
using RetailCore.Customer.Services.Api;
using RetailCore.Shared.DTOs.User;
using RetailCore.Shared.Requests.Auth;

namespace RetailCore.Customer.Pages.Account;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly IAuthApi _authApi;

    public ProfileModel(IAuthApi authApi)
    {
        _authApi = authApi;
    }

    [BindProperty]
    public UpdateProfileRequest Input { get; set; } = new();

    public UserDto? Profile { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Profile = await _authApi.MeAsync();
        Input = new UpdateProfileRequest
        {
            FullName = Profile.FullName,
            AvatarUrl = Profile.AvatarUrl
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Profile = await _authApi.MeAsync();
            return Page();
        }

        try
        {
            Profile = await _authApi.UpdateProfileAsync(Input);
            Input = new UpdateProfileRequest
            {
                FullName = Profile.FullName,
                AvatarUrl = Profile.AvatarUrl
            };

            await RefreshProfileClaimsAsync(Profile);
            SuccessMessage = "Profile updated successfully.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ApiErrorHelper.ExtractMessage(ex);
            Profile = await _authApi.MeAsync();
        }

        return Page();
    }

    private async Task RefreshProfileClaimsAsync(UserDto profile)
    {
        var claims = User.Claims
            .Where(claim =>
                claim.Type != ClaimTypes.Name &&
                claim.Type != ClaimTypes.Email &&
                claim.Type != "avatar_url")
            .ToList();

        claims.Add(new Claim(ClaimTypes.Name, profile.FullName));
        claims.Add(new Claim(ClaimTypes.Email, profile.Email));
        claims.Add(new Claim("avatar_url", profile.AvatarUrl ?? string.Empty));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}
