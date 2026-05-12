using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RetailCore.Customer.Services.Api;
using RetailCore.Shared.DTOs.Auth;
using RetailCore.Shared.Requests.Auth;

namespace RetailCore.Customer.Services;

public class AuthTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceProvider _serviceProvider;

    public AuthTokenHandler(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.AbsolutePath.ToLower();

        // Skip auth endpoints
        if (path != null && (path.Contains("/auth/login") || path.Contains("/auth/register")))
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var accessToken = httpContext.User.FindFirstValue("access_token");

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // Access token expired
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var refreshed = await TryRefreshTokenAsync(httpContext);

            if (!refreshed)
            {
                return response;
            }

            // retry request with new token
            var newAccessToken = httpContext.User.FindFirstValue("access_token");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);

            response.Dispose();

            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }

    private async Task<bool> TryRefreshTokenAsync(HttpContext httpContext)
    {
        var refreshToken = httpContext.User.FindFirstValue("refresh_token");

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return false;
        }

        using var scope = _serviceProvider.CreateScope();
        var authApi = scope.ServiceProvider.GetRequiredService<IAuthApi>();

        try
        {
            var authDto = await authApi.RefreshAsync(new RefreshTokenRequest { RefreshToken = refreshToken });
            await SignInWithAuthDtoAsync(httpContext, authDto);
            return true;
        }
        catch
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return false;
        }
    }

    private static async Task SignInWithAuthDtoAsync(HttpContext httpContext, AuthDto authDto)
    {
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

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}
