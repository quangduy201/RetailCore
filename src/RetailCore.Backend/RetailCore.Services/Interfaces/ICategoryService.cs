using RetailCore.Shared.DTOs.Category;

namespace RetailCore.Services.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetAllAsync();
    Task<CategoryResponseDto?> GetByIdAsync(Guid id);
    Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto);
    Task<bool> UpdateAsync(Guid id, CategoryUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
}
