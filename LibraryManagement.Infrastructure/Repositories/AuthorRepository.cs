using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces.Repositories;
using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories
{
    public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(LibraryDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<Author>> GetAuthorsByBookIdAsync(int bookId)
        {
            return await _dbContext.Authors
                .Include(a => a.BookAuthors)
                .Where(a => a.BookAuthors.Any(ba => ba.BookId == bookId))
                .ToListAsync();
        }
    }
}