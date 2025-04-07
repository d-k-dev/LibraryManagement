using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces.Repositories;
using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(LibraryDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Book)
                .ThenInclude(b => b.Category)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Order>> GetOverdueOrdersAsync()
        {
            return await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Book)
                .Where(o => o.Status == OrderStatus.Borrowed && o.DueDate < DateTime.UtcNow && !o.ReturnDate.HasValue)
                .ToListAsync();
        }
    }
}
