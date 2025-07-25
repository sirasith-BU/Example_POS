using Example_POS.DTOs.Category;
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

        public async Task<IActionResult> Index()
        {
            // Default SQL Connection
            string strConnection = string.Empty;
            SqlConnection? mySqlConnection = null;
            SqlCommand? mySqlCommand = null;
            StringBuilder? strCommand = new("");
            int NumberOfRows = 0;
            string ColumnName = "";
            SqlDataReader? mySqlDataReader = null; // one
            SqlDataAdapter? mySqlDataAdapter = null; // many
            DataSet ListDataSet = new DataSet();

            // Properties
            string CatName = "";
            int catId = 0;
            CategoryOptions TempCat = null;
            List<CategoryOptions> CatList = [];
            try
            {
                //strConnection = AuthVariable.MVCConnectionString;
                mySqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await mySqlConnection.OpenAsync();
                mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandTimeout = 0;

                strCommand = new StringBuilder("");
                strCommand.Append(" SELECT ID, Name FROM Categories");

                mySqlCommand.CommandText = strCommand.ToString();
                mySqlDataAdapter = new SqlDataAdapter();
                mySqlDataAdapter.SelectCommand = mySqlCommand;
                ListDataSet.Dispose();
                ListDataSet = new DataSet();
                NumberOfRows = mySqlDataAdapter.Fill(ListDataSet, "Result");
                mySqlDataAdapter.Dispose();

                foreach (DataRow ListItem in ListDataSet.Tables["Result"]!.Rows)
                {
                    ColumnName = "Name";
                    if (ListItem.IsNull(ColumnName))
                        CatName = "";
                    else
                        CatName = Convert.ToString(ListItem[ColumnName])!;

                    ColumnName = "Id";
                    catId = Convert.ToInt32(ListItem[ColumnName]);

                    TempCat = new()
                    {
                        Id = catId,
                        Name = CatName,

                    };
                    CatList.Add(TempCat);
                    //CatList.Add( new CategoryDTO
                    //{
                    //    Id = catId,
                    //    Name = CategoryName
                    //});
                }
                ViewBag.Category = CatList;
                return View();
            }
            catch(Exception e)
            {
                return View(e);
            }
        }
    }
}
