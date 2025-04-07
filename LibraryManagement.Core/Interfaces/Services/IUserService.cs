using LibraryManagement.Core.Models.Dtos;

namespace LibraryManagement.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto> CreateUserAsync(CreateUserDto userDto);
        Task UpdateUserAsync(int id, UpdateUserDto userDto);
        Task DeleteUserAsync(int id);
    }
}