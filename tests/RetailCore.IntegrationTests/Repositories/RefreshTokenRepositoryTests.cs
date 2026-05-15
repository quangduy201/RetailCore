using Microsoft.EntityFrameworkCore;
using RetailCore.IntegrationTests.Fixtures;
using RetailCore.Repositories.Data;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;

namespace RetailCore.IntegrationTests.Repositories;

public class RefreshTokenRepositoryTests : IDisposable
{
    private readonly SqliteTestDatabase _database;

    public RefreshTokenRepositoryTests()
    {
        _database = new SqliteTestDatabase();
    }

    [Fact]
    public async Task CreateAsync_ShouldAddRefreshTokenWithoutSavingUntilSaveChanges()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var user = await SeedUserAsync(context);

        var repository = new RefreshTokenRepository(context);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "refresh-token",
            UserId = user.Id,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        // Act
        await repository.CreateAsync(refreshToken);

        var beforeSave = await context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Id == refreshToken.Id);

        await context.SaveChangesAsync();

        var afterSave = await context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Id == refreshToken.Id);

        // Assert
        Assert.Null(beforeSave);
        Assert.NotNull(afterSave);
        Assert.Equal("refresh-token", afterSave!.Token);
        Assert.Equal(user.Id, afterSave.UserId);
    }

    [Fact]
    public async Task GetByTokenAsync_WhenTokenExists_ShouldReturnTokenWithUser()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var user = await SeedUserAsync(context);

        var refreshToken = await SeedRefreshTokenAsync(
            context,
            user,
            token: "existing-token");

        var repository = new RefreshTokenRepository(context);

        // Act
        var result = await repository.GetByTokenAsync("existing-token");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(refreshToken.Id, result!.Id);
        Assert.Equal("existing-token", result.Token);
        Assert.Equal(user.Id, result.UserId);

        Assert.NotNull(result.User);
        Assert.Equal(user.Id, result.User.Id);
        Assert.Equal(user.Email, result.User.Email);
    }

    [Fact]
    public async Task GetByTokenAsync_WhenTokenDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new RefreshTokenRepository(context);

        // Act
        var result = await repository.GetByTokenAsync("missing-token");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetActiveByUserIdAsync_ShouldReturnOnlyActiveNonExpiredTokensForUser()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var user = await SeedUserAsync(context, email: "user@example.com");
        var otherUser = await SeedUserAsync(context, email: "other@example.com");

        var activeToken = await SeedRefreshTokenAsync(
            context,
            user,
            token: "active-token",
            isRevoked: false,
            expiresAtUtc: DateTime.UtcNow.AddDays(7));

        await SeedRefreshTokenAsync(
            context,
            user,
            token: "revoked-token",
            isRevoked: true,
            expiresAtUtc: DateTime.UtcNow.AddDays(7));

        await SeedRefreshTokenAsync(
            context,
            user,
            token: "expired-token",
            isRevoked: false,
            expiresAtUtc: DateTime.UtcNow.AddDays(-1));

        await SeedRefreshTokenAsync(
            context,
            otherUser,
            token: "other-user-token",
            isRevoked: false,
            expiresAtUtc: DateTime.UtcNow.AddDays(7));

        var repository = new RefreshTokenRepository(context);

        // Act
        var result = await repository.GetActiveByUserIdAsync(user.Id);

        // Assert
        var token = Assert.Single(result);

        Assert.Equal(activeToken.Id, token.Id);
        Assert.Equal("active-token", token.Token);
        Assert.False(token.IsRevoked);
        Assert.True(token.ExpiresAtUtc > DateTime.UtcNow);
    }

    [Fact]
    public async Task GetActiveByUserIdAsync_WhenUserHasNoActiveTokens_ShouldReturnEmptyList()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var user = await SeedUserAsync(context);

        await SeedRefreshTokenAsync(
            context,
            user,
            token: "revoked-token",
            isRevoked: true,
            expiresAtUtc: DateTime.UtcNow.AddDays(7));

        await SeedRefreshTokenAsync(
            context,
            user,
            token: "expired-token",
            isRevoked: false,
            expiresAtUtc: DateTime.UtcNow.AddDays(-1));

        var repository = new RefreshTokenRepository(context);

        // Act
        var result = await repository.GetActiveByUserIdAsync(user.Id);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetActiveByUserIdAsync_WhenUserDoesNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new RefreshTokenRepository(context);

        // Act
        var result = await repository.GetActiveByUserIdAsync(Guid.NewGuid());

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Update_ShouldUpdateRefreshTokenAfterSaveChanges()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var user = await SeedUserAsync(context);

        var refreshToken = await SeedRefreshTokenAsync(
            context,
            user,
            token: "token-to-update");

        var repository = new RefreshTokenRepository(context);

        refreshToken.IsRevoked = true;
        refreshToken.RevokedAtUtc = DateTime.UtcNow;

        // Act
        repository.Update(refreshToken);
        await context.SaveChangesAsync();

        // Assert
        var updatedToken = await context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Id == refreshToken.Id);

        Assert.NotNull(updatedToken);
        Assert.True(updatedToken!.IsRevoked);
        Assert.NotNull(updatedToken.RevokedAtUtc);
    }

    public void Dispose()
    {
        _database.Dispose();
    }

    private static async Task<AppUser> SeedUserAsync(
        AppDbContext context,
        string email = "user@example.com")
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            NormalizedEmail = email.ToUpperInvariant(),
            NormalizedUserName = email.ToUpperInvariant(),
            FullName = "Test User",
            AvatarUrl = "avatar.png",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        return user;
    }

    private static async Task<RefreshToken> SeedRefreshTokenAsync(
        AppDbContext context,
        AppUser user,
        string token = "refresh-token",
        bool isRevoked = false,
        DateTime? expiresAtUtc = null)
    {
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            UserId = user.Id,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = expiresAtUtc ?? DateTime.UtcNow.AddDays(7),
            IsRevoked = isRevoked,
            RevokedAtUtc = isRevoked ? DateTime.UtcNow : null
        };

        context.RefreshTokens.Add(refreshToken);

        await context.SaveChangesAsync();

        return refreshToken;
    }
}
