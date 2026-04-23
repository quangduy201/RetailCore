using Moq;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Services.Implementations;
using RetailCore.Shared.DTOs.Product;

namespace RetailCore.UnitTests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repoMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repoMock = new Mock<IProductRepository>();
        _service = new ProductService(_repoMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnProducts()
    {
        _repoMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Phone",
                    Category = new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Electronics"
                    }
                }
            });

        var result = await _service.GetAllAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct()
    {
        var id = Guid.NewGuid();

        _repoMock.Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(new Product
            {
                Id = id,
                Name = "Laptop",
                Category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Electronics"
                }
            });

        var result = await _service.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("Laptop", result.Name);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddProduct()
    {
        var dto = new ProductCreateDto
        {
            Name = "New Product",
            CategoryId = Guid.NewGuid()
        };

        await _service.CreateAsync(dto);

        _repoMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
        _repoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProduct()
    {
        var id = Guid.NewGuid();

        _repoMock.Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(new Product { Id = id });

        await _service.DeleteAsync(id);

        _repoMock.Verify(x => x.Delete(It.IsAny<Product>()), Times.Once);
    }
}