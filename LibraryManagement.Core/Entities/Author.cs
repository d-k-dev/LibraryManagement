﻿namespace LibraryManagement.Core.Entities
{
    public class Author : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string Biography { get; set; } = string.Empty;
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}