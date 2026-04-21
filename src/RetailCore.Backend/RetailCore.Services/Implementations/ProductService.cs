using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.DTOs.Product;

namespace RetailCore.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProductResponseDto>> GetAllAsync()
    {
        var products = await _repo.GetAllAsync();

        return products.Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name,
            Price = p.Price
        }).ToList();
    }

    public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null) return null;

        return new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name,
            Price = p.Price
        };
    }

    public async Task<Guid> CreateAsync(ProductCreateDto dto)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            Price = dto.Price,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(product);
        await _repo.SaveChangesAsync();

        return product.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, ProductUpdateDto dto)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return false;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.CategoryId = dto.CategoryId;
        product.Price = dto.Price;
        product.UpdatedAt = DateTime.UtcNow;

        _repo.Update(product);
        await _repo.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return false;

        _repo.Delete(product);
        await _repo.SaveChangesAsync();

        return true;
    }
}
