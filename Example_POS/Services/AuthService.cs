using Example_POS.Data;
using Example_POS.DTOs.User;
using Example_POS.Models;
using Example_POS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Example_POS.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _db = context;
            _configuration = configuration;
        }

        public async Task<string?> LoginAsync(LoginDTO request)
        {
            User? user = _db.Users.FirstOrDefault(u => u.Email == request.Email);
            // User not found
            if (user == null)
            {
                return null!;
            }
            // Wrong Password
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash!, request.Password!) == PasswordVerificationResult.Failed)
            {
                return null!;
            }

            string token = CreateToken(user);

            return token;
        }

        public async Task<User?> RegisterAsync(RegisterDTO request)
        {
            // Check username duplicate
            if (await _db.Users.AnyAsync(u => u.Username == request.Username))
            {
                return null;
            }
            // Check email duplicate
            if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            {
                return null;
            }

            // Create New User
            var newUser = new User();
            var hasedPassword = new PasswordHasher<User>().
                HashPassword(newUser, request.Password!);
            newUser.PasswordHash = hasedPassword;
            newUser.Email = request.Email;
            newUser.Username = request.Username;

            string sql = "INSERT INTO Users (Username, Email, PasswordHash) VALUES (@username, @email, @password)";
            _db.Database.ExecuteSqlRaw(sql,
                new SqlParameter("@username", request.Username),
                new SqlParameter("@email", request.Email),
                new SqlParameter("@password", hasedPassword)
            );

            return newUser;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!)
                );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),


                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
