using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Brand;

namespace RetailCore.UnitTests.Services;

public class BrandServiceTests
{
    private readonly Mock<IBrandRepository> _brandRepoMock;
    private readonly BrandService _brandService;

    public BrandServiceTests()
    {
        _brandRepoMock = new Mock<IBrandRepository>();
        _brandService = new BrandService(_brandRepoMock.Object);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedBrandSummaryDtos()
    {
        // Arrange
        var request = new GetBrandsRequest
        {
            Keyword = "apple",
            Status = BrandStatus.Active,
            PageNumber = 1,
            PageSize = 10
        };

        var brands = new List<Brand>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Apple",
                Slug = "apple",
                Description = "Apple products",
                Status = BrandStatus.Active
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Apple Accessories",
                Slug = "apple-accessories",
                Description = "Accessories",
                Status = BrandStatus.Active
            }
        };

        _brandRepoMock
            .Setup(repo => repo.GetPagedAsync(
                request.Keyword,
                request.Status,
                request.PageNumber,
                request.PageSize))
            .ReturnsAsync((brands, brands.Count));

        // Act
        var result = await _brandService.GetPagedAsync(request);

        // Assert
        Assert.Equal(request.PageNumber, result.PageNumber);
        Assert.Equal(request.PageSize, result.PageSize);
        Assert.Equal(brands.Count, result.TotalCount);

        var items = result.Items.ToList();

        Assert.Equal(2, items.Count);
        Assert.Equal(brands[0].Id, items[0].Id);
        Assert.Equal("Apple", items[0].Name);
        Assert.Equal("apple", items[0].Slug);

        Assert.Equal(brands[1].Id, items[1].Id);
        Assert.Equal("Apple Accessories", items[1].Name);
        Assert.Equal("apple-accessories", items[1].Slug);
    }

    [Fact]
    public async Task GetPagedForManagementAsync_ShouldReturnPagedBrandDetailDtos()
    {
        // Arrange
        var request = new GetBrandsRequest
        {
            Keyword = null,
            Status = BrandStatus.Active,
            PageNumber = 1,
            PageSize = 10
        };

        var createdAt = DateTime.UtcNow;

        var brands = new List<Brand>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Samsung",
                Slug = "samsung",
                Description = "Samsung products",
                Status = BrandStatus.Active,
                CreatedAt = createdAt
            }
        };

        _brandRepoMock
            .Setup(repo => repo.GetPagedAsync(
                request.Keyword,
                request.Status,
                request.PageNumber,
                request.PageSize))
            .ReturnsAsync((brands, brands.Count));

        // Act
        var result = await _brandService.GetPagedForManagementAsync(request);

        // Assert
        Assert.Equal(request.PageNumber, result.PageNumber);
        Assert.Equal(request.PageSize, result.PageSize);
        Assert.Equal(brands.Count, result.TotalCount);

        var item = Assert.Single(result.Items);

        Assert.Equal(brands[0].Id, item.Id);
        Assert.Equal("Samsung", item.Name);
        Assert.Equal("samsung", item.Slug);
        Assert.Equal("Samsung products", item.Description);
        Assert.Equal(BrandStatus.Active, item.Status);
        Assert.Equal(createdAt, item.CreatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBrandExists_ShouldReturnBrandDetailDto()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        var brand = new Brand
        {
            Id = brandId,
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _brandRepoMock
            .Setup(repo => repo.GetByIdAsync(brandId))
            .ReturnsAsync(brand);

        // Act
        var result = await _brandService.GetByIdAsync(brandId);

        // Assert
        Assert.Equal(brand.Id, result.Id);
        Assert.Equal(brand.Name, result.Name);
        Assert.Equal(brand.Slug, result.Slug);
        Assert.Equal(brand.Description, result.Description);
        Assert.Equal(brand.Status, result.Status);
        Assert.Equal(brand.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBrandDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        _brandRepoMock
            .Setup(repo => repo.GetByIdAsync(brandId))
            .ReturnsAsync((Brand?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        {
            return _brandService.GetByIdAsync(brandId);
        });

        // Assert
        Assert.Equal($"Brand id '{brandId}' not found.", exception.Message);
    }

    [Fact]
    public async Task GetBySlugAsync_WhenBrandExists_ShouldReturnBrandDetailDto()
    {
        // Arrange
        var slug = "apple";

        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Apple",
            Slug = slug,
            Description = "Apple products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _brandRepoMock
            .Setup(repo => repo.GetBySlugAsync(slug))
            .ReturnsAsync(brand);

        // Act
        var result = await _brandService.GetBySlugAsync(slug);

        // Assert
        Assert.Equal(brand.Id, result.Id);
        Assert.Equal(brand.Name, result.Name);
        Assert.Equal(brand.Slug, result.Slug);
        Assert.Equal(brand.Description, result.Description);
        Assert.Equal(brand.Status, result.Status);
    }

    [Fact]
    public async Task GetBySlugAsync_WhenBrandDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var slug = "unknown-brand";

        _brandRepoMock
            .Setup(repo => repo.GetBySlugAsync(slug))
            .ReturnsAsync((Brand?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _brandService.GetBySlugAsync(slug));

        // Assert
        Assert.Equal($"Brand slug '{slug}' not found.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenSlugIsUnique_ShouldCreateBrandAndReturnId()
    {
        // Arrange
        var request = new CreateBrandRequest
        {
            Name = "Sony",
            Slug = "sony",
            Description = "Sony products"
        };

        Brand? createdBrand = null;

        _brandRepoMock
            .Setup(repo => repo.IsSlugUniqueAsync(request.Slug, null))
            .ReturnsAsync(true);

        _brandRepoMock
            .Setup(repo => repo.AddAsync(It.IsAny<Brand>()))
            .Callback<Brand>(brand => createdBrand = brand)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _brandService.CreateAsync(request);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        Assert.NotNull(createdBrand);

        Assert.Equal(result, createdBrand!.Id);
        Assert.Equal(request.Name, createdBrand.Name);
        Assert.Equal(request.Slug, createdBrand.Slug);
        Assert.Equal(request.Description, createdBrand.Description);
        Assert.Equal(BrandStatus.Active, createdBrand.Status);
        Assert.NotEqual(default, createdBrand.CreatedAt);

        _brandRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Brand>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenSlugAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new CreateBrandRequest
        {
            Name = "Apple",
            Slug = "apple",
            Description = "Duplicate brand"
        };

        _brandRepoMock
            .Setup(repo => repo.IsSlugUniqueAsync(request.Slug, null))
            .ReturnsAsync(false);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _brandService.CreateAsync(request));

        // Assert
        Assert.Equal($"Brand slug '{request.Slug}' already exists.", exception.Message);

        _brandRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Brand>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenBrandExistsAndSlugUnchanged_ShouldUpdateBrand()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        var brand = new Brand
        {
            Id = brandId,
            Name = "Old Name",
            Slug = "apple",
            Description = "Old description",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var request = new UpdateBrandRequest
        {
            Name = "Apple",
            Slug = "apple",
            Description = "Updated description",
            Status = BrandStatus.Inactive
        };

        _brandRepoMock
            .Setup(repo => repo.GetByIdAsync(brandId))
            .ReturnsAsync(brand);

        _brandRepoMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Brand>()))
            .Returns(Task.CompletedTask);

        // Act
        await _brandService.UpdateAsync(brandId, request);

        // Assert
        Assert.Equal(request.Name, brand.Name);
        Assert.Equal(request.Slug, brand.Slug);
        Assert.Equal(request.Description, brand.Description);
        Assert.Equal(request.Status, brand.Status);
        Assert.NotNull(brand.UpdatedAt);

        _brandRepoMock.Verify(repo => repo.IsSlugUniqueAsync(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Never);
        _brandRepoMock.Verify(repo => repo.UpdateAsync(brand), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenBrandExistsAndSlugChangedToUniqueSlug_ShouldUpdateBrand()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        var brand = new Brand
        {
            Id = brandId,
            Name = "Old Name",
            Slug = "old-slug",
            Description = "Old description",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var request = new UpdateBrandRequest
        {
            Name = "New Name",
            Slug = "new-slug",
            Description = "New description",
            Status = BrandStatus.Active
        };

        _brandRepoMock
            .Setup(repo => repo.GetByIdAsync(brandId))
            .ReturnsAsync(brand);

        _brandRepoMock
            .Setup(repo => repo.IsSlugUniqueAsync(request.Slug, brandId))
            .ReturnsAsync(true);

        _brandRepoMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Brand>()))
            .Returns(Task.CompletedTask);

        // Act
        await _brandService.UpdateAsync(brandId, request);

        // Assert
        Assert.Equal(request.Name, brand.Name);
        Assert.Equal(request.Slug, brand.Slug);
        Assert.Equal(request.Description, brand.Description);
        Assert.Equal(request.Status, brand.Status);
        Assert.NotNull(brand.UpdatedAt);

        _brandRepoMock.Verify(repo => repo.IsSlugUniqueAsync(request.Slug, brandId), Times.Once);
        _brandRepoMock.Verify(repo => repo.UpdateAsync(brand), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenBrandDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        var request = new UpdateBrandRequest
        {
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active
        };

        _brandRepoMock
            .Setup(repo => repo.GetByIdAsync(brandId))
            .ReturnsAsync((Brand?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _brandService.UpdateAsync(brandId, request));

        // Assert
        Assert.Equal($"Brand id '{brandId}' not found.", exception.Message);

        _brandRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Brand>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenSlugChangedToExistingSlug_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        var brand = new Brand
        {
            Id = brandId,
            Name = "Old Brand",
            Slug = "old-brand",
            Description = "Old description",
            Status = BrandStatus.Active
        };

        var request = new UpdateBrandRequest
        {
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active
        };

        _brandRepoMock
            .Setup(repo => repo.GetByIdAsync(brandId))
            .ReturnsAsync(brand);

        _brandRepoMock
            .Setup(repo => repo.IsSlugUniqueAsync(request.Slug, brandId))
            .ReturnsAsync(false);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _brandService.UpdateAsync(brandId, request));

        // Assert
        Assert.Equal($"Brand slug '{request.Slug}' already exists.", exception.Message);

        _brandRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Brand>()), Times.Never);
    }

    [Fact]
    public async Task UpdateBrandStatusAsync_WhenBrandExists_ShouldUpdateStatus()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        var brand = new Brand
        {
            Id = brandId,
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active
        };

        _brandRepoMock
            .Setup(repo => repo.GetByIdAsync(brandId))
            .ReturnsAsync(brand);

        _brandRepoMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Brand>()))
            .Returns(Task.CompletedTask);

        // Act
        await _brandService.UpdateBrandStatusAsync(brandId, BrandStatus.Inactive);

        // Assert
        Assert.Equal(BrandStatus.Inactive, brand.Status);
        Assert.NotNull(brand.UpdatedAt);

        _brandRepoMock.Verify(repo => repo.UpdateAsync(brand), Times.Once);
    }

    [Fact]
    public async Task UpdateBrandStatusAsync_WhenBrandDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        _brandRepoMock
            .Setup(repo => repo.GetByIdAsync(brandId))
            .ReturnsAsync((Brand?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _brandService.UpdateBrandStatusAsync(brandId, BrandStatus.Inactive));

        // Assert
        Assert.Equal($"Brand id '{brandId}' not found.", exception.Message);

        _brandRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Brand>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenBrandExists_ShouldDeleteBrand()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        var brand = new Brand
        {
            Id = brandId,
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active
        };

        _brandRepoMock
            .Setup(repo => repo.GetByIdAsync(brandId))
            .ReturnsAsync(brand);

        _brandRepoMock
            .Setup(repo => repo.DeleteAsync(It.IsAny<Brand>()))
            .Returns(Task.CompletedTask);

        // Act
        await _brandService.DeleteAsync(brandId);

        // Assert
        _brandRepoMock.Verify(repo => repo.DeleteAsync(brand), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenBrandDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        _brandRepoMock
            .Setup(repo => repo.GetByIdAsync(brandId))
            .ReturnsAsync((Brand?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _brandService.DeleteAsync(brandId));

        // Assert
        Assert.Equal($"Brand id '{brandId}' not found.", exception.Message);

        _brandRepoMock.Verify(repo => repo.DeleteAsync(It.IsAny<Brand>()), Times.Never);
    }

    [Fact]
    public void MapToBrandDetailDto_ShouldMapBrandToBrandDetailDto()
    {
        // Arrange
        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Apple",
            Slug = "apple",
            Description = "Apple products",
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = BrandService.MapToBrandDetailDto(brand);

        // Assert
        Assert.Equal(brand.Id, result.Id);
        Assert.Equal(brand.Name, result.Name);
        Assert.Equal(brand.Slug, result.Slug);
        Assert.Equal(brand.Description, result.Description);
        Assert.Equal(brand.Status, result.Status);
        Assert.Equal(brand.CreatedAt, result.CreatedAt);
        Assert.Equal(brand.UpdatedAt, result.UpdatedAt);
    }
}
