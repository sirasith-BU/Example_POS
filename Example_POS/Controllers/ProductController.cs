using Example_POS.DTOs.Category;
using Example_POS.DTOs.Product;
using Example_POS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;

namespace Example_POS.Controllers
{
    public class ProductController : Controller
    {
        private readonly IConfiguration _configuration;
        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Create
        [HttpPost]
        public async Task<IActionResult> Create(AddProduct newProductData)
        {
            string? strConnection = string.Empty;
            SqlConnection? mySqlConnection = null;

            SqlCommand? sqlCheckDupName = null;
            StringBuilder? strCheckDupName = new("");

            SqlCommand? sqlInsertCommand = null;
            StringBuilder? strInsert = new("");

            int NumberOfRows = 0;
            SqlDataReader? mySqlDataReader = null; // one

            try
            {
                // Database Connection
                strConnection = _configuration.GetConnectionString("DefaultConnection");
                mySqlConnection = new SqlConnection(strConnection);
                await mySqlConnection.OpenAsync();

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        // Update
        [HttpPost]
        public async Task<IActionResult> Update(EditProduct updateProductData)
        {
            string? strConnection = string.Empty;
            SqlConnection? mySqlConnection = null;

            try
            {
                // Database Connection
                strConnection = _configuration.GetConnectionString("DefaultConnection");
                mySqlConnection = new SqlConnection(strConnection);
                await mySqlConnection.OpenAsync();

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        // Delete
        public async Task<IActionResult> Delete(int id)
        {
            string? strConnection = string.Empty;
            SqlConnection? mySqlConnection = null;

            try
            {
                // Database Connection
                strConnection = _configuration.GetConnectionString("DefaultConnection");
                mySqlConnection = new SqlConnection(strConnection);
                await mySqlConnection.OpenAsync();

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
