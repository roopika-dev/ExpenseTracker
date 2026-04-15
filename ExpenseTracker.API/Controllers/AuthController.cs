using ExpenseTracker.API.Helpers;
using ExpenseTracker.Core.DTOs.Auth;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ExpenseTracker.API.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwt;

        public AuthController(IAuthService authService, ApplicationDbContext context, JwtHelper jwt)
        {
            _authService = authService;
            _context = context;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.Register(dto);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.Login(dto);

            if (!int.TryParse(result, out int userId))
                return BadRequest(result);

            var user = await _context.Users.FirstAsync(x => x.Id == userId);

            var token = _jwt.GenerateToken(user);

            return Ok(token);
        }

        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            var user = _context.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null) return NotFound();

            return Ok(new
            {
                name = user.Name,
                email = user.Email
            });
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
        {
            if (model == null || string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Email))
                return BadRequest("Invalid data");

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return NotFound();

            user.Name = model.Name;
            user.Email = model.Email;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully" });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.OldPassword) || string.IsNullOrEmpty(dto.NewPassword))
                return BadRequest("Invalid data");

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return NotFound();

            // 🔥 USE SAME HASH METHOD
            var oldHashed = HashPassword(dto.OldPassword);

            if (user.PasswordHash != oldHashed)
                return BadRequest(new { message = "Old password is incorrect" });

            // 🔥 UPDATE WITH SAME HASH
            user.PasswordHash = HashPassword(dto.NewPassword);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully" });
        }
        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }
}