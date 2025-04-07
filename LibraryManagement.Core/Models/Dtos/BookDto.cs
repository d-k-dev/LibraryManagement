namespace LibraryManagement.Core.Models.Dtos
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public int Copies { get; set; }
        public int AvailableCopies { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public CategoryDto Category { get; set; } = null!;
        public ICollection<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
    }

    public class CreateBookDto
    {
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public int Copies { get; set; }
        public int CategoryId { get; set; }
        public List<int> AuthorIds { get; set; } = new List<int>();
    }

    public class UpdateBookDto
    {
        public string? Title { get; set; }
        public string? ISBN { get; set; }
        public int? PublicationYear { get; set; }
        public int? Copies { get; set; }
        public int? CategoryId { get; set; }
        public List<int>? AuthorIds { get; set; }
    }
}
