using Azure.Core;
using Example_POS.DTOs.Token;
using Example_POS.DTOs.User;
using Example_POS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Example_POS.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAuthService _authService;
        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }
        public IActionResult Index()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginDTO request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            TokenResponseDTO? token = await _authService.LoginAsync(request, HttpContext);
            
            if (token is null)
            {
                ViewBag.Message = "Invalid. Please try again";
                return View();
            }

            // Set Cookies
            Response.Cookies.Append("accessToken", token!.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(10)
            });
            Response.Cookies.Append("refreshToken", token.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet("authorize")]
        public IActionResult AuthenticatedEndpoints()
        {
            return Ok("Authenticated!");
        }
    }
}
