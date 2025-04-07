using LibraryManagement.Core.Models.Dtos;

namespace LibraryManagement.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        string GenerateJwtToken(UserDto user);
    }
}