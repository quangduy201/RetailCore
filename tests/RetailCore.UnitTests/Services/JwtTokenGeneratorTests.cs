using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using RetailCore.Services.Options;

namespace RetailCore.UnitTests.Services;

public class JwtTokenGeneratorTests
{
    [Fact]
    public void GenerateAccessToken_ShouldReturnValidJwtWithExpectedClaims()
    {
        // Arrange
        var options = CreateJwtOptions();
        var generator = new JwtTokenGenerator(options);

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = "admin@example.com",
            FullName = "Admin User"
        };

        var roles = new List<string> { "Admin", "Customer" };

        // Act
        var token = generator.GenerateAccessToken(user, roles);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        Assert.Equal(options.Issuer, jwt.Issuer);
        Assert.Contains(options.Audience, jwt.Audiences);

        Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id.ToString());
        Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Name && c.Value == user.FullName);
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());

        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Customer");

        Assert.True(jwt.ValidTo > DateTime.UtcNow);
    }

    [Fact]
    public void GenerateAccessToken_WhenEmailIsNull_ShouldUseEmptyEmailClaim()
    {
        // Arrange
        var generator = new JwtTokenGenerator(CreateJwtOptions());

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = null,
            FullName = "No Email User"
        };

        // Act
        var token = generator.GenerateAccessToken(user, []);

        // Assert
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == string.Empty);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnActiveRefreshToken()
    {
        // Arrange
        var options = CreateJwtOptions();
        var generator = new JwtTokenGenerator(options);

        // Act
        var refreshToken = generator.GenerateRefreshToken();

        // Assert
        Assert.NotEqual(Guid.Empty, refreshToken.Id);
        Assert.False(string.IsNullOrWhiteSpace(refreshToken.Token));
        Assert.False(refreshToken.IsRevoked);
        Assert.True(refreshToken.CreatedAtUtc <= DateTime.UtcNow);
        Assert.True(refreshToken.ExpiresAtUtc > DateTime.UtcNow);
        Assert.True(refreshToken.IsActive);
        Assert.False(refreshToken.IsExpired);
    }

    private static JwtOptions CreateJwtOptions()
    {
        return new JwtOptions
        {
            SecretKey = "this-is-a-very-long-secret-key-for-unit-tests-only",
            Issuer = "RetailCore.UnitTests",
            Audience = "RetailCore.UnitTests.Client",
            AccessTokenExpirationInSeconds = 3600,
            RefreshTokenExpirationInSeconds = 86400
        };
    }
}
