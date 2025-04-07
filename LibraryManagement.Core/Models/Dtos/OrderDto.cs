using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Models.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public UserDto User { get; set; } = null!;
        public BookDto Book { get; set; } = null!;
    }

    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }
}
