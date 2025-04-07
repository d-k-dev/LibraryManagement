using AutoMapper;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces.Services;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Models.Dtos;
using LibraryManagement.Core.Models;

namespace LibraryManagement.Infrastructure.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await _unitOfWork.Books.GetAsync(
                null,
                null,
                "Category",
                true);

            var result = new List<BookDto>();

            foreach (var book in books)
            {
                var bookDto = _mapper.Map<BookDto>(book);
                var authors = await _unitOfWork.Authors.GetAuthorsByBookIdAsync(book.Id);
                bookDto.Authors = _mapper.Map<List<AuthorDto>>(authors);
                result.Add(bookDto);
            }

            return result;
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null)
                return null;

                        await _unitOfWork.Books.GetAsync(
                b => b.Id == id,
                null,
                "Category",
                false);

            var bookDto = _mapper.Map<BookDto>(book);

                        var authors = await _unitOfWork.Authors.GetAuthorsByBookIdAsync(id);
            bookDto.Authors = _mapper.Map<List<AuthorDto>>(authors);

            return bookDto;
        }

        public async Task<IEnumerable<BookDto>> GetBooksByAuthorAsync(int authorId)
        {
            var books = await _unitOfWork.Books.GetBooksByAuthorIdAsync(authorId);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(int categoryId)
        {
            var books = await _unitOfWork.Books.GetBooksByCategoryIdAsync(categoryId);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<IEnumerable<BookDto>> GetAvailableBooksAsync()
        {
            var books = await _unitOfWork.Books.GetAvailableBooksAsync();
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
        {
                        var category = await _unitOfWork.Categories.GetByIdAsync(createBookDto.CategoryId);
            if (category == null)
                throw new ApiException($"Category with ID {createBookDto.CategoryId} not found", 404);

                        foreach (var authorId in createBookDto.AuthorIds)
            {
                var author = await _unitOfWork.Authors.GetByIdAsync(authorId);
                if (author == null)
                    throw new ApiException($"Author with ID {authorId} not found", 404);
            }

                        var book = _mapper.Map<Book>(createBookDto);
            book.AvailableCopies = book.Copies; 

            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.CompleteAsync();

                        foreach (var authorId in createBookDto.AuthorIds)
            {
                var bookAuthor = new BookAuthor
                {
                    BookId = book.Id,
                    AuthorId = authorId
                };

                await _unitOfWork.Books.AddBookAuthorAsync(bookAuthor);
            }

            await _unitOfWork.CompleteAsync();

                        var bookDto = _mapper.Map<BookDto>(book);

                        bookDto.Category = _mapper.Map<CategoryDto>(category);

                        var authors = await _unitOfWork.Authors.GetAuthorsByBookIdAsync(book.Id);
            bookDto.Authors = _mapper.Map<List<AuthorDto>>(authors);

            return bookDto;
        }

        public async Task UpdateBookAsync(int id, UpdateBookDto updateBookDto)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null)
                throw new ApiException($"Book with ID {id} not found", 404);

                        if (updateBookDto.CategoryId.HasValue)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(updateBookDto.CategoryId.Value);
                if (category == null)
                    throw new ApiException($"Category with ID {updateBookDto.CategoryId} not found", 404);

                book.CategoryId = updateBookDto.CategoryId.Value;
            }

                        if (!string.IsNullOrEmpty(updateBookDto.Title))
                book.Title = updateBookDto.Title;

            if (!string.IsNullOrEmpty(updateBookDto.ISBN))
                book.ISBN = updateBookDto.ISBN;

            if (updateBookDto.PublicationYear.HasValue)
                book.PublicationYear = updateBookDto.PublicationYear.Value;

            if (updateBookDto.Copies.HasValue)
            {
                                var difference = updateBookDto.Copies.Value - book.Copies;
                book.Copies = updateBookDto.Copies.Value;
                book.AvailableCopies = Math.Max(0, book.AvailableCopies + difference);
            }

                        if (updateBookDto.AuthorIds != null && updateBookDto.AuthorIds.Any())
            {
                                foreach (var authorId in updateBookDto.AuthorIds)
                {
                    var author = await _unitOfWork.Authors.GetByIdAsync(authorId);
                    if (author == null)
                        throw new ApiException($"Author with ID {authorId} not found", 404);
                }

                                await _unitOfWork.Books.RemoveBookAuthorsByBookIdAsync(id);

                                foreach (var authorId in updateBookDto.AuthorIds)
                {
                    var bookAuthor = new BookAuthor
                    {
                        BookId = book.Id,
                        AuthorId = authorId
                    };

                    await _unitOfWork.Books.AddBookAuthorAsync(bookAuthor);
                }
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null)
                throw new ApiException($"Book with ID {id} not found", 404);

                        var activeOrders = await _unitOfWork.Orders.GetAsync(
                o => o.BookId == id && (o.Status == OrderStatus.Pending || o.Status == OrderStatus.Approved || o.Status == OrderStatus.Borrowed)
            );

                        if (activeOrders != null && activeOrders.Any())
            {
                throw new ApiException("Cannot delete book with active orders", 400);
            }

                        await _unitOfWork.Books.RemoveBookAuthorsByBookIdAsync(id);

                        await _unitOfWork.Books.DeleteAsync(book);
            await _unitOfWork.CompleteAsync();
        }
    }
}