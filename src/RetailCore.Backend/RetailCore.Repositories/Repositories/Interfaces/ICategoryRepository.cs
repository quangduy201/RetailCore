using RetailCore.Repositories.Entities;
using RetailCore.Shared.Enums;

namespace RetailCore.Repositories.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<(IEnumerable<Category> Items, int TotalCount)> GetPagedAsync(
        string? keyword,
        CategoryStatus? status,
        int pageNumber,
        int pageSize);
    Task<Category?> GetByIdAsync(Guid id);
    Task<Category?> GetBySlugAsync(string slug);
    Task<bool> IsSlugUniqueAsync(string slug, Guid? excludeId = null);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Category category);
}
