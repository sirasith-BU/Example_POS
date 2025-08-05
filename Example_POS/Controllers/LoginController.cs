using Example_POS.Data;
using Example_POS.DTOs.User;
using Example_POS.Helper;
using Example_POS.Models;
using Example_POS.Service.System;
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
        private readonly IAuthService _authService;

        public LoginController(ApplicationDbContext db, IAuthService authService)
        {
            _db = db;
            _authService = authService;
        }
        public IActionResult Index()
        {
            ViewBag.Message = TempData["Message"];

            var token = HttpContext.Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginModels loginData)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(loginData.Email) || string.IsNullOrWhiteSpace(loginData.Password))
            {
                ModelState.AddModelError("", "Email and Password are required.");
                return View();
            }

            try
            {
                var res = await _authService.Login(loginData);
                // Set access token cookie
                Response.Cookies.Append("access_token", res.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddMinutes(30)
                });

                Response.Cookies.Append("refresh_token", res.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddDays(7)
                });


                TempData["Message"] = "Login successful!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(loginData);
            }
        }


        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            return RedirectToAction("Index", "Login");
        }
    }
}
