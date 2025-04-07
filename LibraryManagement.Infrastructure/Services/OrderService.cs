using AutoMapper;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces.Services;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Models.Dtos;
using LibraryManagement.Core.Models;

namespace LibraryManagement.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetAsync(
                null,
                null,
                includes: new List<System.Linq.Expressions.Expression<Func<Order, object>>>
                {
                    o => o.User,
                    o => o.Book
                });

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
                return null;

                        await _unitOfWork.Orders.GetAsync(
                o => o.Id == id,
                null,
                includes: new List<System.Linq.Expressions.Expression<Func<Order, object>>>
                {
                    o => o.User,
                    o => o.Book
                },
                false);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(int userId)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetOverdueOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetOverdueOrdersAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
                        var user = await _unitOfWork.Users.GetByIdAsync(createOrderDto.UserId);
            if (user == null)
                throw new ApiException($"User with ID {createOrderDto.UserId} not found", 404);

                        var book = await _unitOfWork.Books.GetByIdAsync(createOrderDto.BookId);
            if (book == null)
                throw new ApiException($"Book with ID {createOrderDto.BookId} not found", 404);

                        if (book.AvailableCopies <= 0)
                throw new ApiException("Book is not available", 400);

                        var order = new Order
            {
                UserId = createOrderDto.UserId,
                BookId = createOrderDto.BookId,
                OrderDate = DateTime.UtcNow,
                DueDate = createOrderDto.DueDate,
                Status = OrderStatus.Pending
            };

            await _unitOfWork.Orders.AddAsync(order);

                        book.AvailableCopies--;
            await _unitOfWork.CompleteAsync();
            return await GetOrderByIdAsync(order.Id) ?? throw new ApiException("Failed to retrieve created order", 500);
        }

        public async Task UpdateOrderStatusAsync(int id, UpdateOrderStatusDto updateOrderStatusDto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
                throw new ApiException($"Order with ID {id} not found", 404);

                        ValidateStatusTransition(order.Status, updateOrderStatusDto.Status);

            order.Status = updateOrderStatusDto.Status;
            await _unitOfWork.CompleteAsync();
        }

        public async Task ReturnBookAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new ApiException($"Order with ID {orderId} not found", 404);

            if (order.Status != OrderStatus.Borrowed)
                throw new ApiException("Only borrowed books can be returned", 400);

                        order.Status = OrderStatus.Returned;
            order.ReturnDate = DateTime.UtcNow;

                        var book = await _unitOfWork.Books.GetByIdAsync(order.BookId);
            if (book != null)
            {
                book.AvailableCopies++;
            }
            await _unitOfWork.CompleteAsync();
        }

        private void ValidateStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            bool isValid = false;

            switch (currentStatus)
            {
                case OrderStatus.Pending:
                    isValid = newStatus == OrderStatus.Approved || newStatus == OrderStatus.Cancelled;
                    break;
                case OrderStatus.Approved:
                    isValid = newStatus == OrderStatus.Borrowed || newStatus == OrderStatus.Cancelled;
                    break;
                case OrderStatus.Borrowed:
                    isValid = newStatus == OrderStatus.Returned || newStatus == OrderStatus.Overdue;
                    break;
                case OrderStatus.Overdue:
                    isValid = newStatus == OrderStatus.Returned;
                    break;
                                case OrderStatus.Returned:
                case OrderStatus.Cancelled:
                    isValid = false;
                    break;
            }
            if (!isValid)
                throw new ApiException($"Invalid status transition from {currentStatus} to {newStatus}", 400);
        }
    }
}