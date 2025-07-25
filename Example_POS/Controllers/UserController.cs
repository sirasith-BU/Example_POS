using Example_POS.Data;
using Example_POS.DTOs;
using Example_POS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Example_POS.Controllers
{
    public class UserController: Controller
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            //IEnumerable<User> allUser = _db.Users;
            //if (!allUser.Any())
            //{
            //    return View();
            //}
            var model = new UserPageViewModel
            {
                Users = _db.Users,
                UpdateForm = new UpdateUser()

            };
            ViewBag.Message = TempData["Message"];
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DelUser(int? userId)
        {
            if (userId == 0 || userId == null)
            {
                return NotFound();
            }

            string delUserById = "DELETE FROM Users WHERE Id=@userId";
            _db.Database.ExecuteSqlRaw(delUserById, new SqlParameter("@userId", userId));

            TempData["Message"] = "Delete user success!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(UpdateUser userUpdateData)
        {
            if (userUpdateData.Id == 0 || userUpdateData.Id == null)
            {
                return NotFound();
            }

            string checkEmailSQL = "SELECT * FROM Users WHERE Email=@email";
            User sameEmail = _db.Users.FromSqlRaw(checkEmailSQL, new SqlParameter("@email", userUpdateData.Email)).FirstOrDefault();
            if (sameEmail != null && sameEmail.Id != userUpdateData.Id)
            {
                ModelState.AddModelError("UpdateForm.Email", "Email already use.");

                var model = new UserPageViewModel
                {
                    Users = _db.Users,
                    UpdateForm = userUpdateData
                };
                ViewBag.ShowUpdateModal = true;
                ViewBag.ModalUserId = userUpdateData.Id;
                return View("Index", model);
            }

            string checkUsernameSQL = "SELECT * FROM Users WHERE Username=@username";
            User checkUsername = _db.Users.FromSqlRaw(checkUsernameSQL, new SqlParameter("@username", userUpdateData.Username)).FirstOrDefault();
            if (checkUsername != null && checkUsername.Id != userUpdateData.Id)
            {
                ModelState.AddModelError("UpdateForm.Username", "Username already use.");

                var model = new UserPageViewModel
                {
                    Users = _db.Users,
                    UpdateForm = userUpdateData
                };
                ViewBag.ShowUpdateModal = true;
                ViewBag.ModalUserId = userUpdateData.Id;
                return View("Index", model);         
            }

            string updateUserByIdSQL = "UPDATE Users SET Email=@email, Username=@username WHERE Id=@userId";
            _db.Database.ExecuteSqlRaw(updateUserByIdSQL,
                new SqlParameter("@userId", userUpdateData.Id),
                new SqlParameter("@email", userUpdateData.Email),
                new SqlParameter("@username", userUpdateData.Username));

            TempData["Message"] = "Update user success!";
            return RedirectToAction("Index");
        }
    }
}
