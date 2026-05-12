namespace RetailCore.Shared.Requests.Auth;

public class LogoutRequest
{
    public string RefreshToken { get; set; } = default!;
}
