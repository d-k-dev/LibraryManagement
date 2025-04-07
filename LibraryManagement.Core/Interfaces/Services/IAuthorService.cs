using LibraryManagement.Core.Models.Dtos;

namespace LibraryManagement.Core.Interfaces.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync();
        Task<AuthorDto?> GetAuthorByIdAsync(int id);
        Task<IEnumerable<AuthorDto>> GetAuthorsByBookAsync(int bookId);
        Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto authorDto);
        Task UpdateAuthorAsync(int id, UpdateAuthorDto authorDto);
        Task DeleteAuthorAsync(int id);
    }
}