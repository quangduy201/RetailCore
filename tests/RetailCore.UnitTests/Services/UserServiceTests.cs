using Microsoft.Extensions.Options;
using RetailCore.Services.Options;

namespace RetailCore.UnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IOptions<AdminOptions> _adminOptions;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _adminOptions = Options.Create(new AdminOptions
        {
            Email = "admin@retailcore.com",
            FullName = "System Admin",
            Password = "Admin@123"
        });

        _userService = new UserService(
            _userRepoMock.Object,
            _adminOptions,
            _unitOfWorkMock.Object);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task GetCustomersAsync_WhenUsersExist_ShouldReturnUserDtos()
    {
        // Arrange
        var users = new List<AppUser>
        {
            CreateUser(
                email: "customer1@example.com",
                fullName: "Customer One",
                isActive: true,
                roles: ["Customer"]),

            CreateUser(
                email: "customer2@example.com",
                fullName: "Customer Two",
                isActive: false,
                roles: ["Customer", "Admin"])
        };

        _userRepoMock
            .Setup(repo => repo.GetCustomersAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _userService.GetCustomersAsync();

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Equal(users[0].Id, result[0].Id);
        Assert.Equal("customer1@example.com", result[0].Email);
        Assert.Equal("Customer One", result[0].FullName);
        Assert.True(result[0].IsActive);
        Assert.Equal(users[0].CreatedAt, result[0].CreatedAt);
        Assert.Single(result[0].Roles);
        Assert.Equal("Customer", result[0].Roles[0]);

        Assert.Equal(users[1].Id, result[1].Id);
        Assert.Equal("customer2@example.com", result[1].Email);
        Assert.Equal("Customer Two", result[1].FullName);
        Assert.False(result[1].IsActive);
        Assert.Equal(users[1].CreatedAt, result[1].CreatedAt);
        Assert.Equal(2, result[1].Roles.Count);
        Assert.Equal("Customer", result[1].Roles[0]);
        Assert.Equal("Admin", result[1].Roles[1]);
    }

    [Fact]
    public async Task GetCustomersAsync_WhenNoUsersExist_ShouldReturnEmptyList()
    {
        // Arrange
        _userRepoMock
            .Setup(repo => repo.GetCustomersAsync())
            .ReturnsAsync([]);

        // Act
        var result = await _userService.GetCustomersAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ShouldReturnUserDto()
    {
        // Arrange
        var user = CreateUser(
            email: "customer@example.com",
            fullName: "Customer User",
            isActive: true,
            roles: ["Customer"]);

        _userRepoMock
            .Setup(repo => repo.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(user.Id, result!.Id);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.FullName, result.FullName);
        Assert.Equal(user.IsActive, result.IsActive);
        Assert.Equal(user.CreatedAt, result.CreatedAt);

        Assert.Single(result.Roles);
        Assert.Equal("Customer", result.Roles[0]);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepoMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((AppUser?)null);

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ToggleActiveStatusAsync_WhenUserIsActive_ShouldSetUserInactive()
    {
        // Arrange
        var user = CreateUser(isActive: true);

        _userRepoMock
            .Setup(repo => repo.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        // Act
        await _userService.ToggleActiveStatusAsync(user.Id);

        // Assert
        Assert.False(user.IsActive);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ToggleActiveStatusAsync_WhenUserIsInactive_ShouldSetUserActive()
    {
        // Arrange
        var user = CreateUser(isActive: false);

        _userRepoMock
            .Setup(repo => repo.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        // Act
        await _userService.ToggleActiveStatusAsync(user.Id);

        // Assert
        Assert.True(user.IsActive);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ToggleActiveStatusAsync_WhenUserDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepoMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((AppUser?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _userService.ToggleActiveStatusAsync(userId));

        // Assert
        Assert.Equal("User not found.", exception.Message);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    private static AppUser CreateUser(
        string email = "user@example.com",
        string fullName = "Test User",
        bool isActive = true,
        List<string>? roles = null)
    {
        roles ??= ["Customer"];

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            FullName = fullName,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow,
            UserRoles = []
        };

        foreach (var roleName in roles)
        {
            user.UserRoles.Add(new AppUserRole
            {
                UserId = user.Id,
                User = user,
                RoleId = Guid.NewGuid(),
                Role = new AppRole
                {
                    Id = Guid.NewGuid(),
                    Name = roleName
                }
            });
        }

        return user;
    }
}
