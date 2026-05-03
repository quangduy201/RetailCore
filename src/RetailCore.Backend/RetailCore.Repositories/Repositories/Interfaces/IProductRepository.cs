using RetailCore.Repositories.Entities;
using RetailCore.Shared.Enums;

namespace RetailCore.Repositories.Repositories.Interfaces;

public interface IProductRepository
{
    Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
        string? keyword,
        Guid? brandId,
        Guid? categoryId,
        ProductStatus? status,
        int pageNumber,
        int pageSize);

    Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedForManagementAsync(
        string? keyword,
        ProductStatus? status,
        int pageNumber,
        int pageSize);

    Task<Product?> GetByIdAsync(Guid id);
    Task<Product?> GetBySlugAsync(string slug);
    Task<Product?> GetByIdWithDetailsAsync(Guid id);
    Task<Product?> GetBySlugWithDetailsAsync(string slug);
    Task<bool> IsSlugUniqueAsync(string slug, Guid? excludeId = null);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
}
