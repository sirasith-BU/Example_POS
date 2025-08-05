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
        public Task<Token> Login(LoginModels loginData);
        public Task Logout();
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

        public Task Logout()
        {
            throw new NotImplementedException();
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

            //var hashedInputPassword = HashPassword(password, user.Salt);

            //if (user.Password != hashedInputPassword)
            //    return null;

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
    }
}
