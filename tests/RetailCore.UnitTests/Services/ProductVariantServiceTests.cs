using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.UnitTests.Services;

public class ProductVariantServiceTests
{
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<IProductVariantRepository> _variantRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    private readonly ProductVariantService _variantService;

    public ProductVariantServiceTests()
    {
        _productRepoMock = new Mock<IProductRepository>();
        _variantRepoMock = new Mock<IProductVariantRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _variantService = new ProductVariantService(
            _productRepoMock.Object,
            _variantRepoMock.Object,
            _unitOfWorkMock.Object);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenVariantExists_ShouldReturnVariantDto()
    {
        // Arrange
        var variant = CreateVariant();

        _variantRepoMock
            .Setup(repo => repo.GetByIdAsync(variant.Id))
            .ReturnsAsync(variant);

        // Act
        var result = await _variantService.GetByIdAsync(variant.Id);

        // Assert
        Assert.Equal(variant.Id, result.Id);
        Assert.Equal(variant.Sku, result.Sku);
        Assert.Equal(variant.Name, result.Name);
        Assert.Equal(variant.Description, result.Description);
        Assert.Equal(variant.Price, result.Price);
        Assert.Equal(variant.CompareAtPrice, result.CompareAtPrice);
        Assert.Equal(variant.Stock, result.Stock);
        Assert.Equal(variant.Status, result.Status);

        Assert.Equal(2, result.Images.Count);
        Assert.Equal(1, result.Images[0].SortOrder);
        Assert.Equal(2, result.Images[1].SortOrder);

        Assert.Equal(2, result.Attributes.Count);
        Assert.Equal("Color", result.Attributes[0].AttributeName);
        Assert.Equal("Black", result.Attributes[0].AttributeValue);
        Assert.Equal("Color", result.Attributes[1].AttributeName);
        Assert.Equal("Black", result.Attributes[1].AttributeValue);
    }

    [Fact]
    public async Task GetByIdAsync_WhenVariantDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var variantId = Guid.NewGuid();

        _variantRepoMock
            .Setup(repo => repo.GetByIdAsync(variantId))
            .ReturnsAsync((ProductVariant?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _variantService.GetByIdAsync(variantId));

        // Assert
        Assert.Equal($"Variant id '{variantId}' not found.", exception.Message);
    }

    [Fact]
    public async Task GetByProductIdAsync_ShouldReturnVariantDtos()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var variants = new List<ProductVariant>
        {
            CreateVariant(productId: productId),
            CreateVariant(productId: productId, sku: "SKU-2")
        };

        _variantRepoMock
            .Setup(repo => repo.GetByProductIdAsync(productId))
            .ReturnsAsync(variants);

        // Act
        var result = await _variantService.GetByProductIdAsync(productId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("SKU-1", result[0].Sku);
        Assert.Equal("SKU-2", result[1].Sku);
    }

    [Fact]
    public async Task CreateAsync_WhenProductDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var request = CreateVariantRequest();

        _productRepoMock
            .Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _variantService.CreateAsync(productId, request));

        // Assert
        Assert.Equal($"Product with ID {productId} not found.", exception.Message);

        _variantRepoMock.Verify(repo => repo.AddAsync(It.IsAny<ProductVariant>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenSkuAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var request = CreateVariantRequest();

        _productRepoMock
            .Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(new Product());

        _variantRepoMock
            .Setup(repo => repo.IsSkuUniqueAsync(request.Sku, null))
            .ReturnsAsync(false);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _variantService.CreateAsync(productId, request));

        // Assert
        Assert.Equal($"SKU '{request.Sku}' already exists.", exception.Message);

        _variantRepoMock.Verify(repo => repo.AddAsync(It.IsAny<ProductVariant>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenCombinationAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var request = CreateVariantRequest();

        var existingVariant = CreateVariant(
            productId: productId,
            attributeValueIds: request.AttributeValueIds);

        _productRepoMock
            .Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(new Product());

        _variantRepoMock
            .Setup(repo => repo.IsSkuUniqueAsync(request.Sku, null))
            .ReturnsAsync(true);

        _variantRepoMock
            .Setup(repo => repo.GetByProductIdAsync(productId))
            .ReturnsAsync([existingVariant]);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _variantService.CreateAsync(productId, request));

        // Assert
        Assert.Equal("Variant combination already exists.", exception.Message);

        _variantRepoMock.Verify(repo => repo.AddAsync(It.IsAny<ProductVariant>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenRequestIsValid_ShouldCreateVariantAndReturnId()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var request = CreateVariantRequest();

        ProductVariant? createdVariant = null;

        _productRepoMock
            .Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(new Product());

        _variantRepoMock
            .Setup(repo => repo.IsSkuUniqueAsync(request.Sku, null))
            .ReturnsAsync(true);

        _variantRepoMock
            .Setup(repo => repo.GetByProductIdAsync(productId))
            .ReturnsAsync([]);

        _variantRepoMock
            .Setup(repo => repo.AddAsync(It.IsAny<ProductVariant>()))
            .Callback<ProductVariant>(variant => createdVariant = variant)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _variantService.CreateAsync(productId, request);

        // Assert
        Assert.NotEqual(Guid.Empty, result);

        Assert.NotNull(createdVariant);

        Assert.Equal(result, createdVariant!.Id);
        Assert.Equal(productId, createdVariant.ProductId);
        Assert.Equal(request.Sku, createdVariant.Sku);
        Assert.Equal(request.Name, createdVariant.Name);
        Assert.Equal(request.Description, createdVariant.Description);
        Assert.Equal(request.Price, createdVariant.Price);
        Assert.Equal(request.CompareAtPrice, createdVariant.CompareAtPrice);
        Assert.Equal(request.Stock, createdVariant.Stock);
        Assert.Equal(request.Status, createdVariant.Status);

        Assert.Equal(2, createdVariant.Images.Count);
        Assert.Equal(2, createdVariant.Attributes.Count);

        _variantRepoMock.Verify(repo => repo.AddAsync(It.IsAny<ProductVariant>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenVariantDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var variantId = Guid.NewGuid();

        var request = UpdateVariantRequest();

        _variantRepoMock
            .Setup(repo => repo.GetTrackedByIdAsync(variantId))
            .ReturnsAsync((ProductVariant?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _variantService.UpdateAsync(variantId, request));

        // Assert
        Assert.Equal($"Variant id '{variantId}' not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenSkuChangedToExistingSku_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var variant = CreateVariant(sku: "OLD-SKU");

        var request = UpdateVariantRequest(sku: "NEW-SKU");

        _variantRepoMock
            .Setup(repo => repo.GetTrackedByIdAsync(variant.Id))
            .ReturnsAsync(variant);

        _variantRepoMock
            .Setup(repo => repo.IsSkuUniqueAsync(request.Sku, variant.Id))
            .ReturnsAsync(false);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _variantService.UpdateAsync(variant.Id, request));

        // Assert
        Assert.Equal($"SKU '{request.Sku}' already exists.", exception.Message);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenSkuUnchanged_ShouldUpdateVariantWithoutCheckingSkuUniqueness()
    {
        // Arrange
        var variant = CreateVariant(sku: "SKU-1");

        var request = UpdateVariantRequest(sku: "SKU-1");

        _variantRepoMock
            .Setup(repo => repo.GetTrackedByIdAsync(variant.Id))
            .ReturnsAsync(variant);

        // Act
        await _variantService.UpdateAsync(variant.Id, request);

        // Assert
        Assert.Equal(request.Name, variant.Name);
        Assert.Equal(request.Description, variant.Description);
        Assert.Equal(request.Price, variant.Price);
        Assert.Equal(request.CompareAtPrice, variant.CompareAtPrice);
        Assert.Equal(request.Stock, variant.Stock);
        Assert.Equal(request.Status, variant.Status);

        _variantRepoMock.Verify(
            repo => repo.IsSkuUniqueAsync(It.IsAny<string>(), It.IsAny<Guid?>()),
            Times.Never);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenSkuChangedToUniqueSku_ShouldUpdateVariant()
    {
        // Arrange
        var variant = CreateVariant(sku: "OLD-SKU");

        var request = UpdateVariantRequest(sku: "NEW-SKU");

        _variantRepoMock
            .Setup(repo => repo.GetTrackedByIdAsync(variant.Id))
            .ReturnsAsync(variant);

        _variantRepoMock
            .Setup(repo => repo.IsSkuUniqueAsync(request.Sku, variant.Id))
            .ReturnsAsync(true);

        // Act
        await _variantService.UpdateAsync(variant.Id, request);

        // Assert
        Assert.Equal("NEW-SKU", variant.Sku);

        _variantRepoMock.Verify(
            repo => repo.IsSkuUniqueAsync(request.Sku, variant.Id),
            Times.Once);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenImagesProvided_ShouldReplaceImages()
    {
        // Arrange
        var variant = CreateVariant();

        var request = UpdateVariantRequest();

        _variantRepoMock
            .Setup(repo => repo.GetTrackedByIdAsync(variant.Id))
            .ReturnsAsync(variant);

        // Act
        await _variantService.UpdateAsync(variant.Id, request);

        // Assert
        Assert.Equal(2, variant.Images.Count);

        Assert.Equal("updated-image-1.jpg", variant.Images.ToList()[0].Url);
        Assert.Equal("updated-image-2.jpg", variant.Images.ToList()[1].Url);

        _variantRepoMock.Verify(
            repo => repo.RemoveImages(It.IsAny<IEnumerable<ProductVariantImage>>()),
            Times.Once);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenImagesNull_ShouldNotReplaceImages()
    {
        // Arrange
        var variant = CreateVariant();

        var request = UpdateVariantRequest();
        request.Images = null;

        _variantRepoMock
            .Setup(repo => repo.GetTrackedByIdAsync(variant.Id))
            .ReturnsAsync(variant);

        // Act
        await _variantService.UpdateAsync(variant.Id, request);

        // Assert
        Assert.Equal(2, variant.Images.Count);

        _variantRepoMock.Verify(
            repo => repo.RemoveImages(It.IsAny<IEnumerable<ProductVariantImage>>()),
            Times.Never);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReplaceAttributes()
    {
        // Arrange
        var variant = CreateVariant();

        var request = UpdateVariantRequest();

        _variantRepoMock
            .Setup(repo => repo.GetTrackedByIdAsync(variant.Id))
            .ReturnsAsync(variant);

        // Act
        await _variantService.UpdateAsync(variant.Id, request);

        // Assert
        Assert.Equal(2, variant.Attributes.Count);

        _variantRepoMock.Verify(
            repo => repo.RemoveAttributes(It.IsAny<IEnumerable<ProductVariantAttribute>>()),
            Times.Once);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteDraftAsync_WhenVariantExists_ShouldDeleteVariant()
    {
        // Arrange
        var variant = CreateVariant();

        _variantRepoMock
            .Setup(repo => repo.GetTrackedByIdAsync(variant.Id))
            .ReturnsAsync(variant);

        // Act
        await _variantService.DeleteDraftAsync(variant.Id);

        // Assert
        _variantRepoMock.Verify(repo => repo.Delete(variant), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteDraftAsync_WhenVariantDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var variantId = Guid.NewGuid();

        _variantRepoMock
            .Setup(repo => repo.GetTrackedByIdAsync(variantId))
            .ReturnsAsync((ProductVariant?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _variantService.DeleteDraftAsync(variantId));

        // Assert
        Assert.Equal($"Variant id '{variantId}' not found.", exception.Message);

        _variantRepoMock.Verify(repo => repo.Delete(It.IsAny<ProductVariant>()), Times.Never);
    }

    [Fact]
    public async Task RemoveAllVariantsAsync_ShouldDeleteAllVariants()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var variants = new List<ProductVariant>
        {
            CreateVariant(productId: productId),
            CreateVariant(productId: productId, sku: "SKU-2")
        };

        _variantRepoMock
            .Setup(repo => repo.GetTrackedByProductIdAsync(productId))
            .ReturnsAsync(variants);

        // Act
        await _variantService.RemoveAllVariantsAsync(productId);

        // Assert
        _variantRepoMock.Verify(repo => repo.DeleteRange(variants), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task IsCombinationUniqueAsync_WhenCombinationExists_ShouldReturnFalse()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var attributeValueIds = new List<Guid>
        {
            Guid.NewGuid(),
            Guid.NewGuid()
        };

        var variants = new List<ProductVariant>
        {
            CreateVariant(productId: productId, attributeValueIds: attributeValueIds)
        };

        _variantRepoMock
            .Setup(repo => repo.GetByProductIdAsync(productId))
            .ReturnsAsync(variants);

        // Act
        var result = await _variantService.IsCombinationUniqueAsync(
            productId,
            attributeValueIds);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsCombinationUniqueAsync_WhenCombinationDoesNotExist_ShouldReturnTrue()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var existingIds = new List<Guid>
        {
            Guid.NewGuid(),
            Guid.NewGuid()
        };

        var newIds = new List<Guid>
        {
            Guid.NewGuid(),
            Guid.NewGuid()
        };

        var variants = new List<ProductVariant>
        {
            CreateVariant(productId: productId, attributeValueIds: existingIds)
        };

        _variantRepoMock
            .Setup(repo => repo.GetByProductIdAsync(productId))
            .ReturnsAsync(variants);

        // Act
        var result = await _variantService.IsCombinationUniqueAsync(
            productId,
            newIds);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void MapToProductVariantDto_ShouldMapVariantToDto()
    {
        // Arrange
        var variant = CreateVariant();

        // Act
        var result = ProductVariantService.MapToProductVariantDto(variant);

        // Assert
        Assert.Equal(variant.Id, result.Id);
        Assert.Equal(variant.Sku, result.Sku);
        Assert.Equal(variant.Name, result.Name);
        Assert.Equal(variant.Description, result.Description);
        Assert.Equal(variant.Price, result.Price);
        Assert.Equal(variant.CompareAtPrice, result.CompareAtPrice);
        Assert.Equal(variant.Stock, result.Stock);
        Assert.Equal(variant.Status, result.Status);

        Assert.Equal(2, result.Images.Count);

        Assert.Equal("image-1.jpg", result.Images[0].Url);
        Assert.Equal(1, result.Images[0].SortOrder);

        Assert.Equal("image-2.jpg", result.Images[1].Url);
        Assert.Equal(2, result.Images[1].SortOrder);

        Assert.Equal(2, result.Attributes.Count);

        Assert.Equal("Color", result.Attributes[0].AttributeName);
        Assert.Equal("Black", result.Attributes[0].AttributeValue);

        Assert.Equal("Color", result.Attributes[1].AttributeName);
        Assert.Equal("Black", result.Attributes[1].AttributeValue);
    }

    private static ProductVariant CreateVariant(
        Guid? productId = null,
        string sku = "SKU-1",
        List<Guid>? attributeValueIds = null)
    {
        productId ??= Guid.NewGuid();

        attributeValueIds ??=
        [
            Guid.NewGuid(),
            Guid.NewGuid()
        ];

        var attributeId = Guid.NewGuid();

        return new ProductVariant
        {
            Id = Guid.NewGuid(),
            ProductId = productId.Value,
            Sku = sku,
            Name = "Variant Name",
            Description = "Variant Description",
            Price = 999,
            CompareAtPrice = 1099,
            Stock = 10,
            Status = ProductVariantStatus.Active,
            CreatedAt = DateTime.UtcNow,

            Images =
            [
                new ProductVariantImage
                {
                    Id = Guid.NewGuid(),
                    Url = "image-2.jpg",
                    SortOrder = 2,
                    IsPrimary = false
                },
                new ProductVariantImage
                {
                    Id = Guid.NewGuid(),
                    Url = "image-1.jpg",
                    SortOrder = 1,
                    IsPrimary = true
                }
            ],

            Attributes = attributeValueIds
                .Select(id => new ProductVariantAttribute
                {
                    ProductAttributeValueId = id,

                    ProductAttributeValue = new ProductAttributeValue
                    {
                        Id = id,
                        Value = "Black",

                        ProductAttributeId = attributeId,

                        ProductAttribute = new ProductAttribute
                        {
                            Id = attributeId,
                            Name = "Color"
                        }
                    }
                })
                .ToList()
        };
    }

    private static CreateProductVariantRequest CreateVariantRequest()
    {
        return new CreateProductVariantRequest
        {
            Sku = "SKU-1",
            Name = "Variant Name",
            Description = "Variant Description",
            Price = 999,
            CompareAtPrice = 1099,
            Stock = 10,
            Status = ProductVariantStatus.Active,

            Images =
            [
                new ProductVariantImageRequest
                {
                    Url = "image-1.jpg",
                    SortOrder = 1,
                    IsPrimary = true
                },
                new ProductVariantImageRequest
                {
                    Url = "image-2.jpg",
                    SortOrder = 2,
                    IsPrimary = false
                }
            ],

            AttributeValueIds =
            [
                Guid.NewGuid(),
                Guid.NewGuid()
            ]
        };
    }

    private static UpdateProductVariantRequest UpdateVariantRequest(
        string sku = "SKU-1")
    {
        return new UpdateProductVariantRequest
        {
            Sku = sku,
            Name = "Updated Variant",
            Description = "Updated Description",
            Price = 1999,
            CompareAtPrice = 2099,
            Stock = 20,
            Status = ProductVariantStatus.Inactive,

            Images =
            [
                new ProductVariantImageRequest
                {
                    Url = "updated-image-1.jpg",
                    SortOrder = 1,
                    IsPrimary = true
                },
                new ProductVariantImageRequest
                {
                    Url = "updated-image-2.jpg",
                    SortOrder = 2,
                    IsPrimary = false
                }
            ],

            AttributeValueIds =
            [
                Guid.NewGuid(),
                Guid.NewGuid()
            ]
        };
    }
}
