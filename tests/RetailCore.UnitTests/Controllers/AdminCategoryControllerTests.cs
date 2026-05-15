using Microsoft.AspNetCore.Mvc;
using RetailCore.Shared.Common;
using RetailCore.Shared.DTOs.Category;
using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Category;

namespace RetailCore.UnitTests.Controllers;

public class AdminCategoryControllerTests
{
    private readonly Mock<ICategoryService> _categoryServiceMock;
    private readonly AdminCategoryController _controller;

    public AdminCategoryControllerTests()
    {
        _categoryServiceMock = new Mock<ICategoryService>();
        _controller = new AdminCategoryController(_categoryServiceMock.Object);
    }

    [Fact]
    public async Task GetPagedForManagement_ShouldReturnOkWithPagedCategories()
    {
        // Arrange
        var request = new GetCategoriesRequest
        {
            Keyword = "phone",
            Status = CategoryStatus.Active,
            PageNumber = 1,
            PageSize = 10
        };

        var expectedResult = new PagedResult<CategoryDetailDto>
        {
            Items =
            [
                new CategoryDetailDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Phones",
                    Slug = "phones",
                    Description = "Phone category",
                    Status = CategoryStatus.Active,
                    CreatedAt = DateTime.UtcNow
                }
            ],
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = 1
        };

        _categoryServiceMock
            .Setup(service => service.GetPagedForManagementAsync(request))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetPagedForManagement(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PagedResult<CategoryDetailDto>>(okResult.Value);

        Assert.Equal(expectedResult.PageNumber, value.PageNumber);
        Assert.Equal(expectedResult.PageSize, value.PageSize);
        Assert.Equal(expectedResult.TotalCount, value.TotalCount);

        var item = Assert.Single(value.Items);
        var expectedItem = expectedResult.Items.Single();

        Assert.Equal(expectedItem.Id, item.Id);
        Assert.Equal(expectedItem.Name, item.Name);
        Assert.Equal(expectedItem.Slug, item.Slug);
        Assert.Equal(expectedItem.Description, item.Description);
        Assert.Equal(expectedItem.Status, item.Status);

        _categoryServiceMock.Verify(
            service => service.GetPagedForManagementAsync(request),
            Times.Once);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var expectedCategory = new CategoryDetailDto
        {
            Id = categoryId,
            Name = "Laptops",
            Slug = "laptops",
            Description = "Laptop category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _categoryServiceMock
            .Setup(service => service.GetByIdAsync(categoryId))
            .ReturnsAsync(expectedCategory);

        // Act
        var result = await _controller.GetById(categoryId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<CategoryDetailDto>(okResult.Value);

        Assert.Equal(expectedCategory.Id, value.Id);
        Assert.Equal(expectedCategory.Name, value.Name);
        Assert.Equal(expectedCategory.Slug, value.Slug);
        Assert.Equal(expectedCategory.Description, value.Description);
        Assert.Equal(expectedCategory.Status, value.Status);

        _categoryServiceMock.Verify(
            service => service.GetByIdAsync(categoryId),
            Times.Once);
    }

    [Fact]
    public async Task GetBySlug_ShouldReturnOkWithCategory()
    {
        // Arrange
        var slug = "laptops";

        var expectedCategory = new CategoryDetailDto
        {
            Id = Guid.NewGuid(),
            Name = "Laptops",
            Slug = slug,
            Description = "Laptop category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _categoryServiceMock
            .Setup(service => service.GetBySlugAsync(slug))
            .ReturnsAsync(expectedCategory);

        // Act
        var result = await _controller.GetBySlug(slug);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<CategoryDetailDto>(okResult.Value);

        Assert.Equal(expectedCategory.Id, value.Id);
        Assert.Equal(expectedCategory.Name, value.Name);
        Assert.Equal(expectedCategory.Slug, value.Slug);
        Assert.Equal(expectedCategory.Description, value.Description);
        Assert.Equal(expectedCategory.Status, value.Status);

        _categoryServiceMock.Verify(
            service => service.GetBySlugAsync(slug),
            Times.Once);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAtActionWithCreatedId()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Name = "Accessories",
            Slug = "accessories",
            Description = "Accessories category"
        };

        var createdId = Guid.NewGuid();

        _categoryServiceMock
            .Setup(service => service.CreateAsync(request))
            .ReturnsAsync(createdId);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);

        Assert.Equal(nameof(AdminCategoryController.GetById), createdResult.ActionName);
        Assert.Equal(createdId, createdResult.Value);
        Assert.NotNull(createdResult.RouteValues);
        Assert.Equal(createdId, createdResult.RouteValues["id"]);

        _categoryServiceMock.Verify(
            service => service.CreateAsync(request),
            Times.Once);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var request = new UpdateCategoryRequest
        {
            Name = "Updated Category",
            Slug = "updated-category",
            Description = "Updated description",
            Status = CategoryStatus.Inactive
        };

        _categoryServiceMock
            .Setup(service => service.UpdateAsync(categoryId, request))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(categoryId, request);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _categoryServiceMock.Verify(
            service => service.UpdateAsync(categoryId, request),
            Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _categoryServiceMock
            .Setup(service => service.DeleteAsync(categoryId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(categoryId);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _categoryServiceMock.Verify(
            service => service.DeleteAsync(categoryId),
            Times.Once);
    }
}
