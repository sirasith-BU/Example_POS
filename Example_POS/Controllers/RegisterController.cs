using Example_POS.Data;
using Example_POS.DTOs;
using Example_POS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Example_POS.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _db;
        public RegisterController(ApplicationDbContext db)
        {
            _db = db;
        } 
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp(RegisterModels obj)
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

            User username = _db.Users.FirstOrDefault(u => u.Username == obj.Username);
            string _sql = "SELECT * FROM Users WHERE Username = @username";
            var user = _db.Users
                .FromSqlRaw(_sql, new SqlParameter("@username", obj.Username))
                .FirstOrDefault();
            if (username != null)
            {
                ModelState.AddModelError("Username", "Username already use.");
                return View("Index", obj);
            }
            User email = _db.Users.FirstOrDefault(u => u.Email == obj.Email);
            if (email != null)
            {
                ModelState.AddModelError("Email", "Email already use.");
                return View("Index", obj);
            }

            // Database.ExecuteSqlRaw()
            string sql = "INSERT INTO Users (Username, Email, Password) VALUES (@username, @email, @password)";
            _db.Database.ExecuteSqlRaw(sql,
                new SqlParameter("@username", obj.Username),
                new SqlParameter("@email", obj.Email),
                new SqlParameter("@password", obj.Password)
            );

            //User newUser = new User
            //{
            //    Username = obj.Username,
            //    Email = obj.Email,
            //    Password = obj.Password,
            //};
            //_db.Users.Add(newUser);
            //_db.SaveChanges();

            TempData["Username"] = obj.Username;
            TempData["Message"] = "Register Success!";
            return RedirectToAction("Index", "Home");
        }
    }
}
