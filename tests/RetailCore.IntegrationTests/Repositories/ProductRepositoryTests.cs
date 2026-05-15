using Microsoft.EntityFrameworkCore;
using RetailCore.IntegrationTests.Fixtures;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories;
using RetailCore.Shared.Enums;

namespace RetailCore.IntegrationTests.Repositories;

public class ProductRepositoryTests : IDisposable
{
    private readonly SqliteTestDatabase _database;

    public ProductRepositoryTests()
    {
        _database = new SqliteTestDatabase();
    }

    [Fact]
    public async Task GetPagedAsync_WhenNoFilters_ShouldReturnProductsOrderedByCreatedAtDescending()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var olderProduct = await SeedProductAsync(context, name: "Old Product", createdAt: DateTime.UtcNow.AddDays(-10));
        var newerProduct = await SeedProductAsync(context, name: "New Product", createdAt: DateTime.UtcNow);

        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: null,
            brandId: null,
            categoryId: null,
            brandSlug: null,
            categorySlug: null,
            status: null,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var products = items.ToList();

        Assert.Equal(2, totalCount);
        Assert.Equal(2, products.Count);
        Assert.Equal(newerProduct.Id, products[0].Id);
        Assert.Equal(olderProduct.Id, products[1].Id);
    }

    [Fact]
    public async Task GetPagedAsync_WhenKeywordMatchesName_ShouldReturnMatchingProducts()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var iphone = await SeedProductAsync(context, name: "iPhone 15");
        await SeedProductAsync(context, name: "Galaxy S24");

        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: "iPhone",
            brandId: null,
            categoryId: null,
            brandSlug: null,
            categorySlug: null,
            status: null,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var product = Assert.Single(items);

        Assert.Equal(1, totalCount);
        Assert.Equal(iphone.Id, product.Id);
    }

    [Fact]
    public async Task GetPagedAsync_WhenKeywordMatchesSlug_ShouldReturnMatchingProducts()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var iphone = await SeedProductAsync(context, name: "iPhone 15", slug: "iphone-15");
        await SeedProductAsync(context, name: "Galaxy S24", slug: "galaxy-s24");

        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: "iphone-15",
            brandId: null,
            categoryId: null,
            brandSlug: null,
            categorySlug: null,
            status: null,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var product = Assert.Single(items);

        Assert.Equal(1, totalCount);
        Assert.Equal(iphone.Id, product.Id);
    }

    [Fact]
    public async Task GetPagedAsync_WhenBrandIdProvided_ShouldReturnMatchingProducts()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var apple = await SeedBrandAsync(context, "Apple", "apple");
        var samsung = await SeedBrandAsync(context, "Samsung", "samsung");
        var category = await SeedCategoryAsync(context);

        var appleProduct = await SeedProductAsync(context, brand: apple, category: category, name: "iPhone 15");
        await SeedProductAsync(context, brand: samsung, category: category, name: "Galaxy S24");

        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: null,
            brandId: apple.Id,
            categoryId: null,
            brandSlug: null,
            categorySlug: null,
            status: null,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var product = Assert.Single(items);

        Assert.Equal(1, totalCount);
        Assert.Equal(appleProduct.Id, product.Id);
        Assert.Equal(apple.Id, product.BrandId);
    }

    [Fact]
    public async Task GetPagedAsync_WhenCategoryIdProvided_ShouldReturnMatchingProducts()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var brand = await SeedBrandAsync(context);
        var phones = await SeedCategoryAsync(context, "Phones", "phones");
        var laptops = await SeedCategoryAsync(context, "Laptops", "laptops");

        var phoneProduct = await SeedProductAsync(context, brand: brand, category: phones, name: "iPhone 15");
        await SeedProductAsync(context, brand: brand, category: laptops, name: "MacBook Air");

        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: null,
            brandId: null,
            categoryId: phones.Id,
            brandSlug: null,
            categorySlug: null,
            status: null,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var product = Assert.Single(items);

        Assert.Equal(1, totalCount);
        Assert.Equal(phoneProduct.Id, product.Id);
        Assert.Equal(phones.Id, product.CategoryId);
    }

    [Fact]
    public async Task GetPagedAsync_WhenBrandSlugProvided_ShouldReturnMatchingProducts()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var apple = await SeedBrandAsync(context, "Apple", "apple");
        var samsung = await SeedBrandAsync(context, "Samsung", "samsung");
        var category = await SeedCategoryAsync(context);

        var appleProduct = await SeedProductAsync(context, brand: apple, category: category, name: "iPhone 15");
        await SeedProductAsync(context, brand: samsung, category: category, name: "Galaxy S24");

        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: null,
            brandId: null,
            categoryId: null,
            brandSlug: "apple",
            categorySlug: null,
            status: null,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var product = Assert.Single(items);

        Assert.Equal(1, totalCount);
        Assert.Equal(appleProduct.Id, product.Id);
        Assert.Equal("apple", product.Brand.Slug);
    }

    [Fact]
    public async Task GetPagedAsync_WhenCategorySlugProvided_ShouldReturnMatchingProducts()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var brand = await SeedBrandAsync(context);
        var phones = await SeedCategoryAsync(context, "Phones", "phones");
        var laptops = await SeedCategoryAsync(context, "Laptops", "laptops");

        var phoneProduct = await SeedProductAsync(context, brand: brand, category: phones, name: "iPhone 15");
        await SeedProductAsync(context, brand: brand, category: laptops, name: "MacBook Air");

        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: null,
            brandId: null,
            categoryId: null,
            brandSlug: null,
            categorySlug: "phones",
            status: null,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var product = Assert.Single(items);

        Assert.Equal(1, totalCount);
        Assert.Equal(phoneProduct.Id, product.Id);
        Assert.Equal("phones", product.Category.Slug);
    }

    [Fact]
    public async Task GetPagedAsync_WhenStatusProvided_ShouldReturnMatchingProducts()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var activeProduct = await SeedProductAsync(context, name: "Active Product", status: ProductStatus.Active);
        await SeedProductAsync(context, name: "Draft Product", status: ProductStatus.Draft);

        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: null,
            brandId: null,
            categoryId: null,
            brandSlug: null,
            categorySlug: null,
            status: ProductStatus.Active,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var product = Assert.Single(items);

        Assert.Equal(1, totalCount);
        Assert.Equal(activeProduct.Id, product.Id);
        Assert.Equal(ProductStatus.Active, product.Status);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldApplyPagination()
    {
        // Arrange
        await using var context = _database.CreateContext();

        await SeedProductAsync(context, name: "Product 1", createdAt: DateTime.UtcNow.AddDays(-3));
        var product2 = await SeedProductAsync(context, name: "Product 2", createdAt: DateTime.UtcNow.AddDays(-2));
        await SeedProductAsync(context, name: "Product 3", createdAt: DateTime.UtcNow.AddDays(-1));

        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: null,
            brandId: null,
            categoryId: null,
            brandSlug: null,
            categorySlug: null,
            status: null,
            pageNumber: 2,
            pageSize: 1);

        // Assert
        var product = Assert.Single(items);

        Assert.Equal(3, totalCount);
        Assert.Equal(product2.Id, product.Id);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldIncludeBrandCategoryVariantsAndImages()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var product = await SeedProductWithVariantAndImageAsync(context);

        var repository = new ProductRepository(context);

        // Act
        var (items, _) = await repository.GetPagedAsync(
            keyword: null,
            brandId: null,
            categoryId: null,
            brandSlug: null,
            categorySlug: null,
            status: null,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var result = Assert.Single(items);

        Assert.Equal(product.Id, result.Id);
        Assert.NotNull(result.Brand);
        Assert.NotNull(result.Category);

        var variant = Assert.Single(result.Variants);
        var image = Assert.Single(variant.Images);

        Assert.Equal("image.jpg", image.Url);
    }

    [Fact]
    public async Task GetPagedForManagementAsync_WhenNoFilters_ShouldReturnProductsOrderedByCreatedAtDescending()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var olderProduct = await SeedProductAsync(context, name: "Old Product", createdAt: DateTime.UtcNow.AddDays(-10));
        var newerProduct = await SeedProductAsync(context, name: "New Product", createdAt: DateTime.UtcNow);

        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedForManagementAsync(
            keyword: null,
            status: null,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var products = items.ToList();

        Assert.Equal(2, totalCount);
        Assert.Equal(2, products.Count);
        Assert.Equal(newerProduct.Id, products[0].Id);
        Assert.Equal(olderProduct.Id, products[1].Id);
    }

    [Fact]
    public async Task GetPagedForManagementAsync_WhenKeywordProvided_ShouldReturnMatchingProducts()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var iphone = await SeedProductAsync(context, name: "iPhone 15");
        await SeedProductAsync(context, name: "Galaxy S24");

        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedForManagementAsync(
            keyword: "iPhone",
            status: null,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var product = Assert.Single(items);

        Assert.Equal(1, totalCount);
        Assert.Equal(iphone.Id, product.Id);
    }

    [Fact]
    public async Task GetPagedForManagementAsync_WhenStatusProvided_ShouldReturnMatchingProducts()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var draftProduct = await SeedProductAsync(context, name: "Draft Product", status: ProductStatus.Draft);
        await SeedProductAsync(context, name: "Active Product", status: ProductStatus.Active);

        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedForManagementAsync(
            keyword: null,
            status: ProductStatus.Draft,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var product = Assert.Single(items);

        Assert.Equal(1, totalCount);
        Assert.Equal(draftProduct.Id, product.Id);
    }

    [Fact]
    public async Task GetPagedForManagementAsync_ShouldApplyPagination()
    {
        // Arrange
        await using var context = _database.CreateContext();

        await SeedProductAsync(context, name: "Product 1", createdAt: DateTime.UtcNow.AddDays(-3));
        var product2 = await SeedProductAsync(context, name: "Product 2", createdAt: DateTime.UtcNow.AddDays(-2));
        await SeedProductAsync(context, name: "Product 3", createdAt: DateTime.UtcNow.AddDays(-1));

        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedForManagementAsync(
            keyword: null,
            status: null,
            pageNumber: 2,
            pageSize: 1);

        // Assert
        var product = Assert.Single(items);

        Assert.Equal(3, totalCount);
        Assert.Equal(product2.Id, product.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ShouldReturnProduct()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var product = await SeedProductAsync(context);

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdWithDetailsAsync_WhenProductExists_ShouldReturnProductWithDetailsAsNoTracking()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var product = await SeedProductWithFullDetailsAsync(context);

        context.ChangeTracker.Clear();

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetByIdWithDetailsAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result!.Id);

        Assert.NotNull(result.Brand);
        Assert.NotNull(result.Category);
        Assert.Single(result.Attributes);
        Assert.Single(result.Variants);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetTrackedByIdWithDetailsAsync_WhenProductExists_ShouldReturnTrackedProductWithDetails()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var product = await SeedProductWithFullDetailsAsync(context);

        context.ChangeTracker.Clear();

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetTrackedByIdWithDetailsAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result!.Id);

        Assert.NotNull(result.Brand);
        Assert.NotNull(result.Category);
        Assert.Single(result.Attributes);
        Assert.Single(result.Variants);
        Assert.NotEmpty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetBySlugAsync_WhenProductExists_ShouldReturnProductAsNoTracking()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var product = await SeedProductAsync(context, slug: "iphone-15");

        context.ChangeTracker.Clear();

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetBySlugAsync("iphone-15");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result!.Id);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetBySlugAsync_WhenProductDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetBySlugAsync("missing-product");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBySlugWithDetailsAsync_WhenProductExists_ShouldReturnProductWithDetailsAsNoTracking()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var product = await SeedProductWithFullDetailsAsync(context, slug: "iphone-15");

        context.ChangeTracker.Clear();

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetBySlugWithDetailsAsync("iphone-15");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result!.Id);

        Assert.NotNull(result.Brand);
        Assert.NotNull(result.Category);
        Assert.Single(result.Attributes);
        Assert.Single(result.Variants);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task IsSlugUniqueAsync_WhenSlugDoesNotExist_ShouldReturnTrue()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.IsSlugUniqueAsync("iphone-15");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_WhenSlugExists_ShouldReturnFalse()
    {
        // Arrange
        await using var context = _database.CreateContext();

        await SeedProductAsync(context, slug: "iphone-15");

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.IsSlugUniqueAsync("iphone-15");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_WhenSlugBelongsToExcludedProduct_ShouldReturnTrue()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var product = await SeedProductAsync(context, slug: "iphone-15");

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.IsSlugUniqueAsync("iphone-15", product.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_WhenSlugBelongsToDifferentProduct_ShouldReturnFalse()
    {
        // Arrange
        await using var context = _database.CreateContext();

        await SeedProductAsync(context, slug: "iphone-15");

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.IsSlugUniqueAsync("iphone-15", Guid.NewGuid());

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProductWithoutSavingUntilSaveChanges()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var brand = await SeedBrandAsync(context);
        var category = await SeedCategoryAsync(context);

        var repository = new ProductRepository(context);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "iPhone 15",
            Slug = "iphone-15",
            ShortDescription = "Short",
            Description = "Full",
            BrandId = brand.Id,
            CategoryId = category.Id,
            Status = ProductStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await repository.AddAsync(product);

        var beforeSave = await context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == product.Id);

        await context.SaveChangesAsync();

        var afterSave = await context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == product.Id);

        // Assert
        Assert.Null(beforeSave);
        Assert.NotNull(afterSave);
        Assert.Equal(product.Id, afterSave!.Id);
    }

    [Fact]
    public async Task Update_ShouldMarkProductAsModifiedAndPersistAfterSaveChanges()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var product = await SeedProductAsync(context);

        var repository = new ProductRepository(context);

        product.Name = "Updated Product";

        // Act
        repository.Update(product);
        await context.SaveChangesAsync();

        // Assert
        var updatedProduct = await context.Products.FindAsync(product.Id);

        Assert.NotNull(updatedProduct);
        Assert.Equal("Updated Product", updatedProduct!.Name);
    }

    [Fact]
    public async Task Delete_ShouldRemoveProductAfterSaveChanges()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var product = await SeedProductAsync(context);

        var repository = new ProductRepository(context);

        // Act
        repository.Delete(product);
        await context.SaveChangesAsync();

        // Assert
        var deletedProduct = await context.Products.FindAsync(product.Id);

        Assert.Null(deletedProduct);
    }

    // [Fact]
    // public async Task RemoveAttributes_ShouldRemoveAttributesAfterSaveChanges()
    // {
    //     // Arrange
    //     await using var context = _database.CreateContext();

    //     var product = await SeedProductWithFullDetailsAsync(context);

    //     var attributes = await context.ProductAttributes
    //         .Where(a => a.ProductId == product.Id)
    //         .ToListAsync();

    //     var repository = new ProductRepository(context);

    //     // Act
    //     repository.RemoveAttributes(attributes);
    //     await context.SaveChangesAsync();

    //     // Assert
    //     var remainingAttributes = await context.ProductAttributes
    //         .Where(a => a.ProductId == product.Id)
    //         .ToListAsync();

    //     Assert.Empty(remainingAttributes);
    // }

    public void Dispose()
    {
        _database.Dispose();
    }

    private static async Task<Brand> SeedBrandAsync(
        RetailCore.Repositories.Data.AppDbContext context,
        string name = "Apple",
        string slug = "apple")
    {
        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = slug,
            Description = $"{name} description",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Brands.Add(brand);
        await context.SaveChangesAsync();

        return brand;
    }

    private static async Task<Category> SeedCategoryAsync(
        RetailCore.Repositories.Data.AppDbContext context,
        string name = "Phones",
        string slug = "phones")
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = slug,
            Description = $"{name} description",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        return category;
    }

    private static async Task<Product> SeedProductAsync(
        RetailCore.Repositories.Data.AppDbContext context,
        Brand? brand = null,
        Category? category = null,
        string name = "iPhone 15",
        string? slug = null,
        ProductStatus status = ProductStatus.Active,
        DateTime? createdAt = null)
    {
        brand ??= await SeedBrandAsync(context, name: $"{name} Brand", slug: $"{name.ToLowerInvariant().Replace(" ", "-")}-brand");
        category ??= await SeedCategoryAsync(context, name: $"{name} Category", slug: $"{name.ToLowerInvariant().Replace(" ", "-")}-category");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = slug ?? name.ToLowerInvariant().Replace(" ", "-"),
            ShortDescription = "Short description",
            Description = "Full description",
            BrandId = brand.Id,
            CategoryId = category.Id,
            Status = status,
            CreatedAt = createdAt ?? DateTime.UtcNow
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        return product;
    }

    private static async Task<Product> SeedProductWithVariantAndImageAsync(
        RetailCore.Repositories.Data.AppDbContext context)
    {
        var product = await SeedProductAsync(context);

        var variant = new ProductVariant
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Sku = "SKU-1",
            Name = "Variant",
            Description = "Variant description",
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
                    Url = "image.jpg",
                    SortOrder = 1,
                    IsPrimary = true,
                    CreatedAt = DateTime.UtcNow
                }
            ]
        };

        context.ProductVariants.Add(variant);
        await context.SaveChangesAsync();

        return product;
    }

    private static async Task<Product> SeedProductWithFullDetailsAsync(
        RetailCore.Repositories.Data.AppDbContext context,
        string slug = "iphone-15")
    {
        var product = await SeedProductAsync(context, slug: slug);

        var attribute = new ProductAttribute
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Name = "Color"
        };

        var attributeValue = new ProductAttributeValue
        {
            Id = Guid.NewGuid(),
            ProductAttributeId = attribute.Id,
            Value = "Black"
        };

        var variant = new ProductVariant
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Sku = $"SKU-{Guid.NewGuid():N}",
            Name = "Black Variant",
            Description = "Black variant",
            Price = 999,
            CompareAtPrice = 1099,
            Stock = 10,
            Status = ProductVariantStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var image = new ProductVariantImage
        {
            Id = Guid.NewGuid(),
            ProductVariantId = variant.Id,
            Url = "image.jpg",
            SortOrder = 1,
            IsPrimary = true,
            CreatedAt = DateTime.UtcNow
        };

        var variantAttribute = new ProductVariantAttribute
        {
            ProductVariantId = variant.Id,
            ProductAttributeValueId = attributeValue.Id
        };

        context.ProductAttributes.Add(attribute);
        context.ProductAttributeValues.Add(attributeValue);
        context.ProductVariants.Add(variant);
        context.ProductVariantImages.Add(image);
        context.ProductVariantAttributes.Add(variantAttribute);

        await context.SaveChangesAsync();

        return product;
    }
}
