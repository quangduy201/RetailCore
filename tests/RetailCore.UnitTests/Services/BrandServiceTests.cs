using Moq;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Services.Implementations;
using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Brand;

namespace RetailCore.UnitTests.Services;

public class BrandServiceTests
{
    private readonly Mock<IBrandRepository> _mockRepo;
    private readonly BrandService _service;

    public BrandServiceTests()
    {
        _mockRepo = new Mock<IBrandRepository>();
        _service = new BrandService(_mockRepo.Object);
    }

    #region GetPagedAsync Tests

    [Fact]
    public async Task GetPagedAsync_WithValidRequest_ReturnsPagedResult()
    {
        // Arrange
        var request = new GetBrandsRequest { PageNumber = 1, PageSize = 10 };
        var brands = new List<Brand>
        {
            new() { Id = Guid.NewGuid(), Name = "Brand 1", Slug = "brand-1", Status = BrandStatus.Active },
            new() { Id = Guid.NewGuid(), Name = "Brand 2", Slug = "brand-2", Status = BrandStatus.Active }
        };

        _mockRepo.Setup(r => r.GetPagedAsync(It.IsAny<string>(), It.IsAny<BrandStatus?>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((brands, 2));

        // Act
        var result = await _service.GetPagedAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task GetPagedAsync_WithEmptyResult_ReturnsEmptyCollection()
    {
        // Arrange
        var request = new GetBrandsRequest { PageNumber = 1, PageSize = 10 };
        _mockRepo.Setup(r => r.GetPagedAsync(It.IsAny<string>(), It.IsAny<BrandStatus?>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((new List<Brand>(), 0));

        // Act
        var result = await _service.GetPagedAsync(request);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    #endregion

    #region GetPagedForManagementAsync Tests

    [Fact]
    public async Task GetPagedForManagementAsync_WithValidRequest_ReturnsPagedDetailResult()
    {
        // Arrange
        var request = new GetBrandsRequest { PageNumber = 1, PageSize = 10 };
        var brands = new List<Brand>
        {
            new() { Id = Guid.NewGuid(), Name = "Brand 1", Slug = "brand-1", Description = "Desc 1", Status = BrandStatus.Active, CreatedAt = DateTime.UtcNow }
        };

        _mockRepo.Setup(r => r.GetPagedAsync(It.IsAny<string>(), It.IsAny<BrandStatus?>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((brands, 1));

        // Act
        var result = await _service.GetPagedForManagementAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Brand 1", result.Items.First().Name);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsBrand()
    {
        // Arrange
        var id = Guid.NewGuid();
        var brand = new Brand { Id = id, Name = "Test Brand", Slug = "test-brand", Status = BrandStatus.Active };

        _mockRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(brand);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Test Brand", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Brand?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(id));
    }

    #endregion

    #region GetBySlugAsync Tests

    [Fact]
    public async Task GetBySlugAsync_WithValidSlug_ReturnsBrand()
    {
        // Arrange
        const string slug = "test-brand";
        var brand = new Brand { Id = Guid.NewGuid(), Name = "Test Brand", Slug = slug, Status = BrandStatus.Active };

        _mockRepo.Setup(r => r.GetBySlugAsync(slug))
            .ReturnsAsync(brand);

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
            .ReturnsAsync((Brand?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetBySlugAsync(slug));
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidRequest_ReturnsBrandId()
    {
        // Arrange
        var request = new CreateBrandRequest { Name = "New Brand", Slug = "new-brand", Description = "Test" };

        _mockRepo.Setup(r => r.IsSlugUniqueAsync(request.Slug))
            .ReturnsAsync(true);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Brand>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Brand>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateSlug_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new CreateBrandRequest { Name = "New Brand", Slug = "existing-slug" };

        _mockRepo.Setup(r => r.IsSlugUniqueAsync(request.Slug))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Brand>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_CreatedBrandHasActiveStatus()
    {
        // Arrange
        var request = new CreateBrandRequest { Name = "New Brand", Slug = "new-brand" };
        Brand? capturedBrand = null;

        _mockRepo.Setup(r => r.IsSlugUniqueAsync(request.Slug))
            .ReturnsAsync(true);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Brand>()))
            .Callback<Brand>(b => capturedBrand = b)
            .Returns(Task.CompletedTask);

        // Act
        await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(capturedBrand);
        Assert.Equal(BrandStatus.Active, capturedBrand.Status);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidRequest_UpdatesBrand()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingBrand = new Brand { Id = id, Name = "Old", Slug = "old-slug", Status = BrandStatus.Active };
        var request = new UpdateBrandRequest { Name = "Updated", Slug = "updated-slug", Status = BrandStatus.Inactive };

        _mockRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(existingBrand);
        _mockRepo.Setup(r => r.IsSlugUniqueAsync(request.Slug, id))
            .ReturnsAsync(true);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Brand>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(id, request);

        // Assert
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Brand>()), Times.Once);
        Assert.Equal("Updated", existingBrand.Name);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateBrandRequest { Name = "Test", Slug = "test" };

        _mockRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Brand?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(id, request));
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateSlug_ThrowsInvalidOperationException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingBrand = new Brand { Id = id, Name = "Old", Slug = "old-slug" };
        var request = new UpdateBrandRequest { Name = "Test", Slug = "new-slug" };

        _mockRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(existingBrand);
        _mockRepo.Setup(r => r.IsSlugUniqueAsync(request.Slug, id))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(id, request));
    }

    #endregion

    #region UpdateBrandStatusAsync Tests

    [Fact]
    public async Task UpdateBrandStatusAsync_WithValidId_UpdatesStatus()
    {
        // Arrange
        var id = Guid.NewGuid();
        var brand = new Brand { Id = id, Name = "Test", Status = BrandStatus.Active };

        _mockRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(brand);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Brand>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateBrandStatusAsync(id, BrandStatus.Inactive);

        // Assert
        Assert.Equal(BrandStatus.Inactive, brand.Status);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Brand>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBrandStatusAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Brand?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateBrandStatusAsync(id, BrandStatus.Active));
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesBrand()
    {
        // Arrange
        var id = Guid.NewGuid();
        var brand = new Brand { Id = id, Name = "Test" };

        _mockRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(brand);
        _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<Brand>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(id);

        // Assert
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Brand>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ThrowsException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Brand?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.DeleteAsync(id));
    }

    #endregion
}
