namespace LibraryManagement.Core.Entities
{
    public enum OrderStatus
    {
        Pending,
        Approved,
        Borrowed,
        Returned,
        Cancelled,
        Overdue
    }

    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
    }
}