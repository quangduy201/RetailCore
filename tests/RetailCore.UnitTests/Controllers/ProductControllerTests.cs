using Microsoft.AspNetCore.Mvc;
using RetailCore.Shared.Common;
using RetailCore.Shared.DTOs.Product;
using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.UnitTests.Controllers;

public class ProductControllerTests
{
    private readonly Mock<IProductService> _productServiceMock;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _productServiceMock = new Mock<IProductService>();
        _controller = new ProductController(_productServiceMock.Object);
    }

    [Fact]
    public async Task GetPaged_ShouldSetStatusToActiveAndReturnOkWithPagedProducts()
    {
        // Arrange
        var request = new GetProductsRequest
        {
            Keyword = "iphone",
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Status = ProductStatus.Draft,
            PageNumber = 1,
            PageSize = 10
        };

        var expectedResult = new PagedResult<ProductSummaryDto>
        {
            Items =
            [
                new ProductSummaryDto
                {
                    Id = Guid.NewGuid(),
                    Name = "iPhone 15",
                    Slug = "iphone-15",
                    BrandName = "Apple",
                    CategoryName = "Phones",
                    Price = 999,
                    CompareAtPrice = 1099,
                    DiscountPercentage = 10,
                    ThumbnailUrl = "image.jpg",
                    IsOutOfStock = false
                }
            ],
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = 1
        };

        _productServiceMock
            .Setup(service => service.GetPagedAsync(request))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetPaged(request);

        // Assert
        Assert.Equal(ProductStatus.Active, request.Status);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PagedResult<ProductSummaryDto>>(okResult.Value);

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
        Assert.Equal(expectedItem.Price, item.Price);
        Assert.Equal(expectedItem.CompareAtPrice, item.CompareAtPrice);
        Assert.Equal(expectedItem.DiscountPercentage, item.DiscountPercentage);
        Assert.Equal(expectedItem.ThumbnailUrl, item.ThumbnailUrl);
        Assert.Equal(expectedItem.IsOutOfStock, item.IsOutOfStock);

        _productServiceMock.Verify(
            service => service.GetPagedAsync(request),
            Times.Once);
    }

    [Fact]
    public async Task GetById_WhenProductIsActive_ShouldReturnOkWithProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var expectedProduct = CreateProductDetailDto(
            id: productId,
            status: ProductStatus.Active);

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
        Assert.Equal(ProductStatus.Active, value.Status);

        _productServiceMock.Verify(
            service => service.GetByIdAsync(productId),
            Times.Once);
    }

    [Fact]
    public async Task GetById_WhenProductIsNotActive_ShouldReturnNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var product = CreateProductDetailDto(
            id: productId,
            status: ProductStatus.Draft);

        _productServiceMock
            .Setup(service => service.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetById(productId);

        // Assert
        Assert.IsType<NotFoundResult>(result);

        _productServiceMock.Verify(
            service => service.GetByIdAsync(productId),
            Times.Once);
    }

    [Fact]
    public async Task GetBySlug_WhenProductIsActive_ShouldReturnOkWithProduct()
    {
        // Arrange
        var slug = "iphone-15";

        var expectedProduct = CreateProductDetailDto(
            slug: slug,
            status: ProductStatus.Active);

        _productServiceMock
            .Setup(service => service.GetBySlugAsync(slug))
            .ReturnsAsync(expectedProduct);

        // Act
        var result = await _controller.GetBySlug(slug);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<ProductDetailDto>(okResult.Value);

        Assert.Equal(expectedProduct.Id, value.Id);
        Assert.Equal(expectedProduct.Name, value.Name);
        Assert.Equal(slug, value.Slug);
        Assert.Equal(expectedProduct.BrandName, value.BrandName);
        Assert.Equal(expectedProduct.CategoryName, value.CategoryName);
        Assert.Equal(ProductStatus.Active, value.Status);

        _productServiceMock.Verify(
            service => service.GetBySlugAsync(slug),
            Times.Once);
    }

    [Fact]
    public async Task GetBySlug_WhenProductIsNotActive_ShouldReturnNotFound()
    {
        // Arrange
        var slug = "draft-product";

        var product = CreateProductDetailDto(
            slug: slug,
            status: ProductStatus.Draft);

        _productServiceMock
            .Setup(service => service.GetBySlugAsync(slug))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetBySlug(slug);

        // Assert
        Assert.IsType<NotFoundResult>(result);

        _productServiceMock.Verify(
            service => service.GetBySlugAsync(slug),
            Times.Once);
    }

    private static ProductDetailDto CreateProductDetailDto(
        Guid? id = null,
        string slug = "iphone-15",
        ProductStatus status = ProductStatus.Active)
    {
        var brandId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        return new ProductDetailDto
        {
            Id = id ?? Guid.NewGuid(),
            Name = "iPhone 15",
            Slug = slug,
            ShortDescription = "Short description",
            Description = "Full description",
            BrandId = brandId,
            BrandName = "Apple",
            CategoryId = categoryId,
            CategoryName = "Phones",
            Status = status,
            Attributes = [],
            Variants = [],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
    }
}
