using LibraryManagement.Core.Interfaces.Services;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(ApiResponse<IEnumerable<BookDto>>.SuccessResponse(books));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound(ApiResponse<BookDto>.ErrorResponse($"Book with ID {id} not found"));

            return Ok(ApiResponse<BookDto>.SuccessResponse(book));
        }

        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetBooksByAuthor(int authorId)
        {
            var books = await _bookService.GetBooksByAuthorAsync(authorId);
            return Ok(ApiResponse<IEnumerable<BookDto>>.SuccessResponse(books));
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetBooksByCategory(int categoryId)
        {
            var books = await _bookService.GetBooksByCategoryAsync(categoryId);
            return Ok(ApiResponse<IEnumerable<BookDto>>.SuccessResponse(books));
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableBooks()
        {
            var books = await _bookService.GetAvailableBooksAsync();
            return Ok(ApiResponse<IEnumerable<BookDto>>.SuccessResponse(books));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookDto createBookDto)
        {
            try
            {
                var book = await _bookService.CreateBookAsync(createBookDto);
                return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, ApiResponse<BookDto>.SuccessResponse(book, "Book created successfully"));
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<BookDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDto updateBookDto)
        {
            try
            {
                await _bookService.UpdateBookAsync(id, updateBookDto);
                return Ok(ApiResponse<object>.SuccessResponse(new { }, "Book updated successfully"));
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                await _bookService.DeleteBookAsync(id);
                return Ok(ApiResponse<object>.SuccessResponse(new { }, "Book deleted successfully"));
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}