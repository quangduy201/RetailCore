using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetailCore.Customer.Services;
using RetailCore.Customer.Services.Api;
using RetailCore.Shared.Requests.Auth;

namespace RetailCore.Customer.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly IAuthApi _authApi;

    public RegisterModel(IAuthApi authApi)
    {
        _authApi = authApi;
    }

    [BindProperty]
    public RegisterRequest Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var authDto = await _authApi.RegisterAsync(Input);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, authDto.User.Id.ToString()),
                new(ClaimTypes.Name, authDto.User.FullName),
                new(ClaimTypes.Email, authDto.User.Email),

                new("access_token", authDto.AccessToken),
                new("refresh_token", authDto.RefreshToken),

                new("avatar_url", authDto.User.AvatarUrl ?? "")
            };

            claims.AddRange(authDto.User.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = ApiErrorHelper.ExtractMessage(ex);
            return Page();
        }
    }
}
