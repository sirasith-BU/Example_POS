using Example_POS.Data;
using Example_POS.DTOs.User;
using Example_POS.Helper;
using Example_POS.Models;
using Example_POS.Service.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Example_POS.Controllers
{
    public class LogoutController : Controller
    {
        public LogoutController()
        {

        }
 
        [HttpGet]
        public IActionResult Index()
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            return RedirectToAction("Index", "Login");
        }
    }
}
