using Moq;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Services.Implementations;
using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Category;

namespace RetailCore.UnitTests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockRepo;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _mockRepo = new Mock<ICategoryRepository>();
        _service = new CategoryService(_mockRepo.Object);
    }

    #region GetPagedAsync Tests

    [Fact]
    public async Task GetPagedAsync_WithValidRequest_ReturnsPagedResult()
    {
        // Arrange
        var request = new GetCategoriesRequest { PageNumber = 1, PageSize = 10 };
        var categories = new List<Category>
        {
            new() { Id = Guid.NewGuid(), Name = "Cat 1", Slug = "cat-1", Status = CategoryStatus.Active },
            new() { Id = Guid.NewGuid(), Name = "Cat 2", Slug = "cat-2", Status = CategoryStatus.Active }
        };

        _mockRepo.Setup(r => r.GetPagedAsync(It.IsAny<string>(), It.IsAny<CategoryStatus?>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((categories, 2));

        // Act
        var result = await _service.GetPagedAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task GetPagedAsync_WithEmptyResult_ReturnsEmptyCollection()
    {
        // Arrange
        var request = new GetCategoriesRequest { PageNumber = 1, PageSize = 10 };
        _mockRepo.Setup(r => r.GetPagedAsync(It.IsAny<string>(), It.IsAny<CategoryStatus?>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((new List<Category>(), 0));

        // Act
        var result = await _service.GetPagedAsync(request);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    #endregion

    #region GetPagedForManagementAsync Tests

    [Fact]
    public async Task GetPagedForManagementAsync_WithValidRequest_ReturnsDetailDtos()
    {
        // Arrange
        var request = new GetCategoriesRequest { PageNumber = 1, PageSize = 10 };
        var categories = new List<Category>
        {
            new() { Id = Guid.NewGuid(), Name = "Cat 1", Slug = "cat-1", Description = "Desc 1", Status = CategoryStatus.Active, CreatedAt = DateTime.UtcNow }
        };

        _mockRepo.Setup(r => r.GetPagedAsync(It.IsAny<string>(), It.IsAny<CategoryStatus?>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((categories, 1));

        // Act
        var result = await _service.GetPagedForManagementAsync(request);

        // Assert
        Assert.Single(result.Items);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCategory()
    {
        // Arrange
        var id = Guid.NewGuid();
        var category = new Category { Id = id, Name = "Test Category", Slug = "test-cat", Status = CategoryStatus.Active };

        _mockRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(category);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Test Category", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Category?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(id));
    }

    #endregion

    #region GetBySlugAsync Tests

    [Fact]
    public async Task GetBySlugAsync_WithValidSlug_ReturnsCategory()
    {
        // Arrange
        const string slug = "test-category";
        var category = new Category { Id = Guid.NewGuid(), Name = "Test", Slug = slug, Status = CategoryStatus.Active };

        _mockRepo.Setup(r => r.GetBySlugAsync(slug))
            .ReturnsAsync(category);

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
        const string slug = "invalid-slug";
        _mockRepo.Setup(r => r.GetBySlugAsync(slug))
            .ReturnsAsync((Category?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetBySlugAsync(slug));
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidRequest_ReturnsCategoryId()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "New Cat", Slug = "new-cat", Description = "Test" };

        _mockRepo.Setup(r => r.IsSlugUniqueAsync(request.Slug))
            .ReturnsAsync(true);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateSlug_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "New Cat", Slug = "existing-slug" };

        _mockRepo.Setup(r => r.IsSlugUniqueAsync(request.Slug))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_CreatedCategoryHasActiveStatus()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "New Cat", Slug = "new-cat" };
        Category? capturedCategory = null;

        _mockRepo.Setup(r => r.IsSlugUniqueAsync(request.Slug))
            .ReturnsAsync(true);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Category>()))
            .Callback<Category>(c => capturedCategory = c)
            .Returns(Task.CompletedTask);

        // Act
        await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(capturedCategory);
        Assert.Equal(CategoryStatus.Active, capturedCategory.Status);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidRequest_UpdatesCategory()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingCategory = new Category { Id = id, Name = "Old", Slug = "old-slug", Status = CategoryStatus.Active };
        var request = new UpdateCategoryRequest { Name = "Updated", Slug = "updated-slug", Status = CategoryStatus.Inactive };

        _mockRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(existingCategory);
        _mockRepo.Setup(r => r.IsSlugUniqueAsync(request.Slug, id))
            .ReturnsAsync(true);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(id, request);

        // Assert
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Once);
        Assert.Equal("Updated", existingCategory.Name);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateCategoryRequest { Name = "Test", Slug = "test" };

        _mockRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Category?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(id, request));
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateSlug_ThrowsInvalidOperationException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingCategory = new Category { Id = id, Name = "Old", Slug = "old-slug" };
        var request = new UpdateCategoryRequest { Name = "Test", Slug = "new-slug" };

        _mockRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(existingCategory);
        _mockRepo.Setup(r => r.IsSlugUniqueAsync(request.Slug, id))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(id, request));
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesCategory()
    {
        // Arrange
        var id = Guid.NewGuid();
        var category = new Category { Id = id, Name = "Test" };

        _mockRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(category);
        _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(id);

        // Assert
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Category?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(id));
    }

    #endregion
}
