using Azure;
using Example_POS.Data;
using Example_POS.DTOs.User;
using Example_POS.Helper;
using Example_POS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Example_POS.Service.System
{
    public interface IAuthService
    {
        public Task Register(RegisterModels register);
        public Task<Token> Login(LoginModels loginData);
    }


    public class AuthService : IAuthService
    {

        private readonly ApplicationDbContext _db;
        private readonly IJwtHelper _jwtHelper;



        public AuthService(ApplicationDbContext db, IJwtHelper jwtHelper)
        {
            _db = db;
            _jwtHelper = jwtHelper;
        }

        public async Task<Token> Login(LoginModels loginData)
        {
            try
            {
                var user = ValidateUser(loginData.Email, loginData.Password);

                if (user == null)
                {

                    throw new Exception("Invalid email or password.");
                }

                var token = _jwtHelper.GenerateJwtToken(user.Email, user.Username);
                var principal = _jwtHelper.ValidateToken(token);

                if (principal == null)
                {
                    throw new Exception("Invalid token.");
                }

                // Generate & store refresh token
                var refreshToken = _jwtHelper.GenerateRefreshToken();
                var refreshExpiry = DateTime.UtcNow.AddDays(7);

                const string updateUserCommand = @"
                                                    UPDATE Users 
                                                    SET RefreshToken = @refreshToken, 
                                                        RefreshTokenExpiryTime = @refreshTokenExpiryTime 
                                                    WHERE Id = @userId";

                await _db.Database.ExecuteSqlRawAsync(updateUserCommand,
                    new SqlParameter("@userId", user.Id),
                    new SqlParameter("@refreshToken", refreshToken),
                    new SqlParameter("@refreshTokenExpiryTime", refreshExpiry));


                return new Token() { AccessToken = token, RefreshToken = refreshToken};


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Register(RegisterModels register)
        {
            string checkExistsCommand = "SELECT * FROM Users WHERE Username = @username or Email = @email";
            var user = await _db.Users
                .FromSqlRaw(checkExistsCommand, 
                new SqlParameter("@username", register.Username), 
                new SqlParameter("@email", register.Email)).FirstOrDefaultAsync();

            if (user != null)
            {
                throw new Exception("Username or Email already use.");
            }

            var salt = GenerateSalt();

            register.Password = HashPassword(register.Password, salt);

            string registerCommand = "INSERT INTO Users (Username, Email, Password,Salt) VALUES (@username, @email, @password, @salt)";
            await _db.Database.ExecuteSqlRawAsync(registerCommand,
                new SqlParameter("@username", register.Username),
                new SqlParameter("@email", register.Email),
                new SqlParameter("@password", register.Password),
                new SqlParameter("@salt", salt));

            await Task.CompletedTask;
        }

        public User? ValidateUser(string email, string password)
        {
            string selectUserCommand = $"SELECT * FROM Users WHERE Email=@email and IsActive = @isActive and IsDelete = @isDelete";
            var user = _db.Users.FromSqlRaw(selectUserCommand,
                                            new SqlParameter("@email", email),
                                            new SqlParameter("@isActive", true),
                                            new SqlParameter("@isDelete", false)).FirstOrDefault();

            if (user == null)
                return null;

            var hashedInputPassword = HashPassword(password, user.Salt);

            if (user.Password != hashedInputPassword)
                return null;

            return user;
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                var bytes = Encoding.UTF8.GetBytes(saltedPassword);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private string GenerateSalt(int size = 32)
        {
            var saltBytes = RandomNumberGenerator.GetBytes(size); 
            return Convert.ToBase64String(saltBytes);
        }
    }
}
