using LibraryManagement.Core.Interfaces.Services;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            return Ok(ApiResponse<IEnumerable<AuthorDto>>.SuccessResponse(authors));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null)
                return NotFound(ApiResponse<AuthorDto>.ErrorResponse($"Author with ID {id} not found"));

            return Ok(ApiResponse<AuthorDto>.SuccessResponse(author));
        }

        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetAuthorsByBook(int bookId)
        {
            var authors = await _authorService.GetAuthorsByBookAsync(bookId);
            return Ok(ApiResponse<IEnumerable<AuthorDto>>.SuccessResponse(authors));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> CreateAuthor([FromBody] CreateAuthorDto createAuthorDto)
        {
            try
            {
                var author = await _authorService.CreateAuthorAsync(createAuthorDto);
                return CreatedAtAction(nameof(GetAuthorById), new { id = author.Id }, ApiResponse<AuthorDto>.SuccessResponse(author, "Author created successfully"));
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<AuthorDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] UpdateAuthorDto updateAuthorDto)
        {
            try
            {
                await _authorService.UpdateAuthorAsync(id, updateAuthorDto);
                return Ok(ApiResponse<object>.SuccessResponse(new { }, "Author updated successfully"));
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                await _authorService.DeleteAuthorAsync(id);
                return Ok(ApiResponse<object>.SuccessResponse(new { }, "Author deleted successfully"));
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}