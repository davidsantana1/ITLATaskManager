using ITLATaskManager.DataAccess.Data;
using ITLATaskManager.Models;
using ITLATaskManagerAPI.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITLATaskManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;

        public AuthController(IAuthService authService, ApplicationDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _authService.GetToken(request.Email, request.Password);
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { message = "Usuario o contraseña incorrectos" });
            }
        }

        [HttpPost("createUser")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Email and password are required");
            }

            if (user.Role != "Admin" && user.Role != "User")
            {
                return BadRequest("Role must be either 'Admin' or 'User'");
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return BadRequest("User with this email already exists");
            }

            var hashedPassword = Convert.ToHexString(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(user.Password)
                )
            );
            user.Password = hashedPassword;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }
    }
}
