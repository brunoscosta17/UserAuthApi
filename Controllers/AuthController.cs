using Microsoft.AspNetCore.Mvc;
using UserAuthApi.DTOs;
using UserAuthApi.Services;

namespace UserAuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            try
            {
                var user = await _userService.RegisterAsync(dto);
                return Ok(new { user.Id, user.Name, user.Email });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            var user = await _userService.AuthenticateAsync(dto);
            if (user is null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
            var token = _tokenService.GenerateToken(user);
            return Ok(new 
            { 
                token,
                refreshToken = user.RefreshToken,
                user = new { user.Id, user.Name, user.Email }
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            var user = await _userService.GetByEmailAsync(dto.Email);
            if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }

            var newToken = _tokenService.GenerateToken(user);
            user.RefreshToken = _userService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userService.UpdateAsync(user);

            return Ok(new
            {
                token = newToken,
                refreshToken = user.RefreshToken
            });
        }

    }
}
