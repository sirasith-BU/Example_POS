using Example_POS.Data;
using Example_POS.DTOs.User;
using Example_POS.Extensions;
using Example_POS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Example_POS.Controllers
{
    public class UserController : Controller
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
        public IActionResult GetUser()
        {
            // ดึงค่าจาก DataTables Request
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int drawInt = draw != null ? Convert.ToInt32(draw) : 0;

            // ดึงค่าการเรียงลำดับ
            var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
            var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault(); // asc or desc

            //Filter
            var username = Request.Form["username"].FirstOrDefault()?.Trim();
            var email = Request.Form["email"].FirstOrDefault()?.Trim();

            // Query
            var query = _db.Users.AsQueryable();

            // Filter (Search)
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.Trim();

                query = query.Where(f =>
                    f.Username.StartsWith(searchValue) ||
                    f.Email.StartsWith(searchValue)
                );
            }


            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(u => u.Username.Contains(username));
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(u => u.Email.Contains(email));
            }


            int recordsTotal = query.Count();


            // Apply Ordering
            if (!string.IsNullOrEmpty(sortColumnIndex))
            {
                int columnIndex = Convert.ToInt32(sortColumnIndex);

                // Map column index to property name
                string columnName = columnIndex switch
                {
                    0 => nameof(Example_POS.Models.User.Id),
                    1 => nameof(Example_POS.Models.User.Username),
                    2 => nameof(Example_POS.Models.User.Email),
                    _ => nameof(Example_POS.Models.User.Id)
                };

                if (sortDirection == "asc")
                {
                    query = query.OrderByDynamic(columnName, ascending: true);
                }
                else
                {
                    query = query.OrderByDynamic(columnName, ascending: false);
                }
            }
            else
            {
                // Default order
                query = query.OrderBy(u => u.Id);
            }

            // ดึงเฉพาะหน้าที่ต้องแสดง
            var data = query.Skip(skip).Take(pageSize).ToList();

            // ส่งกลับในรูปแบบที่ DataTables เข้าใจ
            return Json(new
            {
                draw = drawInt,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = data
            });
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
            User? sameEmail = _db.Users.FromSqlRaw(checkEmailSQL, new SqlParameter("@email", userUpdateData.Email)).FirstOrDefault();
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
            User? checkUsername = _db.Users.FromSqlRaw(checkUsernameSQL, new SqlParameter("@username", userUpdateData.Username)).FirstOrDefault();
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
