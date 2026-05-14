using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RetailCore.Services.Options;
using RetailCore.Shared.Constants;

namespace RetailCore.UnitTests.Services;

public class IdentitySeederServiceTests
{
    private readonly Mock<RoleManager<AppRole>> _roleManagerMock;
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly IOptions<AdminOptions> _adminOptions;
    private readonly IdentitySeederService _service;

    public IdentitySeederServiceTests()
    {
        _roleManagerMock = MockRoleManager();
        _userManagerMock = MockUserManager();

        _adminOptions = Options.Create(new AdminOptions
        {
            Email = "admin@example.com",
            Password = "Admin@123456",
            FullName = "System Admin",
            AvatarUrl = "avatar.png"
        });

        _service = new IdentitySeederService(
            _roleManagerMock.Object,
            _userManagerMock.Object,
            _adminOptions);
    }

    [Fact]
    public async Task SeedAsync_WhenRolesDoNotExist_ShouldCreateAdminAndCustomerRoles()
    {
        // Arrange
        _roleManagerMock
            .Setup(manager => manager.RoleExistsAsync(RoleConstants.Admin))
            .ReturnsAsync(false);

        _roleManagerMock
            .Setup(manager => manager.RoleExistsAsync(RoleConstants.Customer))
            .ReturnsAsync(false);

        _roleManagerMock
            .Setup(manager => manager.CreateAsync(It.IsAny<AppRole>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(manager => manager.FindByEmailAsync(_adminOptions.Value.Email))
            .ReturnsAsync(new AppUser());

        // Act
        await _service.SeedAsync();

        // Assert
        _roleManagerMock.Verify(manager => manager.CreateAsync(
            It.Is<AppRole>(role =>
                role.Name == RoleConstants.Admin &&
                role.Description == "System administrator role")),
            Times.Once);

        _roleManagerMock.Verify(manager => manager.CreateAsync(
            It.Is<AppRole>(role =>
                role.Name == RoleConstants.Customer &&
                role.Description == "Customer role")),
            Times.Once);
    }

    [Fact]
    public async Task SeedAsync_WhenRolesAlreadyExist_ShouldNotCreateRoles()
    {
        // Arrange
        _roleManagerMock
            .Setup(manager => manager.RoleExistsAsync(RoleConstants.Admin))
            .ReturnsAsync(true);

        _roleManagerMock
            .Setup(manager => manager.RoleExistsAsync(RoleConstants.Customer))
            .ReturnsAsync(true);

        _userManagerMock
            .Setup(manager => manager.FindByEmailAsync(_adminOptions.Value.Email))
            .ReturnsAsync(new AppUser());

        // Act
        await _service.SeedAsync();

        // Assert
        _roleManagerMock.Verify(manager => manager.CreateAsync(It.IsAny<AppRole>()), Times.Never);
    }

    [Fact]
    public async Task SeedAsync_WhenAdminUserDoesNotExist_ShouldCreateAdminUser()
    {
        // Arrange
        _roleManagerMock
            .Setup(manager => manager.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _userManagerMock
            .Setup(manager => manager.FindByEmailAsync(_adminOptions.Value.Email))
            .ReturnsAsync((AppUser?)null);

        _userManagerMock
            .Setup(manager => manager.CreateAsync(It.IsAny<AppUser>(), _adminOptions.Value.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(manager => manager.AddToRoleAsync(It.IsAny<AppUser>(), RoleConstants.Admin))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _service.SeedAsync();

        // Assert
        _userManagerMock.Verify(manager => manager.CreateAsync(
            It.Is<AppUser>(user =>
                user.Email == _adminOptions.Value.Email &&
                user.UserName == _adminOptions.Value.Email &&
                user.FullName == _adminOptions.Value.FullName &&
                user.AvatarUrl == _adminOptions.Value.AvatarUrl),
            _adminOptions.Value.Password),
            Times.Once);
    }

    [Fact]
    public async Task SeedAsync_WhenAdminUserCreatedSuccessfully_ShouldAddAdminRole()
    {
        // Arrange
        _roleManagerMock
            .Setup(manager => manager.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _userManagerMock
            .Setup(manager => manager.FindByEmailAsync(_adminOptions.Value.Email))
            .ReturnsAsync((AppUser?)null);

        _userManagerMock
            .Setup(manager => manager.CreateAsync(It.IsAny<AppUser>(), _adminOptions.Value.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(manager => manager.AddToRoleAsync(It.IsAny<AppUser>(), RoleConstants.Admin))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _service.SeedAsync();

        // Assert
        _userManagerMock.Verify(manager => manager.AddToRoleAsync(
            It.Is<AppUser>(user => user.Email == _adminOptions.Value.Email),
            RoleConstants.Admin),
            Times.Once);
    }

    [Fact]
    public async Task SeedAsync_WhenAdminUserCreationFails_ShouldNotAddAdminRole()
    {
        // Arrange
        _roleManagerMock
            .Setup(manager => manager.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _userManagerMock
            .Setup(manager => manager.FindByEmailAsync(_adminOptions.Value.Email))
            .ReturnsAsync((AppUser?)null);

        _userManagerMock
            .Setup(manager => manager.CreateAsync(It.IsAny<AppUser>(), _adminOptions.Value.Password))
            .ReturnsAsync(IdentityResult.Failed(
                new IdentityError
                {
                    Description = "Create failed"
                }));

        // Act
        await _service.SeedAsync();

        // Assert
        _userManagerMock.Verify(manager => manager.AddToRoleAsync(
            It.IsAny<AppUser>(),
            It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task SeedAsync_WhenAdminUserAlreadyExists_ShouldNotCreateAdminUser()
    {
        // Arrange
        var existingAdmin = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = _adminOptions.Value.Email,
            UserName = _adminOptions.Value.Email,
            FullName = _adminOptions.Value.FullName
        };

        _roleManagerMock
            .Setup(manager => manager.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _userManagerMock
            .Setup(manager => manager.FindByEmailAsync(_adminOptions.Value.Email))
            .ReturnsAsync(existingAdmin);

        // Act
        await _service.SeedAsync();

        // Assert
        _userManagerMock.Verify(manager => manager.CreateAsync(
            It.IsAny<AppUser>(),
            It.IsAny<string>()),
            Times.Never);

        _userManagerMock.Verify(manager => manager.AddToRoleAsync(
            It.IsAny<AppUser>(),
            It.IsAny<string>()),
            Times.Never);
    }

    private static Mock<UserManager<AppUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<AppUser>>();

        var mock = new Mock<UserManager<AppUser>>(
            store.Object,
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<AppUser>>(),
            new List<IUserValidator<AppUser>>(),
            new List<IPasswordValidator<AppUser>>(),
            Mock.Of<ILookupNormalizer>(),
            new IdentityErrorDescriber(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<AppUser>>>());

        return mock;
    }

    private static Mock<RoleManager<AppRole>> MockRoleManager()
    {
        var store = new Mock<IRoleStore<AppRole>>();

        var mock = new Mock<RoleManager<AppRole>>(
            store.Object,
            new List<IRoleValidator<AppRole>>(),
            Mock.Of<ILookupNormalizer>(),
            new IdentityErrorDescriber(),
            Mock.Of<ILogger<RoleManager<AppRole>>>());

        return mock;
    }
}
