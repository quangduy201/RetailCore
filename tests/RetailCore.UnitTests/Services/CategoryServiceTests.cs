using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Category;

namespace RetailCore.UnitTests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _categoryService = new CategoryService(_categoryRepoMock.Object);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedCategorySummaryDtos()
    {
        // Arrange
        var request = new GetCategoriesRequest
        {
            Keyword = "phone",
            Status = CategoryStatus.Active,
            PageNumber = 1,
            PageSize = 10
        };

        var categories = new List<Category>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Phones",
                Slug = "phones",
                Description = "Phone category",
                Status = CategoryStatus.Active
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Phone Accessories",
                Slug = "phone-accessories",
                Description = "Accessories category",
                Status = CategoryStatus.Active
            }
        };

        _categoryRepoMock
            .Setup(repo => repo.GetPagedAsync(
                request.Keyword,
                request.Status,
                request.PageNumber,
                request.PageSize))
            .ReturnsAsync((categories, categories.Count));

        // Act
        var result = await _categoryService.GetPagedAsync(request);

        // Assert
        Assert.Equal(request.PageNumber, result.PageNumber);
        Assert.Equal(request.PageSize, result.PageSize);
        Assert.Equal(categories.Count, result.TotalCount);

        var items = result.Items.ToList();

        Assert.Equal(2, items.Count);
        Assert.Equal(categories[0].Id, items[0].Id);
        Assert.Equal("Phones", items[0].Name);
        Assert.Equal("phones", items[0].Slug);
        Assert.Equal("Phone category", items[0].Description);

        Assert.Equal(categories[1].Id, items[1].Id);
        Assert.Equal("Phone Accessories", items[1].Name);
        Assert.Equal("phone-accessories", items[1].Slug);
        Assert.Equal("Accessories category", items[1].Description);
    }

    [Fact]
    public async Task GetPagedForManagementAsync_ShouldReturnPagedCategoryDetailDtos()
    {
        // Arrange
        var request = new GetCategoriesRequest
        {
            Keyword = null,
            Status = CategoryStatus.Active,
            PageNumber = 1,
            PageSize = 10
        };

        var createdAt = DateTime.UtcNow;

        var categories = new List<Category>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Laptops",
                Slug = "laptops",
                Description = "Laptop category",
                Status = CategoryStatus.Active,
                CreatedAt = createdAt
            }
        };

        _categoryRepoMock
            .Setup(repo => repo.GetPagedAsync(
                request.Keyword,
                request.Status,
                request.PageNumber,
                request.PageSize))
            .ReturnsAsync((categories, categories.Count));

        // Act
        var result = await _categoryService.GetPagedForManagementAsync(request);

        // Assert
        Assert.Equal(request.PageNumber, result.PageNumber);
        Assert.Equal(request.PageSize, result.PageSize);
        Assert.Equal(categories.Count, result.TotalCount);

        var item = Assert.Single(result.Items);

        Assert.Equal(categories[0].Id, item.Id);
        Assert.Equal("Laptops", item.Name);
        Assert.Equal("laptops", item.Slug);
        Assert.Equal("Laptop category", item.Description);
        Assert.Equal(CategoryStatus.Active, item.Status);
        Assert.Equal(createdAt, item.CreatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ShouldReturnCategoryDetailDto()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var category = new Category
        {
            Id = categoryId,
            Name = "Laptops",
            Slug = "laptops",
            Description = "Laptop category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _categoryRepoMock
            .Setup(repo => repo.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        // Act
        var result = await _categoryService.GetByIdAsync(categoryId);

        // Assert
        Assert.Equal(category.Id, result.Id);
        Assert.Equal(category.Name, result.Name);
        Assert.Equal(category.Slug, result.Slug);
        Assert.Equal(category.Description, result.Description);
        Assert.Equal(category.Status, result.Status);
        Assert.Equal(category.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _categoryRepoMock
            .Setup(repo => repo.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _categoryService.GetByIdAsync(categoryId));

        // Assert
        Assert.Equal($"Category id '{categoryId}' not found.", exception.Message);
    }

    [Fact]
    public async Task GetBySlugAsync_WhenCategoryExists_ShouldReturnCategoryDetailDto()
    {
        // Arrange
        var slug = "laptops";

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Laptops",
            Slug = slug,
            Description = "Laptop category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _categoryRepoMock
            .Setup(repo => repo.GetBySlugAsync(slug))
            .ReturnsAsync(category);

        // Act
        var result = await _categoryService.GetBySlugAsync(slug);

        // Assert
        Assert.Equal(category.Id, result.Id);
        Assert.Equal(category.Name, result.Name);
        Assert.Equal(category.Slug, result.Slug);
        Assert.Equal(category.Description, result.Description);
        Assert.Equal(category.Status, result.Status);
    }

    [Fact]
    public async Task GetBySlugAsync_WhenCategoryDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var slug = "unknown-category";

        _categoryRepoMock
            .Setup(repo => repo.GetBySlugAsync(slug))
            .ReturnsAsync((Category?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _categoryService.GetBySlugAsync(slug));

        // Assert
        Assert.Equal($"Category slug '{slug}' not found.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenSlugIsUnique_ShouldCreateCategoryAndReturnId()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Name = "Tablets",
            Slug = "tablets",
            Description = "Tablet category"
        };

        Category? createdCategory = null;

        _categoryRepoMock
            .Setup(repo => repo.IsSlugUniqueAsync(request.Slug, null))
            .ReturnsAsync(true);

        _categoryRepoMock
            .Setup(repo => repo.AddAsync(It.IsAny<Category>()))
            .Callback<Category>(category => createdCategory = category)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _categoryService.CreateAsync(request);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        Assert.NotNull(createdCategory);

        Assert.Equal(result, createdCategory!.Id);
        Assert.Equal(request.Name, createdCategory.Name);
        Assert.Equal(request.Slug, createdCategory.Slug);
        Assert.Equal(request.Description, createdCategory.Description);
        Assert.Equal(CategoryStatus.Active, createdCategory.Status);
        Assert.NotEqual(default, createdCategory.CreatedAt);

        _categoryRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenSlugAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Name = "Laptops",
            Slug = "laptops",
            Description = "Duplicate category"
        };

        _categoryRepoMock
            .Setup(repo => repo.IsSlugUniqueAsync(request.Slug, null))
            .ReturnsAsync(false);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _categoryService.CreateAsync(request));

        // Assert
        Assert.Equal($"Category slug '{request.Slug}' already exists.", exception.Message);

        _categoryRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenCategoryExistsAndSlugUnchanged_ShouldUpdateCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var category = new Category
        {
            Id = categoryId,
            Name = "Old Name",
            Slug = "laptops",
            Description = "Old description",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var request = new UpdateCategoryRequest
        {
            Name = "Laptops",
            Slug = "laptops",
            Description = "Updated description",
            Status = CategoryStatus.Inactive
        };

        _categoryRepoMock
            .Setup(repo => repo.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _categoryRepoMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        // Act
        await _categoryService.UpdateAsync(categoryId, request);

        // Assert
        Assert.Equal(request.Name, category.Name);
        Assert.Equal(request.Slug, category.Slug);
        Assert.Equal(request.Description, category.Description);
        Assert.Equal(request.Status, category.Status);
        Assert.NotNull(category.UpdatedAt);

        _categoryRepoMock.Verify(repo => repo.IsSlugUniqueAsync(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Never);
        _categoryRepoMock.Verify(repo => repo.UpdateAsync(category), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenCategoryExistsAndSlugChangedToUniqueSlug_ShouldUpdateCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var category = new Category
        {
            Id = categoryId,
            Name = "Old Name",
            Slug = "old-slug",
            Description = "Old description",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var request = new UpdateCategoryRequest
        {
            Name = "New Name",
            Slug = "new-slug",
            Description = "New description",
            Status = CategoryStatus.Active
        };

        _categoryRepoMock
            .Setup(repo => repo.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _categoryRepoMock
            .Setup(repo => repo.IsSlugUniqueAsync(request.Slug, categoryId))
            .ReturnsAsync(true);

        _categoryRepoMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        // Act
        await _categoryService.UpdateAsync(categoryId, request);

        // Assert
        Assert.Equal(request.Name, category.Name);
        Assert.Equal(request.Slug, category.Slug);
        Assert.Equal(request.Description, category.Description);
        Assert.Equal(request.Status, category.Status);
        Assert.NotNull(category.UpdatedAt);

        _categoryRepoMock.Verify(repo => repo.IsSlugUniqueAsync(request.Slug, categoryId), Times.Once);
        _categoryRepoMock.Verify(repo => repo.UpdateAsync(category), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenCategoryDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var request = new UpdateCategoryRequest
        {
            Name = "Laptops",
            Slug = "laptops",
            Description = "Laptop category",
            Status = CategoryStatus.Active
        };

        _categoryRepoMock
            .Setup(repo => repo.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _categoryService.UpdateAsync(categoryId, request));

        // Assert
        Assert.Equal($"Category id '{categoryId}' not found.", exception.Message);

        _categoryRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenSlugChangedToExistingSlug_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var category = new Category
        {
            Id = categoryId,
            Name = "Old Category",
            Slug = "old-category",
            Description = "Old description",
            Status = CategoryStatus.Active
        };

        var request = new UpdateCategoryRequest
        {
            Name = "Laptops",
            Slug = "laptops",
            Description = "Laptop category",
            Status = CategoryStatus.Active
        };

        _categoryRepoMock
            .Setup(repo => repo.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _categoryRepoMock
            .Setup(repo => repo.IsSlugUniqueAsync(request.Slug, categoryId))
            .ReturnsAsync(false);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _categoryService.UpdateAsync(categoryId, request));

        // Assert
        Assert.Equal($"Category slug '{request.Slug}' already exists.", exception.Message);

        _categoryRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryExists_ShouldDeleteCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var category = new Category
        {
            Id = categoryId,
            Name = "Laptops",
            Slug = "laptops",
            Description = "Laptop category",
            Status = CategoryStatus.Active
        };

        _categoryRepoMock
            .Setup(repo => repo.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _categoryRepoMock
            .Setup(repo => repo.DeleteAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        // Act
        await _categoryService.DeleteAsync(categoryId);

        // Assert
        _categoryRepoMock.Verify(repo => repo.DeleteAsync(category), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _categoryRepoMock
            .Setup(repo => repo.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _categoryService.DeleteAsync(categoryId));

        // Assert
        Assert.Equal($"Category id '{categoryId}' not found.", exception.Message);

        _categoryRepoMock.Verify(repo => repo.DeleteAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public void MapToCategoryDetailDto_ShouldMapCategoryToCategoryDetailDto()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Laptops",
            Slug = "laptops",
            Description = "Laptop category",
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = CategoryService.MapToCategoryDetailDto(category);

        // Assert
        Assert.Equal(category.Id, result.Id);
        Assert.Equal(category.Name, result.Name);
        Assert.Equal(category.Slug, result.Slug);
        Assert.Equal(category.Description, result.Description);
        Assert.Equal(category.Status, result.Status);
        Assert.Equal(category.CreatedAt, result.CreatedAt);
        Assert.Equal(category.UpdatedAt, result.UpdatedAt);
    }
}
