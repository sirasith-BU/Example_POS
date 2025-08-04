using Example_POS.Data;
using Example_POS.DTOs.User;
using Example_POS.Models;
using Example_POS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

            var user = await _authService.LoginAsync(request);
            if (user is null)
            {
                ViewBag.Message = "Invalid. Please try again";
                return View();
            }

            TempData["Message"] = user;
            return RedirectToAction("Index", "Home");
        }
    }
}
