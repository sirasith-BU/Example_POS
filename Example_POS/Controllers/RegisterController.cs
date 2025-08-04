using Example_POS.Data;
using Example_POS.DTOs.User;
using Example_POS.Models;
using Example_POS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Example_POS.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IAuthService _authService;
        public RegisterController(IAuthService authService)
        {
            _authService = authService;
        } 
        public IActionResult Index()
        {
            return View();
        }

        public static User newUser = new();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(RegisterDTO request)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", request);
            }

            if (request.RePassword != request.Password)
            {
                ModelState.AddModelError("RePassword", "Passwords don't match.");
                return View("Index", request);
            }

            var user = await _authService.RegisterAsync(request);
            if (user is null)
            {
                return BadRequest("Username or Email already exists!");
            }

            TempData["Message"] = user;
            return RedirectToAction("Index", "Login");
        }
    }
}
