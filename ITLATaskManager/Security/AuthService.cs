using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ITLATaskManager.DataAccess.Data;
using ITLATaskManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ITLATaskManagerAPI.Security
{
    public class AuthService : IAuthService
    {
        private readonly string _secretKey;
        private readonly ApplicationDbContext _context;

        public AuthService(IOptions<AppSettings> options, ApplicationDbContext context)
        {
            _secretKey = options.Value.SecretKey;
            _context = context;
        }

        public async Task<string> GetToken(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            var hashedPassword = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(password))
            );

            if (user.Password != hashedPassword)
            {
                throw new UnauthorizedAccessException("Invalid password");
            }

            string key = _secretKey;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256Signature
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new[]
                    {
                        new Claim("Email", email),
                        new Claim("PasswordHash", hashedPassword),
                        new Claim(ClaimTypes.Role, user.Role),
                    }
                ),
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(120),
                SigningCredentials = credentials,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return await Task.Run(() => tokenHandler.WriteToken(token));
        }
    }
}
