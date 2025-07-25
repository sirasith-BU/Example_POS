using Example_POS.DTOs;
using Example_POS.DTOs.Category;
using Example_POS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace Example_POS.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IConfiguration _configuration;
        public CategoryController(IConfiguration configuration)
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
            SqlDataAdapter? mySqlDataAdapter = null; // many
            DataSet ListDataSet = new DataSet();

            // Properties
            int Id = 0;
            string Name = "";
            string Desc = "";
            int CreateBy = 0;
            DateTime CreateAt = DateTime.Now;
            int UpdateBy = 0;
            DateTime UpdateAt = DateTime.Now;
            int IsDelete = 0;

            Category TempCat = null;
            List<Category> CatList = [];
            try
            {
                strConnection = _configuration.GetConnectionString("DefaultConnection");
                mySqlConnection = new SqlConnection(strConnection);
                await mySqlConnection.OpenAsync();
                mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandTimeout = 0;

                strCommand = new StringBuilder("");
                strCommand.Append("SELECT * FROM Categories");

                mySqlCommand.CommandText = strCommand.ToString();
                mySqlDataAdapter = new SqlDataAdapter();
                mySqlDataAdapter.SelectCommand = mySqlCommand;
                ListDataSet.Dispose();
                ListDataSet = new DataSet();
                NumberOfRows = mySqlDataAdapter.Fill(ListDataSet, "Result");
                mySqlDataAdapter.Dispose();

                foreach (DataRow ListItem in ListDataSet.Tables["Result"]!.Rows)
                {
                    ColumnName = "Id";
                    Id = Convert.ToInt32(ListItem[ColumnName]);

                    ColumnName = "Name";
                    if (ListItem.IsNull(ColumnName))
                        Name = "";
                    else
                        Name = Convert.ToString(ListItem[ColumnName])!;

                    ColumnName = "Description";
                    if (ListItem.IsNull(ColumnName))
                        Desc = "";
                    else
                        Desc = Convert.ToString(ListItem[ColumnName])!;

                    ColumnName = "CreateBy";
                    CreateBy = Convert.ToInt32(ListItem[ColumnName]);

                    ColumnName = "CreateTime";
                    CreateAt = (DateTime)ListItem[ColumnName];

                    ColumnName = "UpdateBy";
                    UpdateBy = Convert.ToInt32(ListItem[ColumnName]);

                    ColumnName = "UpdateTime";
                    UpdateAt = (DateTime)ListItem[ColumnName];

                    ColumnName = "isDelete";
                    IsDelete = Convert.ToInt32(ListItem[ColumnName]);

                    TempCat = new()
                    {
                        Id = Id,
                        Name = Name,
                        Description = Desc,
                        CreateBy = CreateBy,
                        CreateTime = CreateAt,
                        UpdateBy = UpdateBy,
                        UpdateTime = UpdateAt,
                        IsDelete = IsDelete
                    };
                    CatList.Add(TempCat);
                }
                ViewBag.Category = CatList;
                return View();
            }
            catch(Exception e)
            {
                return View(e);
            }
        }

        public async Task<IActionResult> Create(AddCategory newCatData)
        {
            // Default SQL Connection
            string strConnection = string.Empty;
            SqlConnection? mySqlConnection = null;
            SqlCommand? mySqlCommand = null;
            StringBuilder? strCommand = new("");
            int NumberOfRows = 0;

            try
            {
                strConnection = _configuration.GetConnectionString("DefaultConnection");
                mySqlConnection = new SqlConnection(strConnection);
                await mySqlConnection.OpenAsync();
                mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandTimeout = 0;

                strCommand = new StringBuilder();
                strCommand.Append("INSERT INTO Categories ");
                strCommand.Append("(Name, Description, CreateBy, CreateTime, UpdateBy, UpdateTime, IsDelete) ");
                strCommand.Append("VALUES (@Name, @Description, @CreateBy, @CreateTime, @UpdateBy, @UpdateTime, @IsDelete)");
                mySqlCommand.CommandText = strCommand.ToString();

                mySqlCommand.Parameters.AddWithValue("@Name", newCatData.Name);
                mySqlCommand.Parameters.AddWithValue("@Description", newCatData.Description ?? "");
                mySqlCommand.Parameters.AddWithValue("@CreateBy", 0);
                mySqlCommand.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                mySqlCommand.Parameters.AddWithValue("@UpdateBy", 0);
                mySqlCommand.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                mySqlCommand.Parameters.AddWithValue("@IsDelete", 0);

                NumberOfRows = await mySqlCommand.ExecuteNonQueryAsync();

                ViewBag.Message = "Add Category success!";
                return RedirectToAction("Index", "Category");
            }
            catch (Exception e)
            {
                return View(e);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            // Default SQL Connection
            string strConnection = string.Empty;
            SqlConnection? mySqlConnection = null;
            SqlCommand? mySqlCommand = null;
            StringBuilder? strCommand = new("");
            int NumberOfRows = 0;

            try
            {
                strConnection = _configuration.GetConnectionString("DefaultConnection");
                mySqlConnection = new SqlConnection(strConnection);
                await mySqlConnection.OpenAsync();
                mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandTimeout = 0;

                strCommand = new StringBuilder();
                strCommand.Append("UPDATE Categories ");
                strCommand.Append("SET IsDelete = 1 ");
                strCommand.Append("WHERE Id = @Id");
                mySqlCommand.CommandText = strCommand.ToString();

                mySqlCommand.Parameters.AddWithValue("@Id", id);

                NumberOfRows = await mySqlCommand.ExecuteNonQueryAsync();

                ViewBag.Message = "Delete Category success!";
                return RedirectToAction("Index", "Category");
            }
            catch(Exception e)
            {
                return View(e);
            }
        }
    }
}
