using AutoMapper;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces.Services;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Models.Dtos;
using LibraryManagement.Core.Models;

namespace LibraryManagement.Infrastructure.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
        {
            var authors = await _unitOfWork.Authors.GetAllAsync();
            return _mapper.Map<IEnumerable<AuthorDto>>(authors);
        }

        public async Task<AuthorDto?> GetAuthorByIdAsync(int id)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(id);
            return author != null ? _mapper.Map<AuthorDto>(author) : null;
        }

        public async Task<IEnumerable<AuthorDto>> GetAuthorsByBookAsync(int bookId)
        {
            var authors = await _unitOfWork.Authors.GetAuthorsByBookIdAsync(bookId);
            return _mapper.Map<IEnumerable<AuthorDto>>(authors);
        }

        public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createAuthorDto)
        {
            var author = _mapper.Map<Author>(createAuthorDto);
            await _unitOfWork.Authors.AddAsync(author);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<AuthorDto>(author);
        }

        public async Task UpdateAuthorAsync(int id, UpdateAuthorDto updateAuthorDto)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(id);
            if (author == null)
                throw new ApiException($"Author with ID {id} not found", 404);

                        if (!string.IsNullOrEmpty(updateAuthorDto.FirstName))
                author.FirstName = updateAuthorDto.FirstName;

            if (!string.IsNullOrEmpty(updateAuthorDto.LastName))
                author.LastName = updateAuthorDto.LastName;

            if (updateAuthorDto.BirthDate.HasValue)
                author.BirthDate = updateAuthorDto.BirthDate.Value;

            if (!string.IsNullOrEmpty(updateAuthorDto.Biography))
                author.Biography = updateAuthorDto.Biography;

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAuthorAsync(int id)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(id);
            if (author == null)
                throw new ApiException($"Author with ID {id} not found", 404);

                        var booksWithAuthor = await _unitOfWork.Books.GetBooksByAuthorIdAsync(id);
            if (booksWithAuthor.Any())
                throw new ApiException("Cannot delete author with associated books", 400);

            await _unitOfWork.Authors.DeleteAsync(author);
            await _unitOfWork.CompleteAsync();
        }
    }
}