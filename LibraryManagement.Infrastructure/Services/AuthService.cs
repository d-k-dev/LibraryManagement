using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Interfaces.Services;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Models.Dtos;
using LibraryManagement.Infrastructure.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagement.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _unitOfWork.Users.GetUserByUsernameAsync(loginDto.Username);

            if (user == null)
                return null;

                        var salt = Convert.FromBase64String(user.Salt);
            var passwordHash = PasswordHasher.HashPassword(loginDto.Password, salt);

            if (passwordHash != user.PasswordHash)
                return null;

            var userDto = _mapper.Map<UserDto>(user);
            var token = GenerateJwtToken(userDto);

            return new AuthResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Token = token
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
                        var existingUserWithUsername = await _unitOfWork.Users.GetUserByUsernameAsync(registerDto.Username);
            if (existingUserWithUsername != null)
                throw new ApiException("Username is already taken", 400);

                        var existingUserWithEmail = await _unitOfWork.Users.GetUserByEmailAsync(registerDto.Email);
            if (existingUserWithEmail != null)
                throw new ApiException("Email is already in use", 400);

                        var salt = PasswordHasher.GenerateSalt();
            var passwordHash = PasswordHasher.HashPassword(registerDto.Password, salt);

                        var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                Username = registerDto.Username,
                PasswordHash = passwordHash,
                Salt = Convert.ToBase64String(salt),
                Role = "User"
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            var userDto = _mapper.Map<UserDto>(user);
            var token = GenerateJwtToken(userDto);

            return new AuthResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Token = token
            };
        }

        public string GenerateJwtToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "Khoroshkeev_Dmitry@LibraryManagement");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}