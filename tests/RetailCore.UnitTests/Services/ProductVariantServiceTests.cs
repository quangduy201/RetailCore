using Moq;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Services.Implementations;
using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.UnitTests.Services;

public class ProductVariantServiceTests
{
    private readonly Mock<IProductVariantRepository> _mockVariantRepo;
    private readonly Mock<IProductVariantImageRepository> _mockImageRepo;
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly ProductVariantService _service;

    public ProductVariantServiceTests()
    {
        _mockVariantRepo = new Mock<IProductVariantRepository>();
        _mockImageRepo = new Mock<IProductVariantImageRepository>();
        _mockProductRepo = new Mock<IProductRepository>();
        _service = new ProductVariantService(_mockVariantRepo.Object, _mockImageRepo.Object, _mockProductRepo.Object);
    }

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsVariant()
    {
        // Arrange
        var id = Guid.NewGuid();
        var variant = new ProductVariant
        {
            Id = id,
            Sku = "SKU-001",
            Name = "Variant 1",
            Price = 100,
            Stock = 10,
            Status = ProductVariantStatus.Active
        };

        _mockVariantRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(variant);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("SKU-001", result.Sku);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockVariantRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((ProductVariant?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(id));
    }

    #endregion

    #region GetByProductIdAsync Tests

    [Fact]
    public async Task GetByProductIdAsync_WithValidProductId_ReturnsVariants()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var variants = new List<ProductVariant>
        {
            new() { Id = Guid.NewGuid(), Sku = "SKU-001", Price = 100 },
            new() { Id = Guid.NewGuid(), Sku = "SKU-002", Price = 200 }
        };

        _mockVariantRepo.Setup(r => r.GetByProductIdAsync(productId))
            .ReturnsAsync(variants);

        // Act
        var result = await _service.GetByProductIdAsync(productId);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByProductIdAsync_WithNoVariants_ReturnsEmptyList()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mockVariantRepo.Setup(r => r.GetByProductIdAsync(productId))
            .ReturnsAsync(new List<ProductVariant>());

        // Act
        var result = await _service.GetByProductIdAsync(productId);

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidRequest_ReturnsVariantId()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = new CreateProductVariantRequest
        {
            Sku = "SKU-001",
            Price = 100,
            Stock = 10,
            AttributeValueIds = new List<Guid>(),
            Images = new List<CreateProductVariantImageRequest>()
        };

        var product = new Product { Id = productId };

        _mockProductRepo.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);
        _mockVariantRepo.Setup(r => r.IsSkuUniqueAsync(request.Sku))
            .ReturnsAsync(true);
        _mockVariantRepo.Setup(r => r.GetByProductIdAsync(productId))
            .ReturnsAsync(new List<ProductVariant>());
        _mockVariantRepo.Setup(r => r.AddAsync(It.IsAny<ProductVariant>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(productId, request);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _mockVariantRepo.Verify(r => r.AddAsync(It.IsAny<ProductVariant>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CreateAsync_WithInvalidProductId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = new CreateProductVariantRequest { Sku = "SKU-001", Price = 100, Stock = 10 };

        _mockProductRepo.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateAsync(productId, request));
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateSku_ThrowsInvalidOperationException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = new CreateProductVariantRequest { Sku = "SKU-001", Price = 100, Stock = 10 };
        var product = new Product { Id = productId };

        _mockProductRepo.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);
        _mockVariantRepo.Setup(r => r.IsSkuUniqueAsync(request.Sku))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(productId, request));
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateAttributeCombination_ThrowsInvalidOperationException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var attrValueId = Guid.NewGuid();
        var request = new CreateProductVariantRequest
        {
            Sku = "SKU-001",
            Price = 100,
            Stock = 10,
            AttributeValueIds = new List<Guid> { attrValueId }
        };

        var product = new Product { Id = productId };
        var existingVariant = new ProductVariant
        {
            Id = Guid.NewGuid(),
            Attributes = new List<ProductVariantAttribute>
            {
                new() { ProductAttributeValueId = attrValueId }
            }
        };

        _mockProductRepo.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);
        _mockVariantRepo.Setup(r => r.IsSkuUniqueAsync(request.Sku))
            .ReturnsAsync(true);
        _mockVariantRepo.Setup(r => r.GetByProductIdAsync(productId))
            .ReturnsAsync(new List<ProductVariant> { existingVariant });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(productId, request));
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidRequest_UpdatesVariant()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingVariant = new ProductVariant
        {
            Id = id,
            Sku = "SKU-001",
            Price = 100,
            Stock = 10
        };

        var request = new UpdateProductVariantRequest
        {
            Sku = "SKU-002",
            Price = 150,
            Stock = 20,
            Images = new List<UpdateProductVariantImageRequest>()
        };

        _mockVariantRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(existingVariant);
        _mockVariantRepo.Setup(r => r.IsSkuUniqueAsync(request.Sku, id))
            .ReturnsAsync(true);
        _mockImageRepo.Setup(r => r.GetByVariantIdAsync(id))
            .ReturnsAsync(new List<ProductVariantImage>());
        _mockVariantRepo.Setup(r => r.UpdateAsync(It.IsAny<ProductVariant>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(id, request);

        // Assert
        Assert.Equal("SKU-002", existingVariant.Sku);
        Assert.Equal(150, existingVariant.Price);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateProductVariantRequest { Sku = "SKU-001", Price = 100, Stock = 10 };

        _mockVariantRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((ProductVariant?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(id, request));
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateSku_ThrowsInvalidOperationException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingVariant = new ProductVariant { Id = id, Sku = "SKU-001" };
        var request = new UpdateProductVariantRequest { Sku = "SKU-002", Price = 100, Stock = 10 };

        _mockVariantRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(existingVariant);
        _mockVariantRepo.Setup(r => r.IsSkuUniqueAsync(request.Sku, id))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(id, request));
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesVariant()
    {
        // Arrange
        var id = Guid.NewGuid();
        var variant = new ProductVariant { Id = id };

        _mockVariantRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(variant);
        _mockVariantRepo.Setup(r => r.DeleteAsync(It.IsAny<ProductVariant>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(id);

        // Assert
        _mockVariantRepo.Verify(r => r.DeleteAsync(It.IsAny<ProductVariant>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockVariantRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((ProductVariant?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(id));
    }

    #endregion

    #region ValidateVariantCombinationAsync Tests

    [Fact]
    public async Task ValidateVariantCombinationAsync_WithUniqueCombo_ReturnsTrue()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var attrValues = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var existingVariants = new List<ProductVariant>
        {
            new()
            {
                Attributes = new List<ProductVariantAttribute>
                {
                    new() { ProductAttributeValueId = Guid.NewGuid() }
                }
            }
        };

        _mockVariantRepo.Setup(r => r.GetByProductIdAsync(productId))
            .ReturnsAsync(existingVariants);

        // Act
        var result = await _service.ValidateVariantCombinationAsync(productId, attrValues);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateVariantCombinationAsync_WithDuplicateCombo_ReturnsFalse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var attrValue = Guid.NewGuid();
        var attrValues = new List<Guid> { attrValue };
        var existingVariants = new List<ProductVariant>
        {
            new()
            {
                Attributes = new List<ProductVariantAttribute>
                {
                    new() { ProductAttributeValueId = attrValue }
                }
            }
        };

        _mockVariantRepo.Setup(r => r.GetByProductIdAsync(productId))
            .ReturnsAsync(existingVariants);

        // Act
        var result = await _service.ValidateVariantCombinationAsync(productId, attrValues);

        // Assert
        Assert.False(result);
    }

    #endregion
}
