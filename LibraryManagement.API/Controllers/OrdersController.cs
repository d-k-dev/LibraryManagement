using LibraryManagement.Core.Interfaces.Services;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orders));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(ApiResponse<OrderDto>.ErrorResponse($"Order with ID {id} not found"));

                        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userId != order.User.Id.ToString() && userRole != "Admin" && userRole != "Librarian")
                return Forbid();

            return Ok(ApiResponse<OrderDto>.SuccessResponse(order));
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUser(int userId)
        {
                        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserId != userId.ToString() && userRole != "Admin" && userRole != "Librarian")
                return Forbid();

            var orders = await _orderService.GetOrdersByUserAsync(userId);
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orders));
        }

        [HttpGet("overdue")]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> GetOverdueOrders()
        {
            var orders = await _orderService.GetOverdueOrdersAsync();
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orders));
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(ApiResponse<IEnumerable<OrderDto>>.ErrorResponse("Not authenticated"));

            var orders = await _orderService.GetOrdersByUserAsync(int.Parse(userId));
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orders));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != "Admin" && userRole != "Librarian" && userId != createOrderDto.UserId.ToString())
                    return Forbid();

                var order = await _orderService.CreateOrderAsync(createOrderDto);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, ApiResponse<OrderDto>.SuccessResponse(order, "Order created successfully"));
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<OrderDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto updateOrderStatusDto)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(id, updateOrderStatusDto);
                return Ok(ApiResponse<object>.SuccessResponse(new { }, "Order status updated successfully"));
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}/return")]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            try
            {
                await _orderService.ReturnBookAsync(id);
                return Ok(ApiResponse<object>.SuccessResponse(new { }, "Book returned successfully"));
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}