using Microsoft.AspNetCore.Mvc;
using RetailCore.Shared.Common;
using RetailCore.Shared.DTOs.Brand;
using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Brand;

namespace RetailCore.UnitTests.Controllers;

public class AdminBrandControllerTests
{
    private readonly Mock<IBrandService> _brandServiceMock;
    private readonly AdminBrandController _controller;

    public AdminBrandControllerTests()
    {
        _brandServiceMock = new Mock<IBrandService>();
        _controller = new AdminBrandController(_brandServiceMock.Object);
    }

    [Fact]
    public async Task GetPagedForManagement_ShouldReturnOkWithPagedBrands()
    {
        // Arrange
        var request = new GetBrandsRequest
        {
            Keyword = "apple",
            Status = BrandStatus.Active,
            PageNumber = 1,
            PageSize = 10
        };

        var expectedResult = new PagedResult<BrandDetailDto>
        {
            Items =
            [
                new BrandDetailDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Apple",
                    Slug = "apple",
                    Description = "Apple products",
                    Status = BrandStatus.Active,
                    CreatedAt = DateTime.UtcNow
                }
            ],
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = 1
        };

        _brandServiceMock
            .Setup(service => service.GetPagedForManagementAsync(request))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetPagedForManagement(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PagedResult<BrandDetailDto>>(okResult.Value);

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

        _brandServiceMock.Verify(
            service => service.GetPagedForManagementAsync(request),
            Times.Once);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithBrand()
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
        Assert.Equal(expectedBrand.Status, value.Status);

        _brandServiceMock.Verify(
            service => service.GetByIdAsync(brandId),
            Times.Once);
    }

    [Fact]
    public async Task GetBySlug_ShouldReturnOkWithBrand()
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
        Assert.Equal(expectedBrand.Status, value.Status);

        _brandServiceMock.Verify(
            service => service.GetBySlugAsync(slug),
            Times.Once);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAtActionWithCreatedId()
    {
        // Arrange
        var request = new CreateBrandRequest
        {
            Name = "Samsung",
            Slug = "samsung",
            Description = "Samsung products"
        };

        var createdId = Guid.NewGuid();

        _brandServiceMock
            .Setup(service => service.CreateAsync(request))
            .ReturnsAsync(createdId);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);

        Assert.Equal(nameof(AdminBrandController.GetById), createdResult.ActionName);
        Assert.Equal(createdId, createdResult.Value);
        Assert.NotNull(createdResult.RouteValues);
        Assert.Equal(createdId, createdResult.RouteValues["id"]);

        _brandServiceMock.Verify(
            service => service.CreateAsync(request),
            Times.Once);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        var request = new UpdateBrandRequest
        {
            Name = "Updated Brand",
            Slug = "updated-brand",
            Description = "Updated description",
            Status = BrandStatus.Inactive
        };

        _brandServiceMock
            .Setup(service => service.UpdateAsync(brandId, request))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(brandId, request);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _brandServiceMock.Verify(
            service => service.UpdateAsync(brandId, request),
            Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        _brandServiceMock
            .Setup(service => service.DeleteAsync(brandId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(brandId);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _brandServiceMock.Verify(
            service => service.DeleteAsync(brandId),
            Times.Once);
    }
}
