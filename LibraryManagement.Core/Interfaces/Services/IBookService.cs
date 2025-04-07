using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Models.Dtos;

namespace LibraryManagement.Core.Interfaces.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto?> GetBookByIdAsync(int id);
        Task<IEnumerable<BookDto>> GetBooksByAuthorAsync(int authorId);
        Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(int categoryId);
        Task<IEnumerable<BookDto>> GetAvailableBooksAsync();
        Task<BookDto> CreateBookAsync(CreateBookDto bookDto);
        Task UpdateBookAsync(int id, UpdateBookDto bookDto);
        Task DeleteBookAsync(int id);
    }
}