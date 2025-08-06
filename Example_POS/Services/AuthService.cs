using Azure.Core;
using Example_POS.Data;
using Example_POS.DTOs.Token;
using Example_POS.DTOs.User;
using Example_POS.Models;
using Example_POS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

        public async Task<TokenResponseDTO?> LoginAsync(LoginDTO request, HttpContext httpContext)
        {
            try
            {
                User? user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
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

                string accessToken = CreateAccessToken(user);
                string refreshToken = GenerateRefreshToken();
                DateTime refreshTokenExpiredDate = DateTime.UtcNow.AddDays(7);

                // UPDATE Users
                string sql = @"
                        UPDATE Users 
                        SET 
                            AccessToken = @AccessToken,
                            RefreshToken = @RefreshToken,
                            RefreshTokenExpiredDate = @RefreshTokenExpiredDate
                        WHERE Email = @Email";
                _db.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@AccessToken", accessToken),
                    new SqlParameter("@RefreshToken", refreshToken),
                    new SqlParameter("@RefreshTokenExpiredDate", refreshTokenExpiredDate),
                    new SqlParameter("@Email", request.Email)
                );

                var resposeToken = new TokenResponseDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiredDate = refreshTokenExpiredDate
                };

                return resposeToken;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<User?> RegisterAsync(RegisterDTO request)
        {
            try
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
                // Tokens
                string sql = "INSERT INTO Users (Username, Email, PasswordHash) VALUES (@username, @email, @password)";
                _db.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@username", request.Username),
                    new SqlParameter("@email", request.Email),
                    new SqlParameter("@password", hasedPassword)
                );

                return newUser;
            }
            catch(Exception)
            {
                return null;
            }
        }

        private string CreateAccessToken(User user)
        {
            try
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
                    expires: DateTime.UtcNow.AddMinutes(10),
                    signingCredentials: creds
                    );

                return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            }
            catch(Exception)
            {
                return null!;
            }
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        //private async Task<string> GenerateAndSaveRefreshToken(User user)
        //{
        //    try
        //    {
        //        var refreshToken = GenerateRefreshToken();
        //        user.RefreshToken = refreshToken;
        //        user.RefreshTokenExpiredDate = DateTime.UtcNow.AddDays(7);
        //        await _db.SaveChangesAsync();
        //        return refreshToken;
        //    }
        //    catch (Exception)
        //    {
        //        return null!;
        //    }
        //}

        public ClaimsPrincipal? ValidateAccessToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["AppSettings:Token"]!);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["AppSettings:Issuer"],
                    ValidAudience = _configuration["AppSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return principal;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string?> ValidateRefreshToken(string refreshToken)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user is null)
            {
                return null;
            }
            if (user.RefreshTokenExpiredDate < DateTime.UtcNow)
            {
                return null;
            }
            else
            {
                string newAccessToken = CreateAccessToken(user);
                DateTime exp = DateTime.UtcNow.AddDays(7);

                string sql = @"
                    UPDATE Users 
                    SET 
                        AccessToken = @AccessToken,
                        RefreshTokenExpiredDate = @RefreshTokenExpiredDate
                    WHERE Email = @Email";
                _db.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@AccessToken", newAccessToken),
                    new SqlParameter("@RefreshTokenExpiredDate", exp),
                    new SqlParameter("@Email", user.Email)
                );

                return newAccessToken;
            }
        }
    }
}
