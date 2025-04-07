namespace LibraryManagement.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Repositories.IBookRepository Books { get; }
        Repositories.IAuthorRepository Authors { get; }
        Repositories.ICategoryRepository Categories { get; }
        Repositories.IUserRepository Users { get; }
        Repositories.IOrderRepository Orders { get; }

        Task<int> CompleteAsync();
    }
}