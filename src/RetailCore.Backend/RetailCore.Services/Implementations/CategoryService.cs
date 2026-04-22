using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.DTOs.Category;

namespace RetailCore.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<CategoryResponseDto>> GetAllAsync()
    {
        var categories = await _repo.GetAllAsync();

        return categories.Select(c => new CategoryResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            IsActive = c.IsActive
        }).ToList();
    }

    public async Task<CategoryResponseDto?> GetByIdAsync(Guid id)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c == null) return null;

        return new CategoryResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            IsActive = c.IsActive
        };
    }

    public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(category);
        await _repo.SaveChangesAsync();

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive
        };
    }

    public async Task<bool> UpdateAsync(Guid id, CategoryUpdateDto dto)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category == null) return false;

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.IsActive = dto.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        _repo.Update(category);
        await _repo.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category == null) return false;

        _repo.Delete(category);
        await _repo.SaveChangesAsync();

        return true;
    }
}
