using Moq;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Services.Implementations;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.UnitTests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<IProductAttributeRepository> _mockAttributeRepo;
    private readonly Mock<IProductVariantService> _mockVariantService;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockProductRepo = new Mock<IProductRepository>();
        _mockAttributeRepo = new Mock<IProductAttributeRepository>();
        _mockVariantService = new Mock<IProductVariantService>();
        _service = new ProductService(_mockProductRepo.Object, _mockAttributeRepo.Object, _mockVariantService.Object);
    }

    #region GetPagedAsync Tests

    [Fact]
    public async Task GetPagedAsync_WithValidRequest_ReturnsPagedSummaryDtos()
    {
        // Arrange
        var request = new GetProductsRequest { PageNumber = 1, PageSize = 10 };
        var products = new List<Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product 1",
                Slug = "product-1",
                Brand = new Brand { Name = "Brand 1" },
                Category = new Category { Name = "Cat 1" },
                Variants = new List<ProductVariant>
                {
                    new() { Price = 100, Stock = 5, Images = new List<ProductVariantImage>() }
                }
            }
        };

        _mockProductRepo.Setup(r => r.GetPagedAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<ProductStatus?>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((products, 1));

        // Act
        var result = await _service.GetPagedAsync(request);

        // Assert
        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task GetPagedAsync_WithEmptyResult_ReturnsEmpty()
    {
        // Arrange
        var request = new GetProductsRequest { PageNumber = 1, PageSize = 10 };
        _mockProductRepo.Setup(r => r.GetPagedAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<ProductStatus?>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((new List<Product>(), 0));

        // Act
        var result = await _service.GetPagedAsync(request);

        // Assert
        Assert.Empty(result.Items);
    }

    #endregion

    #region GetPagedForManagementAsync Tests

    [Fact]
    public async Task GetPagedForManagementAsync_WithValidRequest_ReturnsPagedManagementDtos()
    {
        // Arrange
        var request = new GetProductsRequest { PageNumber = 1, PageSize = 10 };
        var products = new List<Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product 1",
                Slug = "product-1",
                Brand = new Brand { Name = "Brand 1" },
                Category = new Category { Name = "Cat 1" },
                Status = ProductStatus.Active,
                Variants = new List<ProductVariant>
                {
                    new() { Price = 100, Stock = 5 }
                },
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockProductRepo.Setup(r => r.GetPagedForManagementAsync(It.IsAny<string>(), It.IsAny<ProductStatus?>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((products, 1));

        // Act
        var result = await _service.GetPagedForManagementAsync(request);

        // Assert
        Assert.Single(result.Items);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsProduct()
    {
        // Arrange
        var id = Guid.NewGuid();
        var product = new Product
        {
            Id = id,
            Name = "Test Product",
            Brand = new Brand { Name = "Brand 1" },
            Category = new Category { Name = "Cat 1" },
            Attributes = new List<ProductAttribute>(),
            Variants = new List<ProductVariant>()
        };

        _mockProductRepo.Setup(r => r.GetByIdWithDetailsAsync(id))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Test Product", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockProductRepo.Setup(r => r.GetByIdWithDetailsAsync(id))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(id));
    }

    #endregion

    #region GetBySlugAsync Tests

    [Fact]
    public async Task GetBySlugAsync_WithValidSlug_ReturnsProduct()
    {
        // Arrange
        const string slug = "test-product";
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Slug = slug,
            Brand = new Brand(),
            Category = new Category(),
            Attributes = new List<ProductAttribute>(),
            Variants = new List<ProductVariant>()
        };

        _mockProductRepo.Setup(r => r.GetBySlugWithDetailsAsync(slug))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetBySlugAsync(slug);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(slug, result.Slug);
    }

    [Fact]
    public async Task GetBySlugAsync_WithInvalidSlug_ThrowsKeyNotFoundException()
    {
        // Arrange
        const string slug = "invalid";
        _mockProductRepo.Setup(r => r.GetBySlugWithDetailsAsync(slug))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetBySlugAsync(slug));
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidRequest_ReturnsProductId()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "New Product",
            Slug = "new-product",
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Attributes = new List<CreateProductAttributeRequest>(),
            Variants = new List<CreateProductVariantRequest>()
        };

        _mockProductRepo.Setup(r => r.IsSlugUniqueAsync(request.Slug))
            .ReturnsAsync(true);
        _mockProductRepo.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _mockProductRepo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateSlug_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Product",
            Slug = "existing-slug",
            Attributes = new List<CreateProductAttributeRequest>(),
            Variants = new List<CreateProductVariantRequest>()
        };

        _mockProductRepo.Setup(r => r.IsSlugUniqueAsync(request.Slug))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_WithAttributes_CreatesAttributesForProduct()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Product",
            Slug = "prod",
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Attributes = new List<CreateProductAttributeRequest>
            {
                new() { Name = "Color" }
            },
            Variants = new List<CreateProductVariantRequest>()
        };

        _mockProductRepo.Setup(r => r.IsSlugUniqueAsync(request.Slug))
            .ReturnsAsync(true);
        _mockProductRepo.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.CreateAsync(request);

        // Assert
        _mockAttributeRepo.Verify(r => r.AddAsync(It.IsAny<ProductAttribute>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CreatedProductHasDraftStatus()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Product",
            Slug = "prod",
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Attributes = new List<CreateProductAttributeRequest>(),
            Variants = new List<CreateProductVariantRequest>()
        };

        Product? capturedProduct = null;
        _mockProductRepo.Setup(r => r.IsSlugUniqueAsync(request.Slug))
            .ReturnsAsync(true);
        _mockProductRepo.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .Callback<Product>(p => capturedProduct = p)
            .Returns(Task.CompletedTask);

        // Act
        await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(capturedProduct);
        Assert.Equal(ProductStatus.Draft, capturedProduct.Status);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidRequest_UpdatesProduct()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingProduct = new Product { Id = id, Name = "Old", Slug = "old-slug" };
        var request = new UpdateProductRequest
        {
            Name = "Updated",
            Slug = "updated",
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Status = ProductStatus.Active
        };

        _mockProductRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(existingProduct);
        _mockProductRepo.Setup(r => r.IsSlugUniqueAsync(request.Slug, id))
            .ReturnsAsync(true);
        _mockProductRepo.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(id, request);

        // Assert
        Assert.Equal("Updated", existingProduct.Name);
        _mockProductRepo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateProductRequest { Name = "Test", Slug = "test", BrandId = Guid.NewGuid(), CategoryId = Guid.NewGuid() };

        _mockProductRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(id, request));
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateSlug_ThrowsInvalidOperationException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingProduct = new Product { Id = id, Name = "Old", Slug = "old-slug" };
        var request = new UpdateProductRequest
        {
            Name = "Test",
            Slug = "new-slug",
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid()
        };

        _mockProductRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(existingProduct);
        _mockProductRepo.Setup(r => r.IsSlugUniqueAsync(request.Slug, id))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(id, request));
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesProduct()
    {
        // Arrange
        var id = Guid.NewGuid();
        var product = new Product { Id = id, Name = "Test" };

        _mockProductRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(product);
        _mockProductRepo.Setup(r => r.DeleteAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(id);

        // Assert
        _mockProductRepo.Verify(r => r.DeleteAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockProductRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(id));
    }

    #endregion
}
