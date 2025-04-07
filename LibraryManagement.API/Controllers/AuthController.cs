using LibraryManagement.Core.Interfaces.Services;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            if (response == null)
                return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid username or password"));

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Login successful"));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var response = await _authService.RegisterAsync(registerDto);
                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Registration successful"));
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<AuthResponseDto>.ErrorResponse(ex.Message));
            }
        }
    }
}