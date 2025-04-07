using LibraryManagement.Core.Models.Dtos;

namespace LibraryManagement.Core.Interfaces.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(int userId);
        Task<IEnumerable<OrderDto>> GetOverdueOrdersAsync();
        Task<OrderDto> CreateOrderAsync(CreateOrderDto orderDto);
        Task UpdateOrderStatusAsync(int id, UpdateOrderStatusDto orderStatusDto);
        Task ReturnBookAsync(int orderId);
    }
}