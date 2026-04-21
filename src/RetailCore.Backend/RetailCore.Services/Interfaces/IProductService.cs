using RetailCore.Shared.DTOs.Product;

namespace RetailCore.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetAllAsync();
    Task<ProductResponseDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(ProductCreateDto dto);
    Task<bool> UpdateAsync(Guid id, ProductUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
}
