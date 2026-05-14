using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.UnitTests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<IProductVariantRepository> _variantRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepoMock = new Mock<IProductRepository>();
        _variantRepoMock = new Mock<IProductVariantRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _productService = new ProductService(
            _productRepoMock.Object,
            _variantRepoMock.Object,
            _unitOfWorkMock.Object);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedProductSummaryDtos()
    {
        // Arrange
        var request = new GetProductsRequest
        {
            Keyword = "iphone",
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Brand = "",
            Category = "",
            Status = ProductStatus.Active,
            PageNumber = 1,
            PageSize = 10
        };

        var products = new List<Product>
        {
            CreateProduct(name: "iPhone 15", slug: "iphone-15")
        };

        _productRepoMock
            .Setup(repo => repo.GetPagedAsync(
                request.Keyword,
                request.BrandId,
                request.CategoryId,
                request.Brand,
                request.Category,
                request.Status,
                request.PageNumber,
                request.PageSize))
            .ReturnsAsync((products, products.Count));

        // Act
        var result = await _productService.GetPagedAsync(request);

        // Assert
        Assert.Equal(request.PageNumber, result.PageNumber);
        Assert.Equal(request.PageSize, result.PageSize);
        Assert.Equal(products.Count, result.TotalCount);

        var item = Assert.Single(result.Items);
        Assert.Equal(products[0].Id, item.Id);
        Assert.Equal("iPhone 15", item.Name);
        Assert.Equal("iphone-15", item.Slug);
        Assert.Equal("Apple", item.BrandName);
        Assert.Equal("Phones", item.CategoryName);
        Assert.Equal(999, item.Price);
        Assert.Equal("image-1.jpg", item.ThumbnailUrl);
        Assert.False(item.IsOutOfStock);
    }

    [Fact]
    public async Task GetPagedForManagementAsync_ShouldReturnPagedProductManagementDtos()
    {
        // Arrange
        var request = new GetProductsRequest
        {
            Keyword = "iphone",
            Status = ProductStatus.Active,
            PageNumber = 1,
            PageSize = 10
        };

        var products = new List<Product>
        {
            CreateProduct(name: "iPhone 15", slug: "iphone-15")
        };

        _productRepoMock
            .Setup(repo => repo.GetPagedForManagementAsync(
                request.Keyword,
                request.Status,
                request.PageNumber,
                request.PageSize))
            .ReturnsAsync((products, products.Count));

        // Act
        var result = await _productService.GetPagedForManagementAsync(request);

        // Assert
        Assert.Equal(request.PageNumber, result.PageNumber);
        Assert.Equal(request.PageSize, result.PageSize);
        Assert.Equal(products.Count, result.TotalCount);

        var item = Assert.Single(result.Items);
        Assert.Equal(products[0].Id, item.Id);
        Assert.Equal("iPhone 15", item.Name);
        Assert.Equal("iphone-15", item.Slug);
        Assert.Equal(ProductStatus.Draft, item.Status);
        Assert.Equal(1, item.VariantCount);
        Assert.Equal(10, item.Stock);
        Assert.Equal(999, item.MinPrice);
        Assert.Equal(999, item.MaxPrice);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ShouldReturnProductDetailDto()
    {
        // Arrange
        var product = CreateProduct();
        var productId = product.Id;

        _productRepoMock
            .Setup(repo => repo.GetByIdWithDetailsAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal(product.Slug, result.Slug);
        Assert.Equal(product.Brand.Name, result.BrandName);
        Assert.Equal(product.Category.Name, result.CategoryName);
        Assert.Single(result.Attributes);
        Assert.Single(result.Variants);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _productRepoMock
            .Setup(repo => repo.GetByIdWithDetailsAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _productService.GetByIdAsync(productId));

        // Assert
        Assert.Equal($"Product id '{productId}' not found.", exception.Message);
    }

    [Fact]
    public async Task GetBySlugAsync_WhenProductExists_ShouldReturnProductDetailDto()
    {
        // Arrange
        var product = CreateProduct(slug: "iphone-15");

        _productRepoMock
            .Setup(repo => repo.GetBySlugWithDetailsAsync(product.Slug))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetBySlugAsync(product.Slug);

        // Assert
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal("iphone-15", result.Slug);
    }

    [Fact]
    public async Task GetBySlugAsync_WhenProductDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var slug = "unknown-product";

        _productRepoMock
            .Setup(repo => repo.GetBySlugWithDetailsAsync(slug))
            .ReturnsAsync((Product?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _productService.GetBySlugAsync(slug));

        // Assert
        Assert.Equal($"Product slug '{slug}' not found.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenSlugIsUnique_ShouldCreateProductAndReturnId()
    {
        // Arrange
        var request = CreateProductRequest();

        Product? createdProduct = null;

        _productRepoMock
            .Setup(repo => repo.IsSlugUniqueAsync(request.Slug, null))
            .ReturnsAsync(true);

        _productRepoMock
            .Setup(repo => repo.AddAsync(It.IsAny<Product>()))
            .Callback<Product>(product => createdProduct = product)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _productService.CreateAsync(request);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        Assert.NotNull(createdProduct);

        Assert.Equal(result, createdProduct!.Id);
        Assert.Equal(request.Name, createdProduct.Name);
        Assert.Equal(request.Slug, createdProduct.Slug);
        Assert.Equal(request.ShortDescription, createdProduct.ShortDescription);
        Assert.Equal(request.Description, createdProduct.Description);
        Assert.Equal(request.BrandId, createdProduct.BrandId);
        Assert.Equal(request.CategoryId, createdProduct.CategoryId);
        Assert.Equal(ProductStatus.Draft, createdProduct.Status);
        Assert.NotEqual(default, createdProduct.CreatedAt);
        Assert.Single(createdProduct.Attributes);

        _productRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenSlugAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = CreateProductRequest();

        _productRepoMock
            .Setup(repo => repo.IsSlugUniqueAsync(request.Slug, null))
            .ReturnsAsync(false);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _productService.CreateAsync(request));

        // Assert
        Assert.Equal($"Product slug '{request.Slug}' already exists.", exception.Message);

        _productRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = CreateUpdateProductRequest();

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _productService.UpdateAsync(productId, request));

        // Assert
        Assert.Equal($"Product id '{productId}' not found.", exception.Message);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenSlugChangedToExistingSlug_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var product = CreateProduct(slug: "old-slug");
        var request = CreateUpdateProductRequest(slug: "new-slug");

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(product.Id))
            .ReturnsAsync(product);

        _productRepoMock
            .Setup(repo => repo.IsSlugUniqueAsync(request.Slug, product.Id))
            .ReturnsAsync(false);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _productService.UpdateAsync(product.Id, request));

        // Assert
        Assert.Equal($"Product slug '{request.Slug}' already exists.", exception.Message);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenSlugUnchangedAndAttributesUnchanged_ShouldUpdateProductWithoutReplacingAttributesOrVariants()
    {
        // Arrange
        var product = CreateProduct(slug: "iphone-15");
        var request = CreateUpdateProductRequest(slug: "iphone-15");

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(product.Id))
            .ReturnsAsync(product);

        // Act
        await _productService.UpdateAsync(product.Id, request);

        // Assert
        Assert.Equal(request.Name, product.Name);
        Assert.Equal(request.Slug, product.Slug);
        Assert.Equal(request.ShortDescription, product.ShortDescription);
        Assert.Equal(request.Description, product.Description);
        Assert.Equal(request.BrandId, product.BrandId);
        Assert.Equal(request.CategoryId, product.CategoryId);
        Assert.NotNull(product.UpdatedAt);

        _productRepoMock.Verify(repo => repo.IsSlugUniqueAsync(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Never);
        _variantRepoMock.Verify(repo => repo.DeleteRange(It.IsAny<List<ProductVariant>>()), Times.Never);
        _productRepoMock.Verify(repo => repo.RemoveAttributes(It.IsAny<IEnumerable<ProductAttribute>>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenSlugChangedToUniqueSlug_ShouldUpdateProduct()
    {
        // Arrange
        var product = CreateProduct(slug: "old-slug");
        var request = CreateUpdateProductRequest(slug: "new-slug");

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(product.Id))
            .ReturnsAsync(product);

        _productRepoMock
            .Setup(repo => repo.IsSlugUniqueAsync(request.Slug, product.Id))
            .ReturnsAsync(true);

        // Act
        await _productService.UpdateAsync(product.Id, request);

        // Assert
        Assert.Equal("new-slug", product.Slug);

        _productRepoMock.Verify(repo => repo.IsSlugUniqueAsync(request.Slug, product.Id), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenAttributesChangedAndProductHasVariants_ShouldDeleteVariantsAndReplaceAttributes()
    {
        // Arrange
        var product = CreateProduct();
        var request = CreateUpdateProductRequest();

        request.Attributes =
        [
            new ProductAttributeRequest
            {
                Name = "Storage",
                Values =
                [
                    new ProductAttributeValueRequest { Value = "128GB" },
                    new ProductAttributeValueRequest { Value = "256GB" }
                ]
            }
        ];

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(product.Id))
            .ReturnsAsync(product);

        // Act
        await _productService.UpdateAsync(product.Id, request);

        // Assert
        Assert.Single(product.Attributes);
        Assert.Equal("Storage", product.Attributes.ToList()[0].Name);
        Assert.Equal(2, product.Attributes.ToList()[0].Values.Count);

        _variantRepoMock.Verify(repo => repo.DeleteRange(It.IsAny<List<ProductVariant>>()), Times.Once);
        _productRepoMock.Verify(repo => repo.RemoveAttributes(It.IsAny<IEnumerable<ProductAttribute>>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenAttributesChangedAndProductHasNoVariants_ShouldReplaceAttributesWithoutDeletingVariants()
    {
        // Arrange
        var product = CreateProduct();
        product.Variants.Clear();

        var request = CreateUpdateProductRequest();

        request.Attributes =
        [
            new ProductAttributeRequest
            {
                Name = "Storage",
                Values =
                [
                    new ProductAttributeValueRequest { Value = "128GB" }
                ]
            }
        ];

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(product.Id))
            .ReturnsAsync(product);

        // Act
        await _productService.UpdateAsync(product.Id, request);

        // Assert
        Assert.Single(product.Attributes);
        Assert.Equal("Storage", product.Attributes.ToList()[0].Name);

        _variantRepoMock.Verify(repo => repo.DeleteRange(It.IsAny<List<ProductVariant>>()), Times.Never);
        _productRepoMock.Verify(repo => repo.RemoveAttributes(It.IsAny<IEnumerable<ProductAttribute>>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_ShouldDeleteProduct()
    {
        // Arrange
        var product = CreateProduct();

        _productRepoMock
            .Setup(repo => repo.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        // Act
        await _productService.DeleteAsync(product.Id);

        // Assert
        _productRepoMock.Verify(repo => repo.Delete(product), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _productRepoMock
            .Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _productService.DeleteAsync(productId));

        // Assert
        Assert.Equal($"Product id '{productId}' not found.", exception.Message);

        _productRepoMock.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenProductDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _productService.UpdateStatusAsync(productId, ProductStatus.Active));

        // Assert
        Assert.Equal($"Product id '{productId}' not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenPublishingValidProduct_ShouldSetStatusToActive()
    {
        // Arrange
        var product = CreateProduct();
        product.Status = ProductStatus.Draft;

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(product.Id))
            .ReturnsAsync(product);

        // Act
        await _productService.UpdateStatusAsync(product.Id, ProductStatus.Active);

        // Assert
        Assert.Equal(ProductStatus.Active, product.Status);
        Assert.NotNull(product.UpdatedAt);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenPublishingProductWithoutName_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var product = CreateProduct();
        product.Name = "";

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(product.Id))
            .ReturnsAsync(product);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _productService.UpdateStatusAsync(product.Id, ProductStatus.Active));

        // Assert
        Assert.Equal("Product name is required.", exception.Message);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenPublishingProductWithoutAttributes_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var product = CreateProduct();
        product.Attributes.Clear();

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(product.Id))
            .ReturnsAsync(product);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _productService.UpdateStatusAsync(product.Id, ProductStatus.Active));

        // Assert
        Assert.Equal("Product must have attributes.", exception.Message);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenPublishingProductWithoutVariants_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var product = CreateProduct();
        product.Variants.Clear();

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(product.Id))
            .ReturnsAsync(product);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _productService.UpdateStatusAsync(product.Id, ProductStatus.Active));

        // Assert
        Assert.Equal("Product must have variants.", exception.Message);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenPublishingProductWithoutVariantImages_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var product = CreateProduct();
        product.Variants.ToList()[0].Images.Clear();

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(product.Id))
            .ReturnsAsync(product);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _productService.UpdateStatusAsync(product.Id, ProductStatus.Active));

        // Assert
        Assert.Equal("At least one variant image is required.", exception.Message);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenPublishingProductWithInvalidVariantPrice_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var product = CreateProduct();
        product.Variants.ToList()[0].Price = 0;

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(product.Id))
            .ReturnsAsync(product);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _productService.UpdateStatusAsync(product.Id, ProductStatus.Active));

        // Assert
        Assert.Equal("All variants must have valid prices.", exception.Message);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenPublishingProductWithEmptySku_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var product = CreateProduct();
        product.Variants.ToList()[0].Sku = "";

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(product.Id))
            .ReturnsAsync(product);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _productService.UpdateStatusAsync(product.Id, ProductStatus.Active));

        // Assert
        Assert.Equal("All variants must have SKU.", exception.Message);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenActiveProductChangedToDraft_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var product = CreateProduct();
        product.Status = ProductStatus.Active;

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(product.Id))
            .ReturnsAsync(product);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _productService.UpdateStatusAsync(product.Id, ProductStatus.Draft));

        // Assert
        Assert.Equal("Active product must be unpublished first.", exception.Message);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenActiveProductChangedToInactive_ShouldUpdateStatus()
    {
        // Arrange
        var product = CreateProduct();
        product.Status = ProductStatus.Active;

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(product.Id))
            .ReturnsAsync(product);

        // Act
        await _productService.UpdateStatusAsync(product.Id, ProductStatus.Inactive);

        // Assert
        Assert.Equal(ProductStatus.Inactive, product.Status);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenArchivedProductChangedToActive_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var product = CreateProduct();
        product.Status = ProductStatus.Archived;

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(product.Id))
            .ReturnsAsync(product);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _productService.UpdateStatusAsync(product.Id, ProductStatus.Active));

        // Assert
        Assert.Equal("Archived product must be restored first.", exception.Message);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenArchivedProductChangedToInactive_ShouldUpdateStatus()
    {
        // Arrange
        var product = CreateProduct();
        product.Status = ProductStatus.Archived;

        _productRepoMock
            .Setup(repo => repo.GetTrackedByIdWithDetailsAsync(product.Id))
            .ReturnsAsync(product);

        // Act
        await _productService.UpdateStatusAsync(product.Id, ProductStatus.Inactive);

        // Assert
        Assert.Equal(ProductStatus.Inactive, product.Status);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public void MapToProductAttribute_WhenIdsProvided_ShouldUseExistingIds()
    {
        // Arrange
        var attributeId = Guid.NewGuid();
        var valueId = Guid.NewGuid();

        var request = new ProductAttributeRequest
        {
            Id = attributeId,
            Name = "Color",
            Values =
            [
                new ProductAttributeValueRequest
                {
                    Id = valueId,
                    Value = "Black"
                }
            ]
        };

        // Act
        var result = ProductService.MapToProductAttribute(request);

        // Assert
        Assert.Equal(attributeId, result.Id);
        Assert.Equal("Color", result.Name);

        var value = Assert.Single(result.Values);
        Assert.Equal(valueId, value.Id);
        Assert.Equal("Black", value.Value);
    }

    [Fact]
    public void MapToProductAttribute_WhenIdsNotProvided_ShouldGenerateIds()
    {
        // Arrange
        var request = new ProductAttributeRequest
        {
            Name = "Color",
            Values =
            [
                new ProductAttributeValueRequest
                {
                    Value = "Black"
                }
            ]
        };

        // Act
        var result = ProductService.MapToProductAttribute(request);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);

        var value = Assert.Single(result.Values);
        Assert.NotEqual(Guid.Empty, value.Id);
        Assert.Equal("Black", value.Value);
    }

    [Fact]
    public void MapToProductSummaryDto_WhenProductHasNoVariants_ShouldUseDefaultValues()
    {
        // Arrange
        var product = CreateProduct();
        product.Variants.Clear();

        // Act
        var result = ProductService.MapToProductSummaryDto(product);

        // Assert
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal(0, result.Price);
        Assert.Null(result.CompareAtPrice);
        Assert.Null(result.DiscountPercentage);
        Assert.Null(result.ThumbnailUrl);
        Assert.True(result.IsOutOfStock);
    }

    [Fact]
    public void MapToProductManagementDto_WhenProductHasNoVariants_ShouldReturnNullPricesAndZeroStock()
    {
        // Arrange
        var product = CreateProduct();
        product.Variants.Clear();

        // Act
        var result = ProductService.MapToProductManagementDto(product);

        // Assert
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(0, result.VariantCount);
        Assert.Equal(0, result.Stock);
        Assert.Null(result.MinPrice);
        Assert.Null(result.MaxPrice);
        Assert.Null(result.ThumbnailUrl);
    }

    [Fact]
    public void MapToProductDetailDto_ShouldMapProductToProductDetailDto()
    {
        // Arrange
        var product = CreateProduct();

        // Act
        var result = ProductService.MapToProductDetailDto(product);

        // Assert
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal(product.Slug, result.Slug);
        Assert.Equal(product.BrandId, result.BrandId);
        Assert.Equal(product.Brand.Name, result.BrandName);
        Assert.Equal(product.CategoryId, result.CategoryId);
        Assert.Equal(product.Category.Name, result.CategoryName);
        Assert.Single(result.Attributes);
        Assert.Single(result.Variants);
    }

    private static Product CreateProduct(
        string name = "iPhone 15",
        string slug = "iphone-15")
    {
        var brandId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var attributeId = Guid.NewGuid();
        var attributeValueId = Guid.NewGuid();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = slug,
            ShortDescription = "Short description",
            Description = "Full description",
            BrandId = brandId,
            CategoryId = categoryId,
            Status = ProductStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            Brand = new Brand
            {
                Id = brandId,
                Name = "Apple",
                Slug = "apple"
            },
            Category = new Category
            {
                Id = categoryId,
                Name = "Phones",
                Slug = "phones"
            },
            Attributes =
            [
                new ProductAttribute
                {
                    Id = attributeId,
                    Name = "Color",
                    Values =
                    [
                        new ProductAttributeValue
                        {
                            Id = attributeValueId,
                            ProductAttributeId = attributeId,
                            Value = "Black"
                        }
                    ]
                }
            ],
            Variants = []
        };

        product.Variants.Add(new ProductVariant
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Sku = "IPHONE-15-BLACK",
            Name = "iPhone 15 Black",
            Description = "Black variant",
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
            Attributes =
            [
                new ProductVariantAttribute
                {
                    ProductAttributeValueId = attributeValueId,
                    ProductAttributeValue = new ProductAttributeValue
                    {
                        Id = attributeValueId,
                        ProductAttributeId = attributeId,
                        Value = "Black",
                        ProductAttribute = new ProductAttribute
                        {
                            Id = attributeId,
                            Name = "Color"
                        }
                    }
                }
            ]
        });

        return product;
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

    private static UpdateProductRequest CreateUpdateProductRequest(string slug = "iphone-15")
    {
        return new UpdateProductRequest
        {
            Name = "iPhone 15 Updated",
            Slug = slug,
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
}
