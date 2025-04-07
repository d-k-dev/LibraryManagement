using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Interfaces.Repositories
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<IReadOnlyList<Book>> GetBooksByAuthorIdAsync(int authorId);
        Task<IReadOnlyList<Book>> GetBooksByCategoryIdAsync(int categoryId);
        Task<IReadOnlyList<Book>> GetAvailableBooksAsync();
        Task AddBookAuthorAsync(BookAuthor bookAuthor);
        Task RemoveBookAuthorAsync(BookAuthor bookAuthor);
        Task<IReadOnlyList<BookAuthor>> GetBookAuthorsByBookIdAsync(int bookId);
        Task RemoveBookAuthorsByBookIdAsync(int bookId);
    }
}