using RetailCore.Repositories.Entities;

namespace RetailCore.Repositories.Repositories.Interfaces;

public interface IUserRepository
{
    Task<List<AppUser>> GetCustomersAsync();
    Task<AppUser?> GetByIdAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
}
