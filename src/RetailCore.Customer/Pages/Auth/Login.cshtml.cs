using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetailCore.Customer.Services;
using RetailCore.Customer.Services.Api;
using RetailCore.Shared.Requests.Auth;

namespace RetailCore.Customer.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly IAuthApi _authApi;

    public LoginModel(IAuthApi authApi)
    {
        _authApi = authApi;
    }

    [BindProperty]
    public LoginRequest Input { get; set; } = new();

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
            var auth = await _authApi.LoginAsync(Input);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, auth.User.Id.ToString()),
                new(ClaimTypes.Name, auth.User.FullName),
                new(ClaimTypes.Email, auth.User.Email),

                new("avatar_url", auth.User.AvatarUrl ?? ""),
                new("access_token", auth.AccessToken),
                new("refresh_token", auth.RefreshToken)
            };

            claims.AddRange(auth.User.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

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
