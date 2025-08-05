using Example_POS.DTOs.Token;
using Example_POS.DTOs.User;
using Example_POS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
