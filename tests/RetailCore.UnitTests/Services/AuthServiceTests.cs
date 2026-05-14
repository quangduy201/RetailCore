using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.Constants;
using RetailCore.Shared.Requests.Auth;

namespace RetailCore.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userManagerMock = MockUserManager();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        _refreshTokenRepoMock = new Mock<IRefreshTokenRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _authService = new AuthService(
            _userManagerMock.Object,
            _jwtTokenGeneratorMock.Object,
            _refreshTokenRepoMock.Object,
            _unitOfWorkMock.Object);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "user@example.com",
            Password = "Password@123",
            FullName = "Existing User"
        };

        _userManagerMock
            .Setup(manager => manager.FindByEmailAsync(request.Email))
            .ReturnsAsync(new AppUser());

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _authService.RegisterAsync(request));

        // Assert
        Assert.Equal("Email already exists.", exception.Message);

        _userManagerMock.Verify(
            manager => manager.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenCreateUserFails_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "user@example.com",
            Password = "weak",
            FullName = "New User"
        };

        var identityErrors = new[]
        {
            new IdentityError { Description = "Password too weak" },
            new IdentityError { Description = "Password requires uppercase" }
        };

        _userManagerMock
            .Setup(manager => manager.FindByEmailAsync(request.Email))
            .ReturnsAsync((AppUser?)null);

        _userManagerMock
            .Setup(manager => manager.CreateAsync(It.IsAny<AppUser>(), request.Password))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _authService.RegisterAsync(request));

        // Assert
        Assert.Equal("Password too weak, Password requires uppercase", exception.Message);

        _userManagerMock.Verify(
            manager => manager.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()),
            Times.Never);

        _refreshTokenRepoMock.Verify(
            repo => repo.CreateAsync(It.IsAny<RefreshToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenRequestIsValid_ShouldCreateUserAddCustomerRoleCreateRefreshTokenAndReturnAuthDto()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "user@example.com",
            Password = "Password@123",
            FullName = "New User"
        };

        var roles = new List<string> { RoleConstants.Customer };

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "refresh-token",
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        AppUser? createdUser = null;

        _userManagerMock
            .Setup(manager => manager.FindByEmailAsync(request.Email))
            .ReturnsAsync((AppUser?)null);

        _userManagerMock
            .Setup(manager => manager.CreateAsync(It.IsAny<AppUser>(), request.Password))
            .Callback<AppUser, string>((user, _) => createdUser = user)
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(manager => manager.AddToRoleAsync(It.IsAny<AppUser>(), RoleConstants.Customer))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(manager => manager.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(roles);

        _jwtTokenGeneratorMock
            .Setup(generator => generator.GenerateAccessToken(It.IsAny<AppUser>(), roles))
            .Returns("access-token");

        _jwtTokenGeneratorMock
            .Setup(generator => generator.GenerateRefreshToken())
            .Returns(refreshToken);

        _refreshTokenRepoMock
            .Setup(repo => repo.CreateAsync(It.IsAny<RefreshToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.NotNull(createdUser);
        Assert.NotEqual(Guid.Empty, createdUser!.Id);
        Assert.Equal(request.Email, createdUser.Email);
        Assert.Equal(request.Email, createdUser.UserName);
        Assert.Equal(request.FullName, createdUser.FullName);
        Assert.Equal("", createdUser.AvatarUrl);
        Assert.True(createdUser.IsActive);
        Assert.NotEqual(default, createdUser.CreatedAt);

        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal("refresh-token", result.RefreshToken);

        Assert.Equal(createdUser.Id, result.User.Id);
        Assert.Equal(request.Email, result.User.Email);
        Assert.Equal(request.FullName, result.User.FullName);
        Assert.Equal("", result.User.AvatarUrl);
        Assert.True(result.User.IsActive);
        Assert.Single(result.User.Roles);
        Assert.Equal(RoleConstants.Customer, result.User.Roles[0]);

        Assert.Equal(createdUser.Id, refreshToken.UserId);

        _userManagerMock.Verify(
            manager => manager.AddToRoleAsync(createdUser, RoleConstants.Customer),
            Times.Once);

        _refreshTokenRepoMock.Verify(
            repo => repo.CreateAsync(refreshToken),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenUserDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "missing@example.com",
            Password = "Password@123"
        };

        _userManagerMock
            .Setup(manager => manager.FindByEmailAsync(request.Email))
            .ReturnsAsync((AppUser?)null);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _authService.LoginAsync(request));

        // Assert
        Assert.Equal("Invalid credentials.", exception.Message);

        _userManagerMock.Verify(
            manager => manager.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenUserIsInactive_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "inactive@example.com",
            Password = "Password@123"
        };

        var user = CreateUser(email: request.Email, isActive: false);

        _userManagerMock
            .Setup(manager => manager.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _authService.LoginAsync(request));

        // Assert
        Assert.Equal("Account is inactive.", exception.Message);

        _userManagerMock.Verify(
            manager => manager.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsInvalid_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "user@example.com",
            Password = "WrongPassword"
        };

        var user = CreateUser(email: request.Email);

        _userManagerMock
            .Setup(manager => manager.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(manager => manager.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(false);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _authService.LoginAsync(request));

        // Assert
        Assert.Equal("Invalid credentials.", exception.Message);

        _refreshTokenRepoMock.Verify(
            repo => repo.CreateAsync(It.IsAny<RefreshToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ShouldCreateRefreshTokenAndReturnAuthDto()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "user@example.com",
            Password = "Password@123"
        };

        var user = CreateUser(email: request.Email);
        var roles = new List<string> { RoleConstants.Customer };

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "login-refresh-token",
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        _userManagerMock
            .Setup(manager => manager.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(manager => manager.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(true);

        _userManagerMock
            .Setup(manager => manager.GetRolesAsync(user))
            .ReturnsAsync(roles);

        _jwtTokenGeneratorMock
            .Setup(generator => generator.GenerateAccessToken(user, roles))
            .Returns("login-access-token");

        _jwtTokenGeneratorMock
            .Setup(generator => generator.GenerateRefreshToken())
            .Returns(refreshToken);

        _refreshTokenRepoMock
            .Setup(repo => repo.CreateAsync(It.IsAny<RefreshToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.Equal("login-access-token", result.AccessToken);
        Assert.Equal("login-refresh-token", result.RefreshToken);

        Assert.Equal(user.Id, result.User.Id);
        Assert.Equal(user.Email, result.User.Email);
        Assert.Equal(user.FullName, result.User.FullName);
        Assert.Equal(user.AvatarUrl, result.User.AvatarUrl);
        Assert.Equal(user.IsActive, result.User.IsActive);
        Assert.Equal(user.CreatedAt, result.User.CreatedAt);
        Assert.Single(result.User.Roles);
        Assert.Equal(RoleConstants.Customer, result.User.Roles[0]);

        Assert.Equal(user.Id, refreshToken.UserId);

        _refreshTokenRepoMock.Verify(
            repo => repo.CreateAsync(refreshToken),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenRefreshTokenDoesNotExist_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            RefreshToken = "missing-token"
        };

        _refreshTokenRepoMock
            .Setup(repo => repo.GetByTokenAsync(request.RefreshToken))
            .ReturnsAsync((RefreshToken?)null);

        // Act
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.RefreshTokenAsync(request));

        // Assert
        Assert.Equal("Invalid refresh token.", exception.Message);

        _refreshTokenRepoMock.Verify(
            repo => repo.Update(It.IsAny<RefreshToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenRefreshTokenIsRevoked_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            RefreshToken = "revoked-token"
        };

        var oldRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = request.RefreshToken,
            User = CreateUser(),
            UserId = Guid.NewGuid(),
            IsRevoked = true,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };

        _refreshTokenRepoMock
            .Setup(repo => repo.GetByTokenAsync(request.RefreshToken))
            .ReturnsAsync(oldRefreshToken);

        // Act
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.RefreshTokenAsync(request));

        // Assert
        Assert.Equal("Invalid refresh token.", exception.Message);

        _refreshTokenRepoMock.Verify(
            repo => repo.Update(It.IsAny<RefreshToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenRefreshTokenIsExpired_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            RefreshToken = "expired-token"
        };

        var oldRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = request.RefreshToken,
            User = CreateUser(),
            UserId = Guid.NewGuid(),
            IsRevoked = false,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-10),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(-1)
        };

        _refreshTokenRepoMock
            .Setup(repo => repo.GetByTokenAsync(request.RefreshToken))
            .ReturnsAsync(oldRefreshToken);

        // Act
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.RefreshTokenAsync(request));

        // Assert
        Assert.Equal("Invalid refresh token.", exception.Message);

        _refreshTokenRepoMock.Verify(
            repo => repo.Update(It.IsAny<RefreshToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenRefreshTokenIsActive_ShouldRotateRefreshTokenAndReturnAuthDto()
    {
        // Arrange
        var user = CreateUser();

        var request = new RefreshTokenRequest
        {
            RefreshToken = "old-refresh-token"
        };

        var oldRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = request.RefreshToken,
            User = user,
            UserId = user.Id,
            IsRevoked = false,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };

        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "new-refresh-token",
            IsRevoked = false,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };

        var roles = new List<string> { RoleConstants.Customer };

        _refreshTokenRepoMock
            .Setup(repo => repo.GetByTokenAsync(request.RefreshToken))
            .ReturnsAsync(oldRefreshToken);

        _userManagerMock
            .Setup(manager => manager.GetRolesAsync(user))
            .ReturnsAsync(roles);

        _jwtTokenGeneratorMock
            .Setup(generator => generator.GenerateAccessToken(user, roles))
            .Returns("new-access-token");

        _jwtTokenGeneratorMock
            .Setup(generator => generator.GenerateRefreshToken())
            .Returns(newRefreshToken);

        _refreshTokenRepoMock
            .Setup(repo => repo.CreateAsync(It.IsAny<RefreshToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RefreshTokenAsync(request);

        // Assert
        Assert.True(oldRefreshToken.IsRevoked);
        Assert.NotNull(oldRefreshToken.RevokedAtUtc);

        Assert.Equal(user.Id, newRefreshToken.UserId);

        Assert.Equal("new-access-token", result.AccessToken);
        Assert.Equal("new-refresh-token", result.RefreshToken);

        Assert.Equal(user.Id, result.User.Id);
        Assert.Equal(user.Email, result.User.Email);
        Assert.Equal(user.FullName, result.User.FullName);
        Assert.Equal(user.AvatarUrl, result.User.AvatarUrl);
        Assert.Equal(user.IsActive, result.User.IsActive);
        Assert.Equal(user.CreatedAt, result.User.CreatedAt);
        Assert.Single(result.User.Roles);
        Assert.Equal(RoleConstants.Customer, result.User.Roles[0]);

        _refreshTokenRepoMock.Verify(
            repo => repo.Update(oldRefreshToken),
            Times.Once);

        _refreshTokenRepoMock.Verify(
            repo => repo.CreateAsync(newRefreshToken),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task LogoutAsync_WhenRefreshTokenDoesNotExist_ShouldReturnWithoutSaving()
    {
        // Arrange
        var request = new LogoutRequest
        {
            RefreshToken = "missing-token"
        };

        _refreshTokenRepoMock
            .Setup(repo => repo.GetByTokenAsync(request.RefreshToken))
            .ReturnsAsync((RefreshToken?)null);

        // Act
        await _authService.LogoutAsync(request);

        // Assert
        _refreshTokenRepoMock.Verify(
            repo => repo.Update(It.IsAny<RefreshToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task LogoutAsync_WhenRefreshTokenExists_ShouldRevokeTokenAndSave()
    {
        // Arrange
        var request = new LogoutRequest
        {
            RefreshToken = "refresh-token"
        };

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = request.RefreshToken,
            IsRevoked = false,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };

        _refreshTokenRepoMock
            .Setup(repo => repo.GetByTokenAsync(request.RefreshToken))
            .ReturnsAsync(refreshToken);

        // Act
        await _authService.LogoutAsync(request);

        // Assert
        Assert.True(refreshToken.IsRevoked);
        Assert.NotNull(refreshToken.RevokedAtUtc);

        _refreshTokenRepoMock.Verify(
            repo => repo.Update(refreshToken),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task LogoutAllAsync_WhenNoActiveRefreshTokens_ShouldReturnWithoutSaving()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _refreshTokenRepoMock
            .Setup(repo => repo.GetActiveByUserIdAsync(userId))
            .ReturnsAsync([]);

        // Act
        await _authService.LogoutAllAsync(userId);

        // Assert
        _refreshTokenRepoMock.Verify(
            repo => repo.Update(It.IsAny<RefreshToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task LogoutAllAsync_WhenActiveRefreshTokensExist_ShouldRevokeAllTokensAndSave()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var tokens = new List<RefreshToken>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = "token-1",
                IsRevoked = false,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = "token-2",
                IsRevoked = false,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
            }
        };

        _refreshTokenRepoMock
            .Setup(repo => repo.GetActiveByUserIdAsync(userId))
            .ReturnsAsync(tokens);

        // Act
        await _authService.LogoutAllAsync(userId);

        // Assert
        Assert.All(tokens, token =>
        {
            Assert.True(token.IsRevoked);
            Assert.NotNull(token.RevokedAtUtc);
        });

        _refreshTokenRepoMock.Verify(
            repo => repo.Update(It.IsAny<RefreshToken>()),
            Times.Exactly(2));

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetCurrentUserAsync_WhenUserDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((AppUser?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _authService.GetCurrentUserAsync(userId));

        // Assert
        Assert.Equal("User not found.", exception.Message);
    }

    [Fact]
    public async Task GetCurrentUserAsync_WhenUserExists_ShouldReturnUserDto()
    {
        // Arrange
        var user = CreateUser();
        var roles = new List<string> { RoleConstants.Customer, RoleConstants.Admin };

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(user.Id.ToString()))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(manager => manager.GetRolesAsync(user))
            .ReturnsAsync(roles);

        // Act
        var result = await _authService.GetCurrentUserAsync(user.Id);

        // Assert
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.FullName, result.FullName);
        Assert.Equal(user.AvatarUrl, result.AvatarUrl);
        Assert.Equal(user.IsActive, result.IsActive);
        Assert.Equal(user.CreatedAt, result.CreatedAt);

        Assert.Equal(2, result.Roles.Count);
        Assert.Equal(RoleConstants.Customer, result.Roles[0]);
        Assert.Equal(RoleConstants.Admin, result.Roles[1]);
    }

    private static AppUser CreateUser(
        string email = "user@example.com",
        string fullName = "Test User",
        bool isActive = true)
    {
        return new AppUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            FullName = fullName,
            AvatarUrl = "avatar.png",
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static Mock<UserManager<AppUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<AppUser>>();

        return new Mock<UserManager<AppUser>>(
            store.Object,
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<AppUser>>(),
            new List<IUserValidator<AppUser>>(),
            new List<IPasswordValidator<AppUser>>(),
            Mock.Of<ILookupNormalizer>(),
            new IdentityErrorDescriber(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<AppUser>>>());
    }
}
