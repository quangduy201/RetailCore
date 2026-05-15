using System.Net;
using System.Net.Http.Json;
using RetailCore.IntegrationTests.Fixtures;
using RetailCore.Shared.DTOs.Auth;
using RetailCore.Shared.DTOs.User;
using RetailCore.Shared.Requests.Auth;

namespace RetailCore.IntegrationTests.Api.Auth;

public class AuthEndpointsTests : IClassFixture<RetailCoreApiFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(RetailCoreApiFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WhenRequestIsValid_ShouldReturnAuthDto()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "customer@example.com",
            Password = "Customer123!",
            FullName = "Customer User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<AuthDto>();

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));

        Assert.NotNull(result.User);
        Assert.Equal(request.Email, result.User.Email);
        Assert.Equal(request.FullName, result.User.FullName);
        Assert.True(result.User.IsActive);
        Assert.Contains("Customer", result.User.Roles);
    }

    [Fact]
    public async Task Register_WhenEmailAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "duplicate@example.com",
            Password = "Customer123!",
            FullName = "Duplicate User"
        };

        await _client.PostAsJsonAsync("/api/auth/register", request);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WhenCredentialsAreValid_ShouldReturnAuthDto()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Email = "login-user@example.com",
            Password = "Customer123!",
            FullName = "Login User"
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<AuthDto>();

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        Assert.Equal(registerRequest.Email, result.User.Email);
    }

    [Fact]
    public async Task Login_WhenPasswordIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Email = "wrong-password@example.com",
            Password = "Customer123!",
            FullName = "Wrong Password User"
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest
        {
            Email = registerRequest.Email,
            Password = "WrongPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_WhenRefreshTokenIsValid_ShouldReturnNewAuthDto()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Email = "refresh-user@example.com",
            Password = "Customer123!",
            FullName = "Refresh User"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthDto>();

        var refreshRequest = new RefreshTokenRequest
        {
            RefreshToken = auth!.RefreshToken
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<AuthDto>();

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        Assert.NotEqual(auth.RefreshToken, result.RefreshToken);
        Assert.Equal(registerRequest.Email, result.User.Email);
    }

    [Fact]
    public async Task Refresh_WhenRefreshTokenIsInvalid_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            RefreshToken = "invalid-refresh-token"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/refresh", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_WhenNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_WhenAuthenticated_ShouldReturnCurrentUser()
    {
        // Arrange
        var auth = await RegisterAndLoginAsync(
            email: "me-user@example.com",
            password: "Customer123!",
            fullName: "Me User");

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Act
        var response = await _client.GetAsync("/api/auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<UserDto>();

        Assert.NotNull(result);
        Assert.Equal("me-user@example.com", result!.Email);
        Assert.Equal("Me User", result.FullName);
        Assert.Contains("Customer", result.Roles);
    }

    [Fact]
    public async Task UpdateMe_WhenAuthenticated_ShouldUpdateCurrentUser()
    {
        // Arrange
        var auth = await RegisterAndLoginAsync(
            email: "update-me@example.com",
            password: "Customer123!",
            fullName: "Old Name");

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new UpdateProfileRequest
        {
            FullName = "Updated Name",
            AvatarUrl = "updated-avatar.png"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/auth/me", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<UserDto>();

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result!.FullName);
        Assert.Equal("updated-avatar.png", result.AvatarUrl);
    }

    [Fact]
    public async Task Logout_WhenAuthenticated_ShouldReturnNoContent()
    {
        // Arrange
        var auth = await RegisterAndLoginAsync(
            email: "logout-user@example.com",
            password: "Customer123!",
            fullName: "Logout User");

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new LogoutRequest
        {
            RefreshToken = auth.RefreshToken
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/logout", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Logout_WhenNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LogoutRequest
        {
            RefreshToken = "refresh-token"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/logout", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task LogoutAll_WhenAuthenticated_ShouldReturnNoContent()
    {
        // Arrange
        var auth = await RegisterAndLoginAsync(
            email: "logout-all-user@example.com",
            password: "Customer123!",
            fullName: "Logout All User");

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Act
        var response = await _client.PostAsync("/api/auth/logout-all", content: null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task LogoutAll_WhenNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.PostAsync("/api/auth/logout-all", content: null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<AuthDto> RegisterAndLoginAsync(
        string email,
        string password,
        string fullName)
    {
        var registerRequest = new RegisterRequest
        {
            Email = email,
            Password = password,
            FullName = fullName
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        loginResponse.EnsureSuccessStatusCode();

        var auth = await loginResponse.Content.ReadFromJsonAsync<AuthDto>();

        Assert.NotNull(auth);

        return auth!;
    }
}
