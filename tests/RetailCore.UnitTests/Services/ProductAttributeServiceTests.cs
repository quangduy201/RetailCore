using Moq;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Services.Implementations;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.UnitTests.Services;

public class ProductAttributeServiceTests
{
    private readonly Mock<IProductAttributeRepository> _mockAttributeRepo;
    private readonly Mock<IProductAttributeValueRepository> _mockValueRepo;
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly ProductAttributeService _service;

    public ProductAttributeServiceTests()
    {
        _mockAttributeRepo = new Mock<IProductAttributeRepository>();
        _mockValueRepo = new Mock<IProductAttributeValueRepository>();
        _mockProductRepo = new Mock<IProductRepository>();
        _service = new ProductAttributeService(_mockAttributeRepo.Object, _mockValueRepo.Object, _mockProductRepo.Object);
    }

    #region GetByProductIdAsync Tests

    [Fact]
    public async Task GetByProductIdAsync_WithValidProductId_ReturnsAttributes()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var attributes = new List<ProductAttribute>
        {
            new() { Id = Guid.NewGuid(), Name = "Color", Values = new List<ProductAttributeValue>() },
            new() { Id = Guid.NewGuid(), Name = "Size", Values = new List<ProductAttributeValue>() }
        };

        _mockAttributeRepo.Setup(r => r.GetByProductIdAsync(productId))
            .ReturnsAsync(attributes);

        // Act
        var result = await _service.GetByProductIdAsync(productId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Color", result[0].Name);
    }

    [Fact]
    public async Task GetByProductIdAsync_WithNoAttributes_ReturnsEmptyList()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mockAttributeRepo.Setup(r => r.GetByProductIdAsync(productId))
            .ReturnsAsync(new List<ProductAttribute>());

        // Act
        var result = await _service.GetByProductIdAsync(productId);

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsAttribute()
    {
        // Arrange
        var id = Guid.NewGuid();
        var attribute = new ProductAttribute
        {
            Id = id,
            Name = "Color",
            Values = new List<ProductAttributeValue>()
        };

        _mockAttributeRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(attribute);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Color", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockAttributeRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((ProductAttribute?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(id));
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidRequest_ReturnsAttributeId()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = new CreateProductAttributeRequest { Name = "Color" };
        var product = new Product { Id = productId };

        _mockProductRepo.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);
        _mockAttributeRepo.Setup(r => r.IsAttributeNameUniqueAsync(productId, request.Name))
            .ReturnsAsync(true);
        _mockAttributeRepo.Setup(r => r.AddAsync(It.IsAny<ProductAttribute>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(productId, request);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _mockAttributeRepo.Verify(r => r.AddAsync(It.IsAny<ProductAttribute>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidProductId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = new CreateProductAttributeRequest { Name = "Color" };

        _mockProductRepo.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateAsync(productId, request));
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateAttributeName_ThrowsInvalidOperationException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = new CreateProductAttributeRequest { Name = "Color" };
        var product = new Product { Id = productId };

        _mockProductRepo.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);
        _mockAttributeRepo.Setup(r => r.IsAttributeNameUniqueAsync(productId, request.Name))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(productId, request));
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidRequest_UpdatesAttribute()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingAttribute = new ProductAttribute { Id = id, ProductId = Guid.NewGuid(), Name = "Color" };
        var request = new UpdateProductAttributeRequest { Name = "Material" };

        _mockAttributeRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(existingAttribute);
        _mockAttributeRepo.Setup(r => r.IsAttributeNameUniqueAsync(existingAttribute.ProductId, request.Name, id))
            .ReturnsAsync(true);
        _mockAttributeRepo.Setup(r => r.UpdateAsync(It.IsAny<ProductAttribute>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(id, request);

        // Assert
        Assert.Equal("Material", existingAttribute.Name);
        _mockAttributeRepo.Verify(r => r.UpdateAsync(It.IsAny<ProductAttribute>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateProductAttributeRequest { Name = "Material" };

        _mockAttributeRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((ProductAttribute?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(id, request));
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateName_ThrowsInvalidOperationException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var existingAttribute = new ProductAttribute { Id = id, ProductId = productId, Name = "Color" };
        var request = new UpdateProductAttributeRequest { Name = "Material" };

        _mockAttributeRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(existingAttribute);
        _mockAttributeRepo.Setup(r => r.IsAttributeNameUniqueAsync(productId, request.Name, id))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(id, request));
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesAttribute()
    {
        // Arrange
        var id = Guid.NewGuid();
        var attribute = new ProductAttribute { Id = id };

        _mockAttributeRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(attribute);
        _mockAttributeRepo.Setup(r => r.DeleteAsync(It.IsAny<ProductAttribute>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(id);

        // Assert
        _mockAttributeRepo.Verify(r => r.DeleteAsync(It.IsAny<ProductAttribute>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockAttributeRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((ProductAttribute?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(id));
    }

    #endregion

    #region GetValuesByAttributeIdAsync Tests

    [Fact]
    public async Task GetValuesByAttributeIdAsync_WithValidAttributeId_ReturnsValues()
    {
        // Arrange
        var attributeId = Guid.NewGuid();
        var values = new List<ProductAttributeValue>
        {
            new() { Id = Guid.NewGuid(), Value = "Red" },
            new() { Id = Guid.NewGuid(), Value = "Blue" }
        };

        _mockValueRepo.Setup(r => r.GetByAttributeIdAsync(attributeId))
            .ReturnsAsync(values);

        // Act
        var result = await _service.GetValuesByAttributeIdAsync(attributeId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Red", result[0].Value);
    }

    [Fact]
    public async Task GetValuesByAttributeIdAsync_WithNoValues_ReturnsEmptyList()
    {
        // Arrange
        var attributeId = Guid.NewGuid();
        _mockValueRepo.Setup(r => r.GetByAttributeIdAsync(attributeId))
            .ReturnsAsync(new List<ProductAttributeValue>());

        // Act
        var result = await _service.GetValuesByAttributeIdAsync(attributeId);

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region GetValueByIdAsync Tests

    [Fact]
    public async Task GetValueByIdAsync_WithValidId_ReturnsValue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var value = new ProductAttributeValue { Id = id, Value = "Red" };

        _mockValueRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(value);

        // Act
        var result = await _service.GetValueByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Red", result.Value);
    }

    [Fact]
    public async Task GetValueByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockValueRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((ProductAttributeValue?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetValueByIdAsync(id));
    }

    #endregion

    #region CreateValueAsync Tests

    [Fact]
    public async Task CreateValueAsync_WithValidRequest_ReturnsValueId()
    {
        // Arrange
        var attributeId = Guid.NewGuid();
        var request = new CreateProductAttributeValueRequest { Value = "Red" };

        _mockValueRepo.Setup(r => r.AddAsync(It.IsAny<ProductAttributeValue>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateValueAsync(attributeId, request);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _mockValueRepo.Verify(r => r.AddAsync(It.IsAny<ProductAttributeValue>()), Times.Once);
    }

    #endregion

    #region UpdateValueAsync Tests

    [Fact]
    public async Task UpdateValueAsync_WithValidRequest_UpdatesValue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingValue = new ProductAttributeValue { Id = id, Value = "Red" };
        var request = new UpdateProductAttributeValueRequest { Value = "Blue" };

        _mockValueRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(existingValue);
        _mockValueRepo.Setup(r => r.UpdateAsync(It.IsAny<ProductAttributeValue>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateValueAsync(id, request);

        // Assert
        Assert.Equal("Blue", existingValue.Value);
        _mockValueRepo.Verify(r => r.UpdateAsync(It.IsAny<ProductAttributeValue>()), Times.Once);
    }

    [Fact]
    public async Task UpdateValueAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateProductAttributeValueRequest { Value = "Blue" };

        _mockValueRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((ProductAttributeValue?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateValueAsync(id, request));
    }

    #endregion

    #region DeleteValueAsync Tests

    [Fact]
    public async Task DeleteValueAsync_WithValidId_DeletesValue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var value = new ProductAttributeValue { Id = id };

        _mockValueRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(value);
        _mockValueRepo.Setup(r => r.DeleteAsync(It.IsAny<ProductAttributeValue>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteValueAsync(id);

        // Assert
        _mockValueRepo.Verify(r => r.DeleteAsync(It.IsAny<ProductAttributeValue>()), Times.Once);
    }

    [Fact]
    public async Task DeleteValueAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockValueRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((ProductAttributeValue?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteValueAsync(id));
    }

    #endregion
}
