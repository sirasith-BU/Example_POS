using Example_POS.Data;
using Example_POS.DTOs.User;
using Example_POS.Models;
using Example_POS.Service.System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Example_POS.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IAuthService _authService;


        public RegisterController(ApplicationDbContext db, IAuthService authService)
        {
            _db = db;
            _authService = authService;
        } 
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(RegisterModels obj)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", obj);
            }

            if (obj.RePassword != obj.Password)
            {
                ModelState.AddModelError("RePassword", "Passwords do not match.");
                return View("Index", obj);
            }

            await _authService.Register(obj);

            TempData["Username"] = obj.Username;
            TempData["Message"] = "Register Success!";
            return RedirectToAction("Index", "Home");
        }
    }
}
