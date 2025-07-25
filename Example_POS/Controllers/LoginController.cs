using Example_POS.Data;
using Example_POS.DTOs;
using Example_POS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Diagnostics;

namespace Example_POS.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _db;
        public LoginController(ApplicationDbContext db)
        {
            _db = db;
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
            User user = _db.Users.FirstOrDefault(u => u.Email == obj.Email);
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
    }
}
