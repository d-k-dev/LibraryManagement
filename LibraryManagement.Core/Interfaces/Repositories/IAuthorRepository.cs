using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Interfaces.Repositories
{
    public interface IAuthorRepository : IGenericRepository<Author>
    {
        Task<IReadOnlyList<Author>> GetAuthorsByBookIdAsync(int bookId);
    }
}