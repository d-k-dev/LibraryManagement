using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces.Repositories;
using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(LibraryDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<Book>> GetBooksByAuthorIdAsync(int authorId)
        {
            return await _dbContext.Books
                .Include(b => b.Category)
                .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .Where(b => b.BookAuthors.Any(ba => ba.AuthorId == authorId))
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Book>> GetBooksByCategoryIdAsync(int categoryId)
        {
            return await _dbContext.Books
                .Include(b => b.Category)
                .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .Where(b => b.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Book>> GetAvailableBooksAsync()
        {
            return await _dbContext.Books
                .Include(b => b.Category)
                .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .Where(b => b.AvailableCopies > 0)
                .ToListAsync();
        }

        public async Task AddBookAuthorAsync(BookAuthor bookAuthor)
        {
            await _dbContext.BookAuthors.AddAsync(bookAuthor);
        }

        public Task RemoveBookAuthorAsync(BookAuthor bookAuthor)
        {
            _dbContext.BookAuthors.Remove(bookAuthor);
            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<BookAuthor>> GetBookAuthorsByBookIdAsync(int bookId)
        {
            return await _dbContext.BookAuthors
                .Where(ba => ba.BookId == bookId)
                .ToListAsync();
        }

        public async Task RemoveBookAuthorsByBookIdAsync(int bookId)
        {
            var bookAuthors = await _dbContext.BookAuthors
                .Where(ba => ba.BookId == bookId)
                .ToListAsync();

            _dbContext.BookAuthors.RemoveRange(bookAuthors);
        }
    }
}