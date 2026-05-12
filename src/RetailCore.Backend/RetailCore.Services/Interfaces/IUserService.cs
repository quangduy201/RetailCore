using RetailCore.Shared.DTOs.User;

namespace RetailCore.Services.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetCustomersAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
    Task ToggleActiveStatusAsync(Guid id);
}
