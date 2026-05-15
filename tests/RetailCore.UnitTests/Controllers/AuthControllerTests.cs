using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailCore.Shared.DTOs.Auth;
using RetailCore.Shared.DTOs.User;
using RetailCore.Shared.Requests.Auth;

namespace RetailCore.UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Register_ShouldReturnOkWithAuthDto()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "user@example.com",
            Password = "Password@123",
            FullName = "Test User"
        };

        var expectedResult = CreateAuthDto(request.Email, request.FullName);

        _authServiceMock
            .Setup(service => service.RegisterAsync(request))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<AuthDto>(okResult.Value);

        Assert.Equal(expectedResult.AccessToken, value.AccessToken);
        Assert.Equal(expectedResult.RefreshToken, value.RefreshToken);
        Assert.Equal(expectedResult.User.Id, value.User.Id);
        Assert.Equal(expectedResult.User.Email, value.User.Email);
        Assert.Equal(expectedResult.User.FullName, value.User.FullName);

        _authServiceMock.Verify(
            service => service.RegisterAsync(request),
            Times.Once);
    }

    [Fact]
    public async Task Login_ShouldReturnOkWithAuthDto()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "user@example.com",
            Password = "Password@123"
        };

        var expectedResult = CreateAuthDto(request.Email, "Test User");

        _authServiceMock
            .Setup(service => service.LoginAsync(request))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<AuthDto>(okResult.Value);

        Assert.Equal(expectedResult.AccessToken, value.AccessToken);
        Assert.Equal(expectedResult.RefreshToken, value.RefreshToken);
        Assert.Equal(expectedResult.User.Email, value.User.Email);

        _authServiceMock.Verify(
            service => service.LoginAsync(request),
            Times.Once);
    }

    [Fact]
    public async Task Refresh_ShouldReturnOkWithAuthDto()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            RefreshToken = "old-refresh-token"
        };

        var expectedResult = CreateAuthDto("user@example.com", "Test User");

        _authServiceMock
            .Setup(service => service.RefreshTokenAsync(request))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.Refresh(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<AuthDto>(okResult.Value);

        Assert.Equal(expectedResult.AccessToken, value.AccessToken);
        Assert.Equal(expectedResult.RefreshToken, value.RefreshToken);
        Assert.Equal(expectedResult.User.Id, value.User.Id);

        _authServiceMock.Verify(
            service => service.RefreshTokenAsync(request),
            Times.Once);
    }

    [Fact]
    public async Task Logout_ShouldReturnNoContent()
    {
        // Arrange
        var request = new LogoutRequest
        {
            RefreshToken = "refresh-token"
        };

        _authServiceMock
            .Setup(service => service.LogoutAsync(request))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Logout(request);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _authServiceMock.Verify(
            service => service.LogoutAsync(request),
            Times.Once);
    }

    [Fact]
    public async Task LogoutAll_WhenUserIdClaimMissing_ShouldReturnUnauthorized()
    {
        // Arrange
        SetUser([]);

        // Act
        var result = await _controller.LogoutAll();

        // Assert
        Assert.IsType<UnauthorizedResult>(result);

        _authServiceMock.Verify(
            service => service.LogoutAllAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task LogoutAll_WhenUserIdClaimExists_ShouldReturnNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetUserWithUserId(userId);

        _authServiceMock
            .Setup(service => service.LogoutAllAsync(userId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.LogoutAll();

        // Assert
        Assert.IsType<NoContentResult>(result);

        _authServiceMock.Verify(
            service => service.LogoutAllAsync(userId),
            Times.Once);
    }

    [Fact]
    public async Task Me_WhenUserIdClaimMissing_ShouldReturnUnauthorized()
    {
        // Arrange
        SetUser([]);

        // Act
        var result = await _controller.Me();

        // Assert
        Assert.IsType<UnauthorizedResult>(result);

        _authServiceMock.Verify(
            service => service.GetCurrentUserAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task Me_WhenUserIdClaimExists_ShouldReturnOkWithUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetUserWithUserId(userId);

        var expectedUser = CreateUserDto(userId);

        _authServiceMock
            .Setup(service => service.GetCurrentUserAsync(userId))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _controller.Me();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<UserDto>(okResult.Value);

        Assert.Equal(expectedUser.Id, value.Id);
        Assert.Equal(expectedUser.Email, value.Email);
        Assert.Equal(expectedUser.FullName, value.FullName);
        Assert.Equal(expectedUser.AvatarUrl, value.AvatarUrl);
        Assert.Equal(expectedUser.IsActive, value.IsActive);
        Assert.Equal(expectedUser.CreatedAt, value.CreatedAt);

        _authServiceMock.Verify(
            service => service.GetCurrentUserAsync(userId),
            Times.Once);
    }

    [Fact]
    public async Task UpdateMe_WhenUserIdClaimMissing_ShouldReturnUnauthorized()
    {
        // Arrange
        SetUser([]);

        var request = new UpdateProfileRequest
        {
            FullName = "Updated User",
            AvatarUrl = "updated-avatar.png"
        };

        // Act
        var result = await _controller.UpdateMe(request);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);

        _authServiceMock.Verify(
            service => service.UpdateCurrentUserAsync(
                It.IsAny<Guid>(),
                It.IsAny<UpdateProfileRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateMe_WhenUserIdClaimExists_ShouldReturnOkWithUpdatedUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetUserWithUserId(userId);

        var request = new UpdateProfileRequest
        {
            FullName = "Updated User",
            AvatarUrl = "updated-avatar.png"
        };

        var expectedUser = CreateUserDto(
            userId,
            fullName: request.FullName,
            avatarUrl: request.AvatarUrl);

        _authServiceMock
            .Setup(service => service.UpdateCurrentUserAsync(userId, request))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _controller.UpdateMe(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<UserDto>(okResult.Value);

        Assert.Equal(expectedUser.Id, value.Id);
        Assert.Equal(request.FullName, value.FullName);
        Assert.Equal(request.AvatarUrl, value.AvatarUrl);

        _authServiceMock.Verify(
            service => service.UpdateCurrentUserAsync(userId, request),
            Times.Once);
    }

    private void SetUserWithUserId(Guid userId)
    {
        SetUser(
        [
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        ]);
    }

    private void SetUser(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, authenticationType: "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }

    private static AuthDto CreateAuthDto(
        string email = "user@example.com",
        string fullName = "Test User")
    {
        var userId = Guid.NewGuid();

        return new AuthDto
        {
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            User = CreateUserDto(userId, email, fullName)
        };
    }

    private static UserDto CreateUserDto(
        Guid id,
        string email = "user@example.com",
        string fullName = "Test User",
        string avatarUrl = "avatar.png")
    {
        return new UserDto
        {
            Id = id,
            Email = email,
            FullName = fullName,
            AvatarUrl = avatarUrl,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Roles = ["Customer"]
        };
    }
}
