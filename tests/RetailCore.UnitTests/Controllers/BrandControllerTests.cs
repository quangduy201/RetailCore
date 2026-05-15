using Microsoft.AspNetCore.Mvc;
using RetailCore.Shared.Common;
using RetailCore.Shared.DTOs.Brand;
using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Brand;

namespace RetailCore.UnitTests.Controllers;

public class BrandControllerTests
{
    private readonly Mock<IBrandService> _brandServiceMock;
    private readonly BrandController _controller;

    public BrandControllerTests()
    {
        _brandServiceMock = new Mock<IBrandService>();
        _controller = new BrandController(_brandServiceMock.Object);
    }

    [Fact]
    public async Task GetPaged_ShouldSetStatusToActiveAndReturnOkWithPagedBrands()
    {
        // Arrange
        var request = new GetBrandsRequest
        {
            Keyword = "apple",
            Status = BrandStatus.Inactive,
            PageNumber = 1,
            PageSize = 10
        };

        var expectedResult = new PagedResult<BrandSummaryDto>
        {
            Items =
            [
                new BrandSummaryDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Apple",
                    Slug = "apple"
                }
            ],
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = 1
        };

        _brandServiceMock
            .Setup(service => service.GetPagedAsync(request))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetPaged(request);

        // Assert
        Assert.Equal(BrandStatus.Active, request.Status);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PagedResult<BrandSummaryDto>>(okResult.Value);

        Assert.Equal(expectedResult.PageNumber, value.PageNumber);
        Assert.Equal(expectedResult.PageSize, value.PageSize);
        Assert.Equal(expectedResult.TotalCount, value.TotalCount);

        var item = Assert.Single(value.Items);
        var expectedItem = expectedResult.Items.Single();

        Assert.Equal(expectedItem.Id, item.Id);
        Assert.Equal(expectedItem.Name, item.Name);
        Assert.Equal(expectedItem.Slug, item.Slug);

        _brandServiceMock.Verify(
            service => service.GetPagedAsync(request),
            Times.Once);
    }

    [Fact]
    public async Task GetById_WhenBrandIsActive_ShouldReturnOkWithBrand()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        var expectedBrand = new BrandDetailDto
        {
            Id = brandId,
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _brandServiceMock
            .Setup(service => service.GetByIdAsync(brandId))
            .ReturnsAsync(expectedBrand);

        // Act
        var result = await _controller.GetById(brandId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<BrandDetailDto>(okResult.Value);

        Assert.Equal(expectedBrand.Id, value.Id);
        Assert.Equal(expectedBrand.Name, value.Name);
        Assert.Equal(expectedBrand.Slug, value.Slug);
        Assert.Equal(expectedBrand.Description, value.Description);
        Assert.Equal(BrandStatus.Active, value.Status);

        _brandServiceMock.Verify(
            service => service.GetByIdAsync(brandId),
            Times.Once);
    }

    [Fact]
    public async Task GetById_WhenBrandIsNotActive_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        var inactiveBrand = new BrandDetailDto
        {
            Id = brandId,
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Inactive,
            CreatedAt = DateTime.UtcNow
        };

        _brandServiceMock
            .Setup(service => service.GetByIdAsync(brandId))
            .ReturnsAsync(inactiveBrand);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _controller.GetById(brandId));

        // Assert
        Assert.Equal($"Brand id '{brandId}' not found.", exception.Message);

        _brandServiceMock.Verify(
            service => service.GetByIdAsync(brandId),
            Times.Once);
    }

    [Fact]
    public async Task GetBySlug_WhenBrandIsActive_ShouldReturnOkWithBrand()
    {
        // Arrange
        var slug = "apple";

        var expectedBrand = new BrandDetailDto
        {
            Id = Guid.NewGuid(),
            Name = "Apple",
            Slug = slug,
            Description = "Apple products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _brandServiceMock
            .Setup(service => service.GetBySlugAsync(slug))
            .ReturnsAsync(expectedBrand);

        // Act
        var result = await _controller.GetBySlug(slug);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<BrandDetailDto>(okResult.Value);

        Assert.Equal(expectedBrand.Id, value.Id);
        Assert.Equal(expectedBrand.Name, value.Name);
        Assert.Equal(expectedBrand.Slug, value.Slug);
        Assert.Equal(expectedBrand.Description, value.Description);
        Assert.Equal(BrandStatus.Active, value.Status);

        _brandServiceMock.Verify(
            service => service.GetBySlugAsync(slug),
            Times.Once);
    }

    [Fact]
    public async Task GetBySlug_WhenBrandIsNotActive_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var slug = "inactive-brand";

        var inactiveBrand = new BrandDetailDto
        {
            Id = Guid.NewGuid(),
            Name = "Inactive Brand",
            Slug = slug,
            Description = "Inactive brand",
            Status = BrandStatus.Inactive,
            CreatedAt = DateTime.UtcNow
        };

        _brandServiceMock
            .Setup(service => service.GetBySlugAsync(slug))
            .ReturnsAsync(inactiveBrand);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _controller.GetBySlug(slug));

        // Assert
        Assert.Equal($"Brand slug '{slug}' not found.", exception.Message);

        _brandServiceMock.Verify(
            service => service.GetBySlugAsync(slug),
            Times.Once);
    }
}
