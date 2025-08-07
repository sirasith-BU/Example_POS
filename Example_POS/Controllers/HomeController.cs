using Example_POS.Data;
using Example_POS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Example_POS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            bool isLogin = true;
            if (isLogin)
            {
                ViewBag.Message = TempData["Message"];
                //ViewBag.Email = TempData["Email"];
                //ViewBag.Password = TempData["Password"];
                ViewBag.Username = TempData["Username"];
                return View();
            }
            TempData["Message"] = "Please login or register!";
            return RedirectToAction("Index", "Login");
        }

        public IActionResult Privacy()
        {
            Boolean Login = true;
            if (Login)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
