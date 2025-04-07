using AutoMapper;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces.Services;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Models.Dtos;
using LibraryManagement.Core.Models;
using LibraryManagement.Infrastructure.Utils;

namespace LibraryManagement.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(email);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var existingUserWithEmail = await _unitOfWork.Users.GetUserByEmailAsync(createUserDto.Email);
            if (existingUserWithEmail != null)
                throw new ApiException("Email is already in use", 400);

            var existingUserWithUsername = await _unitOfWork.Users.GetUserByUsernameAsync(createUserDto.Username);
            if (existingUserWithUsername != null)
                throw new ApiException("Username is already in use", 400);

            var salt = PasswordHasher.GenerateSalt();
            var passwordHash = PasswordHasher.HashPassword(createUserDto.Password, salt);

            var user = _mapper.Map<User>(createUserDto);
            user.PasswordHash = passwordHash;
            user.Salt = Convert.ToBase64String(salt);

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                throw new ApiException($"User with ID {id} not found", 404);

            if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != user.Email)
            {
                var existingUserWithEmail = await _unitOfWork.Users.GetUserByEmailAsync(updateUserDto.Email);
                if (existingUserWithEmail != null)
                    throw new ApiException("Email is already in use", 400);

                user.Email = updateUserDto.Email;
            }

            if (!string.IsNullOrEmpty(updateUserDto.FirstName))
                user.FirstName = updateUserDto.FirstName;

            if (!string.IsNullOrEmpty(updateUserDto.LastName))
                user.LastName = updateUserDto.LastName;

            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                var salt = PasswordHasher.GenerateSalt();
                var passwordHash = PasswordHasher.HashPassword(updateUserDto.Password, salt);
                user.PasswordHash = passwordHash;
                user.Salt = Convert.ToBase64String(salt);
            }

            if (!string.IsNullOrEmpty(updateUserDto.Role))
                user.Role = updateUserDto.Role;

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                throw new ApiException($"User with ID {id} not found", 404);

            var activeOrders = await _unitOfWork.Orders.GetAsync(
                o => o.UserId == id && (o.Status == OrderStatus.Pending || o.Status == OrderStatus.Approved || o.Status == OrderStatus.Borrowed)
            );

            if (activeOrders.Any())
                throw new ApiException("Cannot delete user with active orders", 400);

            await _unitOfWork.Users.DeleteAsync(user);
            await _unitOfWork.CompleteAsync();
        }
    }
}