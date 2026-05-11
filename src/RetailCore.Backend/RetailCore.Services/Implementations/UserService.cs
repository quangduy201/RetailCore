using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.DTOs.User;

namespace RetailCore.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<UserDto>> GetCustomersAsync()
    {
        var users = await _userRepository.GetCustomersAsync();

        return users.Select(user => new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            Roles = user.UserRoles.Select(u => u.Role.Name!).ToList()
        }).ToList();
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            return null;
        }

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            Roles = user.UserRoles.Select(u => u.Role.Name!).ToList()
        };
    }

    public async Task ToggleActiveStatusAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        user.IsActive = !user.IsActive;
        await _unitOfWork.SaveChangesAsync();
    }
}
