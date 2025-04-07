using LibraryManagement.Core.Interfaces.Repositories;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Infrastructure.Data;

namespace LibraryManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryDbContext _dbContext;

        private IBookRepository _bookRepository = null!;
        private IAuthorRepository _authorRepository = null!;
        private ICategoryRepository _categoryRepository = null!;
        private IUserRepository _userRepository = null!;
        private IOrderRepository _orderRepository = null!;

        public UnitOfWork(LibraryDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IBookRepository Books => _bookRepository ??= new BookRepository(_dbContext);
        public IAuthorRepository Authors => _authorRepository ??= new AuthorRepository(_dbContext);
        public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(_dbContext);
        public IUserRepository Users => _userRepository ??= new UserRepository(_dbContext);
        public IOrderRepository Orders => _orderRepository ??= new OrderRepository(_dbContext);

        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}