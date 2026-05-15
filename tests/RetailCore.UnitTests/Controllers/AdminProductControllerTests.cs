using Microsoft.AspNetCore.Mvc;
using RetailCore.Shared.Common;
using RetailCore.Shared.DTOs.Product;
using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.UnitTests.Controllers;

public class AdminProductControllerTests
{
    private readonly Mock<IProductService> _productServiceMock;
    private readonly AdminProductController _controller;

    public AdminProductControllerTests()
    {
        _productServiceMock = new Mock<IProductService>();
        _controller = new AdminProductController(_productServiceMock.Object);
    }

    [Fact]
    public async Task GetPagedForManagement_ShouldReturnOkWithPagedProducts()
    {
        // Arrange
        var request = new GetProductsRequest
        {
            Keyword = "iphone",
            Status = ProductStatus.Active,
            PageNumber = 1,
            PageSize = 10
        };

        var expectedResult = new PagedResult<ProductManagementDto>
        {
            Items =
            [
                new ProductManagementDto
                {
                    Id = Guid.NewGuid(),
                    Name = "iPhone 15",
                    Slug = "iphone-15",
                    ShortDescription = "Short description",
                    Description = "Full description",
                    BrandId = Guid.NewGuid(),
                    BrandName = "Apple",
                    CategoryId = Guid.NewGuid(),
                    CategoryName = "Phones",
                    Status = ProductStatus.Active,
                    VariantCount = 2,
                    Stock = 20,
                    MinPrice = 999,
                    MaxPrice = 1199,
                    ThumbnailUrl = "image.jpg",
                    CreatedAt = DateTime.UtcNow
                }
            ],
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = 1
        };

        _productServiceMock
            .Setup(service => service.GetPagedForManagementAsync(request))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetPagedForManagement(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PagedResult<ProductManagementDto>>(okResult.Value);

        Assert.Equal(expectedResult.PageNumber, value.PageNumber);
        Assert.Equal(expectedResult.PageSize, value.PageSize);
        Assert.Equal(expectedResult.TotalCount, value.TotalCount);

        var item = Assert.Single(value.Items);
        var expectedItem = expectedResult.Items.Single();

        Assert.Equal(expectedItem.Id, item.Id);
        Assert.Equal(expectedItem.Name, item.Name);
        Assert.Equal(expectedItem.Slug, item.Slug);
        Assert.Equal(expectedItem.BrandName, item.BrandName);
        Assert.Equal(expectedItem.CategoryName, item.CategoryName);
        Assert.Equal(expectedItem.Status, item.Status);

        _productServiceMock.Verify(
            service => service.GetPagedForManagementAsync(request),
            Times.Once);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var expectedProduct = CreateProductDetailDto(productId);

        _productServiceMock
            .Setup(service => service.GetByIdAsync(productId))
            .ReturnsAsync(expectedProduct);

        // Act
        var result = await _controller.GetById(productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<ProductDetailDto>(okResult.Value);

        Assert.Equal(expectedProduct.Id, value.Id);
        Assert.Equal(expectedProduct.Name, value.Name);
        Assert.Equal(expectedProduct.Slug, value.Slug);
        Assert.Equal(expectedProduct.BrandName, value.BrandName);
        Assert.Equal(expectedProduct.CategoryName, value.CategoryName);
        Assert.Equal(expectedProduct.Status, value.Status);

        _productServiceMock.Verify(
            service => service.GetByIdAsync(productId),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenRequestIsValid_ShouldReturnCreatedAtActionWithCreatedId()
    {
        // Arrange
        var request = CreateProductRequest();
        var createdId = Guid.NewGuid();

        _productServiceMock
            .Setup(service => service.CreateAsync(request))
            .ReturnsAsync(createdId);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);

        Assert.Equal(nameof(AdminProductController.GetById), createdResult.ActionName);
        Assert.Equal(createdId, createdResult.Value);
        Assert.NotNull(createdResult.RouteValues);
        Assert.Equal(createdId, createdResult.RouteValues["id"]);

        _productServiceMock.Verify(
            service => service.CreateAsync(request),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenNameIsEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = CreateProductRequest();
        request.Name = "";

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Create(request));

        // Assert
        Assert.Equal("Product name is required.", exception.Message);

        _productServiceMock.Verify(
            service => service.CreateAsync(It.IsAny<CreateProductRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenSlugIsEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = CreateProductRequest();
        request.Slug = " ";

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Create(request));

        // Assert
        Assert.Equal("Product slug is required.", exception.Message);

        _productServiceMock.Verify(
            service => service.CreateAsync(It.IsAny<CreateProductRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenBrandIdIsEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = CreateProductRequest();
        request.BrandId = Guid.Empty;

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Create(request));

        // Assert
        Assert.Equal("BrandId is required.", exception.Message);

        _productServiceMock.Verify(
            service => service.CreateAsync(It.IsAny<CreateProductRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenCategoryIdIsEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = CreateProductRequest();
        request.CategoryId = Guid.Empty;

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Create(request));

        // Assert
        Assert.Equal("CategoryId is required.", exception.Message);

        _productServiceMock.Verify(
            service => service.CreateAsync(It.IsAny<CreateProductRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_WhenRequestIsValid_ShouldReturnNoContent()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = UpdateProductRequest();

        _productServiceMock
            .Setup(service => service.UpdateAsync(productId, request))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(productId, request);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _productServiceMock.Verify(
            service => service.UpdateAsync(productId, request),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenNameIsEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = UpdateProductRequest();
        request.Name = "";

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Update(productId, request));

        // Assert
        Assert.Equal("Product name is required.", exception.Message);

        _productServiceMock.Verify(
            service => service.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateProductRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_WhenSlugIsEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = UpdateProductRequest();
        request.Slug = "";

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Update(productId, request));

        // Assert
        Assert.Equal("Product slug is required.", exception.Message);

        _productServiceMock.Verify(
            service => service.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateProductRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_WhenBrandIdIsEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = UpdateProductRequest();
        request.BrandId = Guid.Empty;

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Update(productId, request));

        // Assert
        Assert.Equal("BrandId is required.", exception.Message);

        _productServiceMock.Verify(
            service => service.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateProductRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_WhenCategoryIdIsEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = UpdateProductRequest();
        request.CategoryId = Guid.Empty;

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Update(productId, request));

        // Assert
        Assert.Equal("CategoryId is required.", exception.Message);

        _productServiceMock.Verify(
            service => service.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateProductRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _productServiceMock
            .Setup(service => service.DeleteAsync(productId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(productId);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _productServiceMock.Verify(
            service => service.DeleteAsync(productId),
            Times.Once);
    }

    [Fact]
    public async Task Publish_ShouldSetStatusToActiveAndReturnNoContent()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _productServiceMock
            .Setup(service => service.UpdateStatusAsync(productId, ProductStatus.Active))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Publish(productId);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _productServiceMock.Verify(
            service => service.UpdateStatusAsync(productId, ProductStatus.Active),
            Times.Once);
    }

    [Fact]
    public async Task UnpublishOrRestore_ShouldSetStatusToInactiveAndReturnNoContent()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _productServiceMock
            .Setup(service => service.UpdateStatusAsync(productId, ProductStatus.Inactive))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UnpublishOrRestore(productId);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _productServiceMock.Verify(
            service => service.UpdateStatusAsync(productId, ProductStatus.Inactive),
            Times.Once);
    }

    [Fact]
    public async Task Archive_ShouldSetStatusToArchivedAndReturnNoContent()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _productServiceMock
            .Setup(service => service.UpdateStatusAsync(productId, ProductStatus.Archived))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Archive(productId);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _productServiceMock.Verify(
            service => service.UpdateStatusAsync(productId, ProductStatus.Archived),
            Times.Once);
    }

    private static CreateProductRequest CreateProductRequest()
    {
        return new CreateProductRequest
        {
            Name = "iPhone 15",
            Slug = "iphone-15",
            ShortDescription = "Short description",
            Description = "Full description",
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Attributes =
            [
                new ProductAttributeRequest
                {
                    Name = "Color",
                    Values =
                    [
                        new ProductAttributeValueRequest
                        {
                            Value = "Black"
                        }
                    ]
                }
            ]
        };
    }

    private static UpdateProductRequest UpdateProductRequest()
    {
        return new UpdateProductRequest
        {
            Name = "iPhone 15 Updated",
            Slug = "iphone-15-updated",
            ShortDescription = "Updated short description",
            Description = "Updated full description",
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Attributes =
            [
                new ProductAttributeRequest
                {
                    Name = "Color",
                    Values =
                    [
                        new ProductAttributeValueRequest
                        {
                            Value = "Black"
                        }
                    ]
                }
            ]
        };
    }

    private static ProductDetailDto CreateProductDetailDto(Guid id)
    {
        var brandId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        return new ProductDetailDto
        {
            Id = id,
            Name = "iPhone 15",
            Slug = "iphone-15",
            ShortDescription = "Short description",
            Description = "Full description",
            BrandId = brandId,
            BrandName = "Apple",
            CategoryId = categoryId,
            CategoryName = "Phones",
            Status = ProductStatus.Active,
            Attributes = [],
            Variants = [],
            CreatedAt = DateTime.UtcNow
        };
    }
}
