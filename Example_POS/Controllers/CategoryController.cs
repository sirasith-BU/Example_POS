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
        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetAllCategories()
        {
            string? strConnection = string.Empty;
            SqlConnection? mySqlConnection = null;
            SqlCommand? mySqlCommand = null;
            StringBuilder? strCommand = new("");
            int NumberOfRows = 0;
            string ColumnName = "";
            SqlDataAdapter? mySqlDataAdapter = null; // many
            DataSet ListDataSet = new DataSet();

            int Id = 0;
            string Name = "";
            string Desc = "";
            int CreateBy = 0;
            DateTime CreateAt = DateTime.Now;
            int UpdateBy = 0;
            DateTime UpdateAt = DateTime.Now;
            int IsDelete = 0;

            Category? TempCat = null;
            List<Category> CatList = [];
            try
            {
                strConnection = _configuration.GetConnectionString("DefaultConnection");
                mySqlConnection = new SqlConnection(strConnection);
                await mySqlConnection.OpenAsync();

                mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandTimeout = 0;
                strCommand = new StringBuilder("");
                strCommand.Append("SELECT TOP 1000 * FROM Categories");
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

                //ViewBag.Category = CatList;
                return Json(CatList);
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

        public async Task<JsonResult> CatagoryOptions()
        {
            string? strConnection = string.Empty;
            SqlConnection? mySqlConnection = null;
            SqlCommand? mySqlCommand = null;
            StringBuilder? strCommand = new("");
            int NumberOfRows = 0;
            string ColumnName = "";
            SqlDataAdapter? mySqlDataAdapter = null; // many
            DataSet ListDataSet = new DataSet();

            int Id = 0;
            string Name = "";

            CategoryOptions? catagory = null;
            List<CategoryOptions> CatOptions = [];
            try
            {
                // Database Connection
                strConnection = _configuration.GetConnectionString("DefaultConnection");
                mySqlConnection = new SqlConnection(strConnection);
                await mySqlConnection.OpenAsync();

                // Get Id, Name
                mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandTimeout = 0;
                strCommand = new StringBuilder("");
                strCommand.Append("SELECT Id, Name FROM Categories WHERE IsDelete = 0");
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


                    catagory = new()
                    {
                        Id = Id,
                        Name = Name
                    };
                    CatOptions.Add(catagory);
                }

                //ViewBag.Category = CatList;
                return Json(CatOptions);
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddCategory newCatData)
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
                //strConnection = _configuration.GetConnectionString("DefaultConnection");
                mySqlConnection = new SqlConnection(strConnection);
                await mySqlConnection.OpenAsync();

                // Check Create Name Duplicate
                sqlCheckDupName = mySqlConnection.CreateCommand();
                sqlCheckDupName.CommandTimeout = 0;
                strInsert = new StringBuilder();
                strInsert.Append("SELECT Id, Name FROM Categories WHERE Name = @Name");
                sqlCheckDupName.CommandText = strInsert.ToString();
                sqlCheckDupName.Parameters.AddWithValue("@Name", newCatData.Name);
                mySqlDataReader = await sqlCheckDupName.ExecuteReaderAsync();
                if (await mySqlDataReader.ReadAsync())
                {
                    return BadRequest("Already have this name!");
                }
                if (mySqlDataReader != null)
                {
                    await mySqlDataReader.CloseAsync();
                    mySqlDataReader = null;
                }

                // Create new category
                sqlInsertCommand = mySqlConnection.CreateCommand();
                sqlInsertCommand.CommandTimeout = 0;
                strInsert = new StringBuilder();
                strInsert.Append("INSERT INTO Categories ");
                strInsert.Append("(Name, Description, CreateBy, CreateTime, UpdateBy, UpdateTime, IsDelete) ");
                strInsert.Append("VALUES (@Name, @Description, @CreateBy, @CreateTime, @UpdateBy, @UpdateTime, @IsDelete)");
                sqlInsertCommand.CommandText = strInsert.ToString();
                sqlInsertCommand.Parameters.AddWithValue("@Name", newCatData.Name);
                sqlInsertCommand.Parameters.AddWithValue("@Description", newCatData.Description ?? "");
                sqlInsertCommand.Parameters.AddWithValue("@CreateBy", 0);
                sqlInsertCommand.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                sqlInsertCommand.Parameters.AddWithValue("@UpdateBy", 0);
                sqlInsertCommand.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                sqlInsertCommand.Parameters.AddWithValue("@IsDelete", 0);
                NumberOfRows = await sqlInsertCommand.ExecuteNonQueryAsync();

                //ViewBag.Message = "Add Category success!";
                //return RedirectToAction("Index", "Category");
                return Ok("Add Category success!");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(EditCategory updateCatData)
        {
            string? strConnection = string.Empty;
            SqlConnection? mySqlConnection = null;
            SqlCommand? myInsertSqlCommand = null;
            StringBuilder? updateCommand = new("");
            SqlCommand? myCheckDupNameSqlCommand = null;
            StringBuilder? checkDupNameCommand = new("");
            SqlDataReader? mySqlDataReader = null; // one
            int NumberOfRows = 0;

            try
            {
                // Database Connection
                strConnection = _configuration.GetConnectionString("DefaultConnection");
                mySqlConnection = new SqlConnection(strConnection);
                await mySqlConnection.OpenAsync();

                // Check Update Name Duplicate
                myCheckDupNameSqlCommand = mySqlConnection.CreateCommand();
                myCheckDupNameSqlCommand.CommandTimeout = 0;
                checkDupNameCommand = new StringBuilder();
                checkDupNameCommand.Append("SELECT Id, Name FROM Categories WHERE Name = @Name AND Id != @Id");
                myCheckDupNameSqlCommand.CommandText = checkDupNameCommand.ToString();
                myCheckDupNameSqlCommand.Parameters.AddWithValue("@Name", updateCatData.Name);
                myCheckDupNameSqlCommand.Parameters.AddWithValue("@Id", updateCatData.Id);
                mySqlDataReader = await myCheckDupNameSqlCommand.ExecuteReaderAsync();
                if (await mySqlDataReader.ReadAsync())
                {
                    return BadRequest("Already have this name!");
                }
                if (mySqlDataReader != null)
                {
                    await mySqlDataReader.CloseAsync();
                    mySqlDataReader = null;
                }

                // Update category
                myInsertSqlCommand = mySqlConnection.CreateCommand();
                myInsertSqlCommand.CommandTimeout = 0;
                updateCommand = new StringBuilder();
                updateCommand.Append("UPDATE Categories ");
                updateCommand.Append("SET Name = @Name, Description = @Description, UpdateBy = @UpdateBy, UpdateTime = @UpdateTime, isDelete = @Delete ");
                updateCommand.Append("WHERE Id = @Id");
                myInsertSqlCommand.CommandText = updateCommand.ToString();
                myInsertSqlCommand.Parameters.AddWithValue("@Name", updateCatData.Name);
                myInsertSqlCommand.Parameters.AddWithValue("@Description", updateCatData.Description ?? "");
                myInsertSqlCommand.Parameters.AddWithValue("@UpdateBy", 0);
                myInsertSqlCommand.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                myInsertSqlCommand.Parameters.AddWithValue("@Id", updateCatData.Id);
                myInsertSqlCommand.Parameters.AddWithValue("@Delete", updateCatData.Delete);
                NumberOfRows = await myInsertSqlCommand.ExecuteNonQueryAsync();

                //ViewBag.Message = "Update Category success!";
                //return RedirectToAction("Index", "Category");
                return Ok("Update Category success!");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            string? strConnection = string.Empty;
            SqlConnection? mySqlConnection = null;
            SqlCommand? mySqlCommand = null;
            StringBuilder? strCommand = new("");
            int NumberOfRows = 0;

            try
            {
                // Database Connection
                strConnection = _configuration.GetConnectionString("DefaultConnection");
                mySqlConnection = new SqlConnection(strConnection);
                await mySqlConnection.OpenAsync();

                // Update category status = 1
                mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandTimeout = 0;
                strCommand = new StringBuilder();
                strCommand.Append("UPDATE Categories ");
                strCommand.Append("SET IsDelete = 1, UpdateTime = @UpdateAt, UpdateBy = @UpdateBy ");
                strCommand.Append("WHERE Id = @Id");
                mySqlCommand.CommandText = strCommand.ToString();
                mySqlCommand.Parameters.AddWithValue("@Id", id);
                mySqlCommand.Parameters.AddWithValue("@UpdateAt", DateTime.Now);
                mySqlCommand.Parameters.AddWithValue("@UpdateBy", 0);
                NumberOfRows = await mySqlCommand.ExecuteNonQueryAsync();

                //ViewBag.Message = "Delete Category success!";
                //return RedirectToAction("Index", "Category");
                return Ok("Delete Category success!");
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
        }

        public async Task<IActionResult> Search(string keyword, string? Delete) 
        {
            string? strConnection = string.Empty;
            SqlConnection? mySqlConnection = null;
            SqlCommand? mySqlCommand = null;
            StringBuilder? strCommand = new("");
            int NumberOfRows = 0;
            string ColumnName = "";
            SqlDataAdapter? mySqlDataAdapter = null; // many
            DataSet ListDataSet = new DataSet();

            int Id = 0;
            string Name = "";
            string Desc = "";
            int CreateBy = 0;
            DateTime CreateAt = DateTime.Now;
            int UpdateBy = 0;
            DateTime UpdateAt = DateTime.Now;
            int IsDelete = 0;

            Category? category = null;
            List<Category> AllCategories = [];

            try
            {
                // Database Connection
                strConnection = _configuration.GetConnectionString("DefaultConnection");
                mySqlConnection = new SqlConnection(strConnection);
                await mySqlConnection.OpenAsync();

                // Search by name
                mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandTimeout = 0;
                strCommand = new StringBuilder("");
                strCommand.Append("SELECT TOP 1000 * FROM Categories WHERE Name LIKE @Name ");
                if (Delete != null)
                {
                    strCommand.Append("AND isDelete = @Delete");
                }

                //strCommand.Append("SELECT TOP 5 * FROM Categories WHERE Name ='");
                //strCommand.Append(keyword);
                //strCommand.Append("'");

                mySqlCommand.CommandText = strCommand.ToString();
                mySqlCommand.Parameters.AddWithValue("@Name", "%" + keyword + "%");
                if (Delete != null)
                {
                    mySqlCommand.Parameters.AddWithValue("@Delete", Delete);
                }
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

                    ColumnName = "IsDelete";
                    IsDelete = Convert.ToInt32(ListItem[ColumnName]);

                    category = new()
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
                    AllCategories.Add(category);
                }

                //ViewBag.Category = AllCategories;
                return Ok(AllCategories);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
