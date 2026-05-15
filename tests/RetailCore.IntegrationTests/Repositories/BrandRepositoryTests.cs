using RetailCore.IntegrationTests.Fixtures;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories;
using RetailCore.Shared.Enums;

namespace RetailCore.IntegrationTests.Repositories;

public class BrandRepositoryTests : IDisposable
{
    private readonly SqliteTestDatabase _database;

    public BrandRepositoryTests()
    {
        _database = new SqliteTestDatabase();
    }

    [Fact]
    public async Task GetPagedAsync_WhenNoFilters_ShouldReturnPagedBrandsOrderedByCreatedAtDescending()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var olderBrand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        var newerBrand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Samsung",
            Slug = "samsung",
            Description = "Samsung products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Brands.AddRange(olderBrand, newerBrand);
        await context.SaveChangesAsync();

        var repository = new BrandRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: null,
            status: null,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var brands = items.ToList();

        Assert.Equal(2, totalCount);
        Assert.Equal(2, brands.Count);
        Assert.Equal(newerBrand.Id, brands[0].Id);
        Assert.Equal(olderBrand.Id, brands[1].Id);
    }

    [Fact]
    public async Task GetPagedAsync_WhenKeywordMatchesName_ShouldReturnMatchingBrands()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var apple = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var samsung = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Samsung",
            Slug = "samsung",
            Description = "Samsung products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Brands.AddRange(apple, samsung);
        await context.SaveChangesAsync();

        var repository = new BrandRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: "Apple",
            status: null,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var brand = Assert.Single(items);

        Assert.Equal(1, totalCount);
        Assert.Equal(apple.Id, brand.Id);
    }

    [Fact]
    public async Task GetPagedAsync_WhenKeywordMatchesSlug_ShouldReturnMatchingBrands()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var sony = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Sony Corporation",
            Slug = "sony",
            Description = "Sony products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var lg = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "LG Electronics",
            Slug = "lg",
            Description = "LG products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Brands.AddRange(sony, lg);
        await context.SaveChangesAsync();

        var repository = new BrandRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: "sony",
            status: null,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var brand = Assert.Single(items);

        Assert.Equal(1, totalCount);
        Assert.Equal(sony.Id, brand.Id);
    }

    [Fact]
    public async Task GetPagedAsync_WhenStatusFilterProvided_ShouldReturnOnlyMatchingStatus()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var activeBrand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var inactiveBrand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Old Brand",
            Slug = "old-brand",
            Description = "Old products",
            Status = BrandStatus.Inactive,
            CreatedAt = DateTime.UtcNow
        };

        context.Brands.AddRange(activeBrand, inactiveBrand);
        await context.SaveChangesAsync();

        var repository = new BrandRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: null,
            status: BrandStatus.Active,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var brand = Assert.Single(items);

        Assert.Equal(1, totalCount);
        Assert.Equal(activeBrand.Id, brand.Id);
        Assert.Equal(BrandStatus.Active, brand.Status);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldApplyPagination()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var brand1 = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Brand 1",
            Slug = "brand-1",
            Description = "Brand 1",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow.AddDays(-3)
        };

        var brand2 = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Brand 2",
            Slug = "brand-2",
            Description = "Brand 2",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };

        var brand3 = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Brand 3",
            Slug = "brand-3",
            Description = "Brand 3",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        context.Brands.AddRange(brand1, brand2, brand3);
        await context.SaveChangesAsync();

        var repository = new BrandRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: null,
            status: null,
            pageNumber: 2,
            pageSize: 1);

        // Assert
        var brand = Assert.Single(items);

        Assert.Equal(3, totalCount);
        Assert.Equal(brand2.Id, brand.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBrandExists_ShouldReturnBrand()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Brands.Add(brand);
        await context.SaveChangesAsync();

        var repository = new BrandRepository(context);

        // Act
        var result = await repository.GetByIdAsync(brand.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(brand.Id, result!.Id);
        Assert.Equal("Apple", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBrandDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new BrandRepository(context);

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBySlugAsync_WhenBrandExists_ShouldReturnBrand()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Brands.Add(brand);
        await context.SaveChangesAsync();

        var repository = new BrandRepository(context);

        // Act
        var result = await repository.GetBySlugAsync("apple");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(brand.Id, result!.Id);
        Assert.Equal("apple", result.Slug);
    }

    [Fact]
    public async Task GetBySlugAsync_WhenBrandDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new BrandRepository(context);

        // Act
        var result = await repository.GetBySlugAsync("missing-brand");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_WhenSlugDoesNotExist_ShouldReturnTrue()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new BrandRepository(context);

        // Act
        var result = await repository.IsSlugUniqueAsync("apple");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_WhenSlugExists_ShouldReturnFalse()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Brands.Add(brand);
        await context.SaveChangesAsync();

        var repository = new BrandRepository(context);

        // Act
        var result = await repository.IsSlugUniqueAsync("apple");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_WhenSlugBelongsToExcludedBrand_ShouldReturnTrue()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Brands.Add(brand);
        await context.SaveChangesAsync();

        var repository = new BrandRepository(context);

        // Act
        var result = await repository.IsSlugUniqueAsync("apple", brand.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_WhenSlugBelongsToDifferentBrand_ShouldReturnFalse()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var otherBrandId = Guid.NewGuid();

        context.Brands.Add(brand);
        await context.SaveChangesAsync();

        var repository = new BrandRepository(context);

        // Act
        var result = await repository.IsSlugUniqueAsync("apple", otherBrandId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddBrandAndSaveChanges()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new BrandRepository(context);

        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await repository.AddAsync(brand);

        // Assert
        var savedBrand = await context.Brands.FindAsync(brand.Id);

        Assert.NotNull(savedBrand);
        Assert.Equal("Apple", savedBrand!.Name);
        Assert.Equal("apple", savedBrand.Slug);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBrandAndSaveChanges()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Brands.Add(brand);
        await context.SaveChangesAsync();

        var repository = new BrandRepository(context);

        brand.Name = "Apple Updated";
        brand.Description = "Updated description";
        brand.Status = BrandStatus.Inactive;

        // Act
        await repository.UpdateAsync(brand);

        // Assert
        var updatedBrand = await context.Brands.FindAsync(brand.Id);

        Assert.NotNull(updatedBrand);
        Assert.Equal("Apple Updated", updatedBrand!.Name);
        Assert.Equal("Updated description", updatedBrand.Description);
        Assert.Equal(BrandStatus.Inactive, updatedBrand.Status);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveBrandAndSaveChanges()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Brands.Add(brand);
        await context.SaveChangesAsync();

        var repository = new BrandRepository(context);

        // Act
        await repository.DeleteAsync(brand);

        // Assert
        var deletedBrand = await context.Brands.FindAsync(brand.Id);

        Assert.Null(deletedBrand);
    }

    public void Dispose()
    {
        _database.Dispose();
    }
}
