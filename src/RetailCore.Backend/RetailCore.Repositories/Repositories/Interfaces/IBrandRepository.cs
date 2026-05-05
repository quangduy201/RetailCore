using RetailCore.Repositories.Entities;
using RetailCore.Shared.Enums;

namespace RetailCore.Repositories.Repositories.Interfaces;

public interface IBrandRepository
{
    Task<(IEnumerable<Brand> Items, int TotalCount)> GetPagedAsync(
        string? keyword,
        BrandStatus? status,
        int pageNumber,
        int pageSize);
    Task<Brand?> GetByIdAsync(Guid id);
    Task<Brand?> GetBySlugAsync(string slug);
    Task<bool> IsSlugUniqueAsync(string slug, Guid? excludeId = null);
    Task AddAsync(Brand brand);
    Task UpdateAsync(Brand brand);
    Task DeleteAsync(Brand brand);
}
