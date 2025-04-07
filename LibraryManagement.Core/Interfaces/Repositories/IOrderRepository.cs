using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(int userId);
        Task<IReadOnlyList<Order>> GetOverdueOrdersAsync();
    }
}