using Example_POS.Data;
using Example_POS.DTOs.User;
using Example_POS.Helper;
using Example_POS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Example_POS.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IJwtHelper _jwtHelper;

        public LoginController(ApplicationDbContext db, IJwtHelper jwtHelper)
        {
            _db = db;
            _jwtHelper = jwtHelper;
        }
        public IActionResult Index()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LoginModels obj)
        {
            //string email = "chaky@email.com";
            //string password = "0123456789";

            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            if (obj == null)
            {
                ViewBag.Message = "Invalid input.";
                return View(obj);
            }

            //if (obj.Email == email && obj.Password == password)
            //{
            //    TempData["Message"] = "Login Success!";
            //    TempData["Email"] = email;
            //    TempData["Password"] = password;
            //    return RedirectToAction("Index", "Home");
            //}
            User? user = _db.Users.FirstOrDefault(u => u.Email == obj.Email);
            if (user != null && user.Password == obj.Password)
            {
                TempData["Message"] = "Login Success!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "Login Failed. Please check your email or password.";
                return View(obj);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModels loginData)
        {

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(loginData.Email) || string.IsNullOrWhiteSpace(loginData.Password))
            {
                ModelState.AddModelError("", "Email and Password are required.");
                return View();
            }

            try
            {
                var user = ValidateUser(loginData.Email, loginData.Password);

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View();
                }

                var token = _jwtHelper.GenerateJwtToken(user.Email, user.Username);
                var principal = _jwtHelper.ValidateToken(token);

                if (principal == null)
                {
                    ModelState.AddModelError("", "Invalid token.");
                    return View();
                }

                // Generate & store refresh token
                var refreshToken = _jwtHelper.GenerateRefreshToken();
                var refreshExpiry = DateTime.UtcNow.AddDays(7);

                const string updateUserCommand = @"
                                    UPDATE Users 
                                    SET RefreshToken = @refreshToken, 
                                        RefreshTokenExpiryTime = @refreshTokenExpiryTime 
                                    WHERE Id = @userId";

                _db.Database.ExecuteSqlRaw(updateUserCommand,
                    new SqlParameter("@userId", user.Id),
                    new SqlParameter("@refreshToken", refreshToken),
                    new SqlParameter("@refreshTokenExpiryTime", refreshExpiry));

                // Set access token cookie
                Response.Cookies.Append("access_token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddMinutes(30)
                });

                Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = refreshExpiry
                });


                TempData["Message"] = "Login successful!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Optional: Log the exception
                // _logger.LogError(ex, "Login failed");

                ModelState.AddModelError("", ex.Message);
                return View();
            }
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
