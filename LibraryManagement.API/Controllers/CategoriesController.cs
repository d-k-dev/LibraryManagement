using LibraryManagement.Core.Interfaces.Services;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(categories));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound(ApiResponse<CategoryDto>.ErrorResponse($"Category with ID {id} not found"));

            return Ok(ApiResponse<CategoryDto>.SuccessResponse(category));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            try
            {
                var category = await _categoryService.CreateCategoryAsync(createCategoryDto);
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, ApiResponse<CategoryDto>.SuccessResponse(category, "Category created successfully"));
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<CategoryDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            try
            {
                await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
                return Ok(ApiResponse<object>.SuccessResponse(new { }, "Category updated successfully"));
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                return Ok(ApiResponse<object>.SuccessResponse(new { }, "Category deleted successfully"));
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}