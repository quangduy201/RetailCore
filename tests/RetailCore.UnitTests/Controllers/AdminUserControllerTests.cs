using Microsoft.AspNetCore.Mvc;
using RetailCore.Shared.DTOs.User;

namespace RetailCore.UnitTests.Controllers;

public class AdminUserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly AdminUsersController _controller;

    public AdminUserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _controller = new AdminUsersController(_userServiceMock.Object);
    }

    [Fact]
    public async Task GetCustomers_ShouldReturnOkWithCustomers()
    {
        // Arrange
        var expectedCustomers = new List<UserDto>
        {
            CreateUserDto(
                email: "customer1@example.com",
                fullName: "Customer One",
                isActive: true,
                roles: ["Customer"]),

            CreateUserDto(
                email: "customer2@example.com",
                fullName: "Customer Two",
                isActive: false,
                roles: ["Customer", "VIP"])
        };

        _userServiceMock
            .Setup(service => service.GetCustomersAsync())
            .ReturnsAsync(expectedCustomers);

        // Act
        var result = await _controller.GetCustomers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<UserDto>>(okResult.Value);

        Assert.Equal(2, value.Count);

        Assert.Equal(expectedCustomers[0].Id, value[0].Id);
        Assert.Equal("customer1@example.com", value[0].Email);
        Assert.Equal("Customer One", value[0].FullName);
        Assert.True(value[0].IsActive);
        Assert.Single(value[0].Roles);
        Assert.Equal("Customer", value[0].Roles[0]);

        Assert.Equal(expectedCustomers[1].Id, value[1].Id);
        Assert.Equal("customer2@example.com", value[1].Email);
        Assert.Equal("Customer Two", value[1].FullName);
        Assert.False(value[1].IsActive);
        Assert.Equal(2, value[1].Roles.Count);
        Assert.Equal("Customer", value[1].Roles[0]);
        Assert.Equal("VIP", value[1].Roles[1]);

        _userServiceMock.Verify(
            service => service.GetCustomersAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetCustomers_WhenNoCustomersExist_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        _userServiceMock
            .Setup(service => service.GetCustomersAsync())
            .ReturnsAsync([]);

        // Act
        var result = await _controller.GetCustomers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<UserDto>>(okResult.Value);

        Assert.Empty(value);

        _userServiceMock.Verify(
            service => service.GetCustomersAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetById_WhenUserExists_ShouldReturnOkWithUser()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var expectedUser = CreateUserDto(
            id: userId,
            email: "customer@example.com",
            fullName: "Customer User",
            isActive: true,
            roles: ["Customer"]);

        _userServiceMock
            .Setup(service => service.GetByIdAsync(userId))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _controller.GetById(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<UserDto>(okResult.Value);

        Assert.Equal(expectedUser.Id, value.Id);
        Assert.Equal(expectedUser.Email, value.Email);
        Assert.Equal(expectedUser.FullName, value.FullName);
        Assert.Equal(expectedUser.AvatarUrl, value.AvatarUrl);
        Assert.Equal(expectedUser.IsActive, value.IsActive);
        Assert.Equal(expectedUser.CreatedAt, value.CreatedAt);
        Assert.Single(value.Roles);
        Assert.Equal("Customer", value.Roles[0]);

        _userServiceMock.Verify(
            service => service.GetByIdAsync(userId),
            Times.Once);
    }

    [Fact]
    public async Task GetById_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userServiceMock
            .Setup(service => service.GetByIdAsync(userId))
            .ReturnsAsync((UserDto?)null);

        // Act
        var result = await _controller.GetById(userId);

        // Assert
        Assert.IsType<NotFoundResult>(result);

        _userServiceMock.Verify(
            service => service.GetByIdAsync(userId),
            Times.Once);
    }

    [Fact]
    public async Task ToggleActiveStatus_ShouldReturnNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userServiceMock
            .Setup(service => service.ToggleActiveStatusAsync(userId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ToggleActiveStatus(userId);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _userServiceMock.Verify(
            service => service.ToggleActiveStatusAsync(userId),
            Times.Once);
    }

    private static UserDto CreateUserDto(
        string email = "customer@example.com",
        string fullName = "Customer User",
        bool isActive = true,
        List<string>? roles = null,
        Guid? id = null)
    {
        return new UserDto
        {
            Id = id ?? Guid.NewGuid(),
            Email = email,
            FullName = fullName,
            AvatarUrl = "avatar.png",
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow,
            Roles = roles ?? ["Customer"]
        };
    }
}
