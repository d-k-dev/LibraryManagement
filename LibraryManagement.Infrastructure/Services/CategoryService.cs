using AutoMapper;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces.Services;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Models.Dtos;
using LibraryManagement.Core.Models;

namespace LibraryManagement.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            return category != null ? _mapper.Map<CategoryDto>(category) : null;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var category = _mapper.Map<Category>(createCategoryDto);
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                throw new ApiException($"Category with ID {id} not found", 404);

                        if (!string.IsNullOrEmpty(updateCategoryDto.Name))
                category.Name = updateCategoryDto.Name;

            if (!string.IsNullOrEmpty(updateCategoryDto.Description))
                category.Description = updateCategoryDto.Description;

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                throw new ApiException($"Category with ID {id} not found", 404);

                        var booksInCategory = await _unitOfWork.Books.GetBooksByCategoryIdAsync(id);
            if (booksInCategory.Any())
                throw new ApiException("Cannot delete category with associated books", 400);

            await _unitOfWork.Categories.DeleteAsync(category);
            await _unitOfWork.CompleteAsync();
        }
    }
}