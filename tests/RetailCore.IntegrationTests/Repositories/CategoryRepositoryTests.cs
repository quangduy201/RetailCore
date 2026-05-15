using RetailCore.IntegrationTests.Fixtures;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories;
using RetailCore.Shared.Enums;

namespace RetailCore.IntegrationTests.Repositories;

public class CategoryRepositoryTests : IDisposable
{
    private readonly SqliteTestDatabase _database;

    public CategoryRepositoryTests()
    {
        _database = new SqliteTestDatabase();
    }

    [Fact]
    public async Task GetPagedAsync_WhenNoFilters_ShouldReturnPagedCategoriesOrderedByCreatedAtDescending()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var olderCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Phones",
            Slug = "phones",
            Description = "Phone category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        var newerCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Laptops",
            Slug = "laptops",
            Description = "Laptop category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Categories.AddRange(olderCategory, newerCategory);
        await context.SaveChangesAsync();

        var repository = new CategoryRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: null,
            status: null,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var categories = items.ToList();

        Assert.Equal(2, totalCount);
        Assert.Equal(2, categories.Count);
        Assert.Equal(newerCategory.Id, categories[0].Id);
        Assert.Equal(olderCategory.Id, categories[1].Id);
    }

    [Fact]
    public async Task GetPagedAsync_WhenKeywordMatchesName_ShouldReturnMatchingCategories()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var phones = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Phones",
            Slug = "phones",
            Description = "Phone category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var laptops = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Laptops",
            Slug = "laptops",
            Description = "Laptop category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Categories.AddRange(phones, laptops);
        await context.SaveChangesAsync();

        var repository = new CategoryRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: "Phones",
            status: null,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var category = Assert.Single(items);

        Assert.Equal(1, totalCount);
        Assert.Equal(phones.Id, category.Id);
    }

    [Fact]
    public async Task GetPagedAsync_WhenKeywordMatchesSlug_ShouldReturnMatchingCategories()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var accessories = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Accessories",
            Slug = "accessories",
            Description = "Accessory category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var monitors = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Monitors",
            Slug = "monitors",
            Description = "Monitor category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Categories.AddRange(accessories, monitors);
        await context.SaveChangesAsync();

        var repository = new CategoryRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: "accessories",
            status: null,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var category = Assert.Single(items);

        Assert.Equal(1, totalCount);
        Assert.Equal(accessories.Id, category.Id);
    }

    [Fact]
    public async Task GetPagedAsync_WhenStatusFilterProvided_ShouldReturnOnlyMatchingStatus()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var activeCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Phones",
            Slug = "phones",
            Description = "Phone category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var inactiveCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Old Category",
            Slug = "old-category",
            Description = "Old category",
            Status = CategoryStatus.Inactive,
            CreatedAt = DateTime.UtcNow
        };

        context.Categories.AddRange(activeCategory, inactiveCategory);
        await context.SaveChangesAsync();

        var repository = new CategoryRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: null,
            status: CategoryStatus.Active,
            pageNumber: 1,
            pageSize: 10);

        // Assert
        var category = Assert.Single(items);

        Assert.Equal(1, totalCount);
        Assert.Equal(activeCategory.Id, category.Id);
        Assert.Equal(CategoryStatus.Active, category.Status);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldApplyPagination()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var category1 = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Category 1",
            Slug = "category-1",
            Description = "Category 1",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow.AddDays(-3)
        };

        var category2 = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Category 2",
            Slug = "category-2",
            Description = "Category 2",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };

        var category3 = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Category 3",
            Slug = "category-3",
            Description = "Category 3",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        context.Categories.AddRange(category1, category2, category3);
        await context.SaveChangesAsync();

        var repository = new CategoryRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            keyword: null,
            status: null,
            pageNumber: 2,
            pageSize: 1);

        // Assert
        var category = Assert.Single(items);

        Assert.Equal(3, totalCount);
        Assert.Equal(category2.Id, category.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ShouldReturnCategory()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Phones",
            Slug = "phones",
            Description = "Phone category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var repository = new CategoryRepository(context);

        // Act
        var result = await repository.GetByIdAsync(category.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(category.Id, result!.Id);
        Assert.Equal("Phones", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new CategoryRepository(context);

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBySlugAsync_WhenCategoryExists_ShouldReturnCategory()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Phones",
            Slug = "phones",
            Description = "Phone category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var repository = new CategoryRepository(context);

        // Act
        var result = await repository.GetBySlugAsync("phones");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(category.Id, result!.Id);
        Assert.Equal("phones", result.Slug);
    }

    [Fact]
    public async Task GetBySlugAsync_WhenCategoryDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new CategoryRepository(context);

        // Act
        var result = await repository.GetBySlugAsync("missing-category");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_WhenSlugDoesNotExist_ShouldReturnTrue()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new CategoryRepository(context);

        // Act
        var result = await repository.IsSlugUniqueAsync("phones");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_WhenSlugExists_ShouldReturnFalse()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Phones",
            Slug = "phones",
            Description = "Phone category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var repository = new CategoryRepository(context);

        // Act
        var result = await repository.IsSlugUniqueAsync("phones");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_WhenSlugBelongsToExcludedCategory_ShouldReturnTrue()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Phones",
            Slug = "phones",
            Description = "Phone category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var repository = new CategoryRepository(context);

        // Act
        var result = await repository.IsSlugUniqueAsync("phones", category.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_WhenSlugBelongsToDifferentCategory_ShouldReturnFalse()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Phones",
            Slug = "phones",
            Description = "Phone category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var otherCategoryId = Guid.NewGuid();

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var repository = new CategoryRepository(context);

        // Act
        var result = await repository.IsSlugUniqueAsync("phones", otherCategoryId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddCategoryAndSaveChanges()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var repository = new CategoryRepository(context);

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Phones",
            Slug = "phones",
            Description = "Phone category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await repository.AddAsync(category);

        // Assert
        var savedCategory = await context.Categories.FindAsync(category.Id);

        Assert.NotNull(savedCategory);
        Assert.Equal("Phones", savedCategory!.Name);
        Assert.Equal("phones", savedCategory.Slug);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCategoryAndSaveChanges()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Phones",
            Slug = "phones",
            Description = "Phone category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var repository = new CategoryRepository(context);

        category.Name = "Phones Updated";
        category.Description = "Updated description";
        category.Status = CategoryStatus.Inactive;

        // Act
        await repository.UpdateAsync(category);

        // Assert
        var updatedCategory = await context.Categories.FindAsync(category.Id);

        Assert.NotNull(updatedCategory);
        Assert.Equal("Phones Updated", updatedCategory!.Name);
        Assert.Equal("Updated description", updatedCategory.Description);
        Assert.Equal(CategoryStatus.Inactive, updatedCategory.Status);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCategoryAndSaveChanges()
    {
        // Arrange
        await using var context = _database.CreateContext();

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Phones",
            Slug = "phones",
            Description = "Phone category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var repository = new CategoryRepository(context);

        // Act
        await repository.DeleteAsync(category);

        // Assert
        var deletedCategory = await context.Categories.FindAsync(category.Id);

        Assert.Null(deletedCategory);
    }

    public void Dispose()
    {
        _database.Dispose();
    }
}
