using RetailCore.IntegrationTests.Fixtures;
using RetailCore.Repositories.Data;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories;
using RetailCore.Shared.Constants;

namespace RetailCore.IntegrationTests.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly SqliteTestDatabase _database;

    public UserRepositoryTests()
    {
        _database = new SqliteTestDatabase();
    }

    [Fact]
    public async Task GetCustomersAsync_ShouldReturnOnlyUsersWithCustomerRole()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var customerRole = await SeedRoleAsync(context, RoleConstants.Customer);
        var adminRole = await SeedRoleAsync(context, RoleConstants.Admin);

        var customerUser = await SeedUserAsync(
            context,
            email: "customer@example.com",
            fullName: "Customer User",
            createdAt: DateTime.UtcNow.AddDays(-1));

        var adminUser = await SeedUserAsync(
            context,
            email: "admin@example.com",
            fullName: "Admin User",
            createdAt: DateTime.UtcNow);

        await SeedUserRoleAsync(context, customerUser, customerRole);
        await SeedUserRoleAsync(context, adminUser, adminRole);

        var repository = new UserRepository(context);

        // Act
        var result = await repository.GetCustomersAsync();

        // Assert
        var user = Assert.Single(result);

        Assert.Equal(customerUser.Id, user.Id);
        Assert.Equal("customer@example.com", user.Email);
        Assert.Equal("Customer User", user.FullName);

        var userRole = Assert.Single(user.UserRoles);
        Assert.Equal(RoleConstants.Customer, userRole.Role.Name);
    }

    [Fact]
    public async Task GetCustomersAsync_ShouldIncludeUserWithCustomerRoleEvenIfUserAlsoHasOtherRoles()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var customerRole = await SeedRoleAsync(context, RoleConstants.Customer);
        var adminRole = await SeedRoleAsync(context, RoleConstants.Admin);

        var user = await SeedUserAsync(
            context,
            email: "multi-role@example.com",
            fullName: "Multi Role User");

        await SeedUserRoleAsync(context, user, customerRole);
        await SeedUserRoleAsync(context, user, adminRole);

        var repository = new UserRepository(context);

        // Act
        var result = await repository.GetCustomersAsync();

        // Assert
        var customer = Assert.Single(result);

        Assert.Equal(user.Id, customer.Id);
        Assert.Equal("multi-role@example.com", customer.Email);
        Assert.Equal(2, customer.UserRoles.Count);

        Assert.Contains(customer.UserRoles, ur => ur.Role.Name == RoleConstants.Customer);
        Assert.Contains(customer.UserRoles, ur => ur.Role.Name == RoleConstants.Admin);
    }

    [Fact]
    public async Task GetCustomersAsync_ShouldOrderCustomersByCreatedAtDescending()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var customerRole = await SeedRoleAsync(context, RoleConstants.Customer);

        var olderCustomer = await SeedUserAsync(
            context,
            email: "older@example.com",
            fullName: "Older Customer",
            createdAt: DateTime.UtcNow.AddDays(-10));

        var newerCustomer = await SeedUserAsync(
            context,
            email: "newer@example.com",
            fullName: "Newer Customer",
            createdAt: DateTime.UtcNow);

        await SeedUserRoleAsync(context, olderCustomer, customerRole);
        await SeedUserRoleAsync(context, newerCustomer, customerRole);

        var repository = new UserRepository(context);

        // Act
        var result = await repository.GetCustomersAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(newerCustomer.Id, result[0].Id);
        Assert.Equal(olderCustomer.Id, result[1].Id);
    }

    [Fact]
    public async Task GetCustomersAsync_WhenNoCustomersExist_ShouldReturnEmptyList()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var adminRole = await SeedRoleAsync(context, RoleConstants.Admin);

        var adminUser = await SeedUserAsync(
            context,
            email: "admin@example.com",
            fullName: "Admin User");

        await SeedUserRoleAsync(context, adminUser, adminRole);

        var repository = new UserRepository(context);

        // Act
        var result = await repository.GetCustomersAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCustomersAsync_ShouldReturnUsersAsNoTracking()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var customerRole = await SeedRoleAsync(context, RoleConstants.Customer);

        var user = await SeedUserAsync(
            context,
            email: "customer@example.com",
            fullName: "Customer User");

        await SeedUserRoleAsync(context, user, customerRole);

        context.ChangeTracker.Clear();

        var repository = new UserRepository(context);

        // Act
        var result = await repository.GetCustomersAsync();

        // Assert
        Assert.Single(result);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ShouldReturnUserWithRoles()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var customerRole = await SeedRoleAsync(context, RoleConstants.Customer);

        var user = await SeedUserAsync(
            context,
            email: "customer@example.com",
            fullName: "Customer User");

        await SeedUserRoleAsync(context, user, customerRole);

        var repository = new UserRepository(context);

        // Act
        var result = await repository.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Id);
        Assert.Equal("customer@example.com", result.Email);
        Assert.Equal("Customer User", result.FullName);

        var userRole = Assert.Single(result.UserRoles);
        Assert.Equal(RoleConstants.Customer, userRole.Role.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new UserRepository(context);

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTrackedUser()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var customerRole = await SeedRoleAsync(context, RoleConstants.Customer);

        var user = await SeedUserAsync(
            context,
            email: "customer@example.com",
            fullName: "Customer User");

        await SeedUserRoleAsync(context, user, customerRole);

        context.ChangeTracker.Clear();

        var repository = new UserRepository(context);

        // Act
        var result = await repository.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task ExistsByEmailAsync_WhenEmailExists_ShouldReturnTrue()
    {
        // Arrange
        await using var context = _database.CreateContext();

        await SeedUserAsync(
            context,
            email: "customer@example.com",
            fullName: "Customer User");

        var repository = new UserRepository(context);

        // Act
        var result = await repository.ExistsByEmailAsync("customer@example.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsByEmailAsync_WhenEmailDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new UserRepository(context);

        // Act
        var result = await repository.ExistsByEmailAsync("missing@example.com");

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _database.Dispose();
    }

    private static async Task<AppRole> SeedRoleAsync(
        AppDbContext context,
        string roleName)
    {
        var role = new AppRole
        {
            Id = Guid.NewGuid(),
            Name = roleName,
            NormalizedName = roleName.ToUpperInvariant()
        };

        context.Roles.Add(role);
        await context.SaveChangesAsync();

        return role;
    }

    private static async Task<AppUser> SeedUserAsync(
        AppDbContext context,
        string email,
        string fullName,
        bool isActive = true,
        DateTime? createdAt = null)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            NormalizedEmail = email.ToUpperInvariant(),
            NormalizedUserName = email.ToUpperInvariant(),
            FullName = fullName,
            AvatarUrl = "avatar.png",
            IsActive = isActive,
            CreatedAt = createdAt ?? DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    private static async Task SeedUserRoleAsync(
        AppDbContext context,
        AppUser user,
        AppRole role)
    {
        context.UserRoles.Add(new AppUserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });

        await context.SaveChangesAsync();
    }
}
