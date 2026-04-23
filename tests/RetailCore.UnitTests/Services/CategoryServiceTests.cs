using Moq;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Services.Implementations;
using RetailCore.Shared.DTOs.Category;

namespace RetailCore.UnitTests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _repoMock;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _repoMock = new Mock<ICategoryRepository>();
        _service = new CategoryService(_repoMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnsCategories()
    {
        _repoMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Category>
            {
                new() { Id = Guid.NewGuid(), Name = "A" }
            });

        var result = await _service.GetAllAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCategory_WhenExists()
    {
        var id = Guid.NewGuid();

        _repoMock.Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(new Category { Id = id, Name = "Test" });

        var result = await _service.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddCategory()
    {
        var dto = new CategoryCreateDto
        {
            Name = "New Category"
        };

        await _service.CreateAsync(dto);

        _repoMock.Verify(x => x.AddAsync(It.IsAny<Category>()), Times.Once);
        _repoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCategory()
    {
        var id = Guid.NewGuid();

        _repoMock.Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(new Category { Id = id });

        await _service.DeleteAsync(id);

        _repoMock.Verify(x => x.Delete(It.IsAny<Category>()), Times.Once);
    }
}
