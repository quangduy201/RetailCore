using Microsoft.EntityFrameworkCore;
using RetailCore.IntegrationTests.Fixtures;
using RetailCore.Repositories.Data;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories;
using RetailCore.Shared.Enums;

namespace RetailCore.IntegrationTests.Repositories;

public class ProductVariantRepositoryTests : IDisposable
{
    private readonly SqliteTestDatabase _database;

    public ProductVariantRepositoryTests()
    {
        _database = new SqliteTestDatabase();
    }

    [Fact]
    public async Task GetByIdAsync_WhenVariantExists_ShouldReturnVariantWithDetailsAsNoTracking()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var variant = await SeedVariantWithDetailsAsync(context);

        context.ChangeTracker.Clear();

        var repository = new ProductVariantRepository(context);

        // Act
        var result = await repository.GetByIdAsync(variant.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(variant.Id, result!.Id);
        Assert.Equal(variant.Sku, result.Sku);

        Assert.Single(result.Images);
        Assert.Single(result.Attributes);

        var attribute = Assert.Single(result.Attributes);
        Assert.NotNull(attribute.ProductAttributeValue);
        Assert.NotNull(attribute.ProductAttributeValue.ProductAttribute);
        Assert.Equal("Color", attribute.ProductAttributeValue.ProductAttribute.Name);

        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetByIdAsync_WhenVariantDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new ProductVariantRepository(context);

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTrackedByIdAsync_WhenVariantExists_ShouldReturnTrackedVariantWithDetails()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var variant = await SeedVariantWithDetailsAsync(context);

        context.ChangeTracker.Clear();

        var repository = new ProductVariantRepository(context);

        // Act
        var result = await repository.GetTrackedByIdAsync(variant.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(variant.Id, result!.Id);
        Assert.Single(result.Images);
        Assert.Single(result.Attributes);

        Assert.NotEmpty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetTrackedByIdAsync_WhenVariantDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new ProductVariantRepository(context);

        // Act
        var result = await repository.GetTrackedByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    // [Fact]
    // public async Task GetByProductIdAsync_ShouldReturnVariantsForProductOnlyAsNoTracking()
    // {
    //     // Arrange
    //     await using var context = _database.CreateContext();

    //     var product = await SeedProductAsync(context);
    //     var otherProduct = await SeedProductAsync(context, name: "Other Product", slug: "other-product");

    //     var variant1 = await SeedVariantWithDetailsAsync(context, product, sku: "SKU-1");
    //     var variant2 = await SeedVariantWithDetailsAsync(context, product, sku: "SKU-2");
    //     await SeedVariantWithDetailsAsync(context, otherProduct, sku: "SKU-3");

    //     context.ChangeTracker.Clear();

    //     var repository = new ProductVariantRepository(context);

    //     // Act
    //     var result = await repository.GetByProductIdAsync(product.Id);

    //     // Assert
    //     Assert.Equal(2, result.Count);
    //     Assert.Contains(result, v => v.Id == variant1.Id);
    //     Assert.Contains(result, v => v.Id == variant2.Id);
    //     Assert.DoesNotContain(result, v => v.Sku == "SKU-3");

    //     Assert.All(result, variant =>
    //     {
    //         Assert.NotEmpty(variant.Images);
    //         Assert.NotEmpty(variant.Attributes);
    //     });

    //     Assert.Empty(context.ChangeTracker.Entries());
    // }

    // [Fact]
    // public async Task GetTrackedByProductIdAsync_ShouldReturnTrackedVariantsForProductOnly()
    // {
    //     // Arrange
    //     await using var context = _database.CreateContext();

    //     var product = await SeedProductAsync(context);
    //     var otherProduct = await SeedProductAsync(context, name: "Other Product", slug: "other-product");

    //     var variant1 = await SeedVariantWithDetailsAsync(context, product, sku: "SKU-1");
    //     var variant2 = await SeedVariantWithDetailsAsync(context, product, sku: "SKU-2");
    //     await SeedVariantWithDetailsAsync(context, otherProduct, sku: "SKU-3");

    //     context.ChangeTracker.Clear();

    //     var repository = new ProductVariantRepository(context);

    //     // Act
    //     var result = await repository.GetTrackedByProductIdAsync(product.Id);

    //     // Assert
    //     Assert.Equal(2, result.Count);
    //     Assert.Contains(result, v => v.Id == variant1.Id);
    //     Assert.Contains(result, v => v.Id == variant2.Id);
    //     Assert.DoesNotContain(result, v => v.Sku == "SKU-3");

    //     Assert.NotEmpty(context.ChangeTracker.Entries());
    // }

    [Fact]
    public async Task GetBySkuAsync_WhenVariantExists_ShouldReturnVariantAsNoTracking()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var variant = await SeedVariantWithDetailsAsync(context, sku: "SKU-1");

        context.ChangeTracker.Clear();

        var repository = new ProductVariantRepository(context);

        // Act
        var result = await repository.GetBySkuAsync("SKU-1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(variant.Id, result!.Id);
        Assert.Equal("SKU-1", result.Sku);
        Assert.Single(result.Images);
        Assert.Single(result.Attributes);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetBySkuAsync_WhenVariantDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new ProductVariantRepository(context);

        // Act
        var result = await repository.GetBySkuAsync("MISSING-SKU");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task IsSkuUniqueAsync_WhenSkuDoesNotExist_ShouldReturnTrue()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new ProductVariantRepository(context);

        // Act
        var result = await repository.IsSkuUniqueAsync("SKU-1");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsSkuUniqueAsync_WhenSkuExists_ShouldReturnFalse()
    {
        // Arrange
        await using var context = _database.CreateContext();

        await SeedVariantWithDetailsAsync(context, sku: "SKU-1");

        var repository = new ProductVariantRepository(context);

        // Act
        var result = await repository.IsSkuUniqueAsync("SKU-1");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsSkuUniqueAsync_WhenSkuBelongsToExcludedVariant_ShouldReturnTrue()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var variant = await SeedVariantWithDetailsAsync(context, sku: "SKU-1");

        var repository = new ProductVariantRepository(context);

        // Act
        var result = await repository.IsSkuUniqueAsync("SKU-1", variant.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsSkuUniqueAsync_WhenSkuBelongsToDifferentVariant_ShouldReturnFalse()
    {
        // Arrange
        await using var context = _database.CreateContext();

        await SeedVariantWithDetailsAsync(context, sku: "SKU-1");

        var repository = new ProductVariantRepository(context);

        // Act
        var result = await repository.IsSkuUniqueAsync("SKU-1", Guid.NewGuid());

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddVariantWithoutSavingUntilSaveChanges()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var product = await SeedProductAsync(context);

        var repository = new ProductVariantRepository(context);

        var variant = new ProductVariant
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Sku = "SKU-NEW",
            Name = "New Variant",
            Description = "New variant description",
            Price = 999,
            CompareAtPrice = 1099,
            Stock = 10,
            Status = ProductVariantStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await repository.AddAsync(variant);

        var beforeSave = await context.ProductVariants
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == variant.Id);

        await context.SaveChangesAsync();

        var afterSave = await context.ProductVariants
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == variant.Id);

        // Assert
        Assert.Null(beforeSave);
        Assert.NotNull(afterSave);
        Assert.Equal("SKU-NEW", afterSave!.Sku);
    }

    [Fact]
    public async Task Update_ShouldMarkVariantAsModifiedAndPersistAfterSaveChanges()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var variant = await SeedVariantWithDetailsAsync(context);

        var repository = new ProductVariantRepository(context);

        variant.Name = "Updated Variant";
        variant.Price = 1999;
        variant.Stock = 50;

        // Act
        repository.Update(variant);
        await context.SaveChangesAsync();

        // Assert
        var updatedVariant = await context.ProductVariants.FindAsync(variant.Id);

        Assert.NotNull(updatedVariant);
        Assert.Equal("Updated Variant", updatedVariant!.Name);
        Assert.Equal(1999, updatedVariant.Price);
        Assert.Equal(50, updatedVariant.Stock);
    }

    [Fact]
    public async Task Delete_ShouldRemoveVariantAfterSaveChanges()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var variant = await SeedVariantWithDetailsAsync(context);

        var repository = new ProductVariantRepository(context);

        // Act
        repository.Delete(variant);
        await context.SaveChangesAsync();

        // Assert
        var deletedVariant = await context.ProductVariants.FindAsync(variant.Id);

        Assert.Null(deletedVariant);
    }

    // [Fact]
    // public async Task DeleteRange_ShouldRemoveVariantsAfterSaveChanges()
    // {
    //     // Arrange
    //     await using var context = _database.CreateContext();

    //     var product = await SeedProductAsync(context);

    //     var variant1 = await SeedVariantWithDetailsAsync(context, product, sku: "SKU-1");
    //     var variant2 = await SeedVariantWithDetailsAsync(context, product, sku: "SKU-2");

    //     var variants = await context.ProductVariants
    //         .Where(v => v.ProductId == product.Id)
    //         .ToListAsync();

    //     var repository = new ProductVariantRepository(context);

    //     // Act
    //     repository.DeleteRange(variants);
    //     await context.SaveChangesAsync();

    //     // Assert
    //     var remainingVariants = await context.ProductVariants
    //         .Where(v => v.ProductId == product.Id)
    //         .ToListAsync();

    //     Assert.Empty(remainingVariants);
    //     Assert.Equal(2, variants.Count);
    //     Assert.Contains(variants, v => v.Id == variant1.Id);
    //     Assert.Contains(variants, v => v.Id == variant2.Id);
    // }

    [Fact]
    public async Task RemoveImages_ShouldRemoveImagesAfterSaveChanges()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var variant = await SeedVariantWithDetailsAsync(context);

        var images = await context.ProductVariantImages
            .Where(i => i.ProductVariantId == variant.Id)
            .ToListAsync();

        var repository = new ProductVariantRepository(context);

        // Act
        repository.RemoveImages(images);
        await context.SaveChangesAsync();

        // Assert
        var remainingImages = await context.ProductVariantImages
            .Where(i => i.ProductVariantId == variant.Id)
            .ToListAsync();

        Assert.Empty(remainingImages);
    }

    [Fact]
    public async Task RemoveAttributes_ShouldRemoveVariantAttributesAfterSaveChanges()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var variant = await SeedVariantWithDetailsAsync(context);

        var attributes = await context.ProductVariantAttributes
            .Where(a => a.ProductVariantId == variant.Id)
            .ToListAsync();

        var repository = new ProductVariantRepository(context);

        // Act
        repository.RemoveAttributes(attributes);
        await context.SaveChangesAsync();

        // Assert
        var remainingAttributes = await context.ProductVariantAttributes
            .Where(a => a.ProductVariantId == variant.Id)
            .ToListAsync();

        Assert.Empty(remainingAttributes);
    }

    public void Dispose()
    {
        _database.Dispose();
    }

    private static async Task<Brand> SeedBrandAsync(
        AppDbContext context,
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
        AppDbContext context,
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
        AppDbContext context,
        Brand? brand = null,
        Category? category = null,
        string name = "iPhone 15",
        string slug = "iphone-15",
        ProductStatus status = ProductStatus.Active)
    {
        brand ??= await SeedBrandAsync(
            context,
            name: $"{name} Brand",
            slug: $"{slug}-brand");

        category ??= await SeedCategoryAsync(
            context,
            name: $"{name} Category",
            slug: $"{slug}-category");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = slug,
            ShortDescription = "Short description",
            Description = "Full description",
            BrandId = brand.Id,
            CategoryId = category.Id,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        return product;
    }

    private static async Task<ProductVariant> SeedVariantWithDetailsAsync(
        AppDbContext context,
        Product? product = null,
        string sku = "SKU-1")
    {
        product ??= await SeedProductAsync(context);

        var productAttribute = new ProductAttribute
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Name = "Color"
        };

        var productAttributeValue = new ProductAttributeValue
        {
            Id = Guid.NewGuid(),
            ProductAttributeId = productAttribute.Id,
            Value = "Black"
        };

        var variant = new ProductVariant
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Sku = sku,
            Name = "Black Variant",
            Description = "Black variant description",
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
            ProductAttributeValueId = productAttributeValue.Id
        };

        context.ProductAttributes.Add(productAttribute);
        context.ProductAttributeValues.Add(productAttributeValue);
        context.ProductVariants.Add(variant);
        context.ProductVariantImages.Add(image);
        context.ProductVariantAttributes.Add(variantAttribute);

        await context.SaveChangesAsync();

        return variant;
    }
}
