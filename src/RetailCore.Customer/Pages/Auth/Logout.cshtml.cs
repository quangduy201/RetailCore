using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetailCore.Customer.Services.Api;
using RetailCore.Shared.Requests.Auth;

namespace RetailCore.Customer.Pages.Auth;

public class LogoutModel : PageModel
{
    private readonly IAuthApi _authApi;

    public LogoutModel(IAuthApi authApi)
    {
        _authApi = authApi;
    }

    public IActionResult OnGet()
    {
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var refreshToken = User.FindFirstValue("refresh_token");

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await _authApi.LogoutAsync(new LogoutRequest { RefreshToken = refreshToken });
            }
        }
        catch
        {
            // ignore logout api failures
            // because user should still be logged out locally
        }

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToPage("/Index");
    }
}
