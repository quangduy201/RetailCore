using Microsoft.AspNetCore.Mvc;
using RetailCore.Shared.Common;
using RetailCore.Shared.DTOs.Category;
using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Category;

namespace RetailCore.UnitTests.Controllers;

public class CategoryControllerTests
{
    private readonly Mock<ICategoryService> _categoryServiceMock;
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
        _categoryServiceMock = new Mock<ICategoryService>();
        _controller = new CategoryController(_categoryServiceMock.Object);
    }

    [Fact]
    public async Task GetPaged_ShouldSetStatusToActiveAndReturnOkWithPagedCategories()
    {
        // Arrange
        var request = new GetCategoriesRequest
        {
            Keyword = "phone",
            Status = CategoryStatus.Inactive,
            PageNumber = 1,
            PageSize = 10
        };

        var expectedResult = new PagedResult<CategorySummaryDto>
        {
            Items =
            [
                new CategorySummaryDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Phones",
                    Slug = "phones",
                    Description = "Phone category"
                }
            ],
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = 1
        };

        _categoryServiceMock
            .Setup(service => service.GetPagedAsync(request))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetPaged(request);

        // Assert
        Assert.Equal(CategoryStatus.Active, request.Status);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PagedResult<CategorySummaryDto>>(okResult.Value);

        Assert.Equal(expectedResult.PageNumber, value.PageNumber);
        Assert.Equal(expectedResult.PageSize, value.PageSize);
        Assert.Equal(expectedResult.TotalCount, value.TotalCount);

        var item = Assert.Single(value.Items);
        var expectedItem = expectedResult.Items.Single();

        Assert.Equal(expectedItem.Id, item.Id);
        Assert.Equal(expectedItem.Name, item.Name);
        Assert.Equal(expectedItem.Slug, item.Slug);
        Assert.Equal(expectedItem.Description, item.Description);

        _categoryServiceMock.Verify(
            service => service.GetPagedAsync(request),
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
}
