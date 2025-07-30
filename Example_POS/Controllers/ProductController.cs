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

        public async Task<IActionResult> Search(string keyword, string? Delete, string? Category)
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
            int CategoryId = 0;
            string CategoryName = "";
            string Name = "";
            int Quantity = 0;
            decimal Price = 0;
            string Desc = "";
            int CreateBy = 0;
            DateTime CreateAt = DateTime.Now;
            int UpdateBy = 0;
            DateTime UpdateAt = DateTime.Now;
            int IsDelete = 0;

            ShowProduct? product = null;
            List<ShowProduct> AllProducts = [];

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
                strCommand.Append("SELECT TOP 5 ");
                strCommand.Append("Products.Id, ");
                strCommand.Append("Products.CategoryId, ");
                strCommand.Append("Categories.Name AS CategoryName, ");
                strCommand.Append("Products.Name, ");
                strCommand.Append("Products.Quantity, ");
                strCommand.Append("Products.Price, ");
                strCommand.Append("Products.Description, ");
                strCommand.Append("Products.CreateBy, ");
                strCommand.Append("Products.CreateTime, ");
                strCommand.Append("Products.UpdateBy, ");
                strCommand.Append("Products.UpdateTime, ");
                strCommand.Append("Products.IsDelete ");
                strCommand.Append("FROM Products ");
                strCommand.Append("LEFT JOIN Categories ON Products.CategoryId = Categories.Id ");
                strCommand.Append("WHERE Products.Name LIKE @Name ");
                if (Delete != null)
                {
                    strCommand.Append("AND Products.IsDelete = @Delete");
                }
                if (Category != null)
                {
                    strCommand.Append("AND Products.CategoryId = @CategoryId");
                }
                mySqlCommand.CommandText = strCommand.ToString();
                mySqlCommand.Parameters.AddWithValue("@Name", "%" + keyword + "%");
                if (Delete != null)
                {
                    mySqlCommand.Parameters.AddWithValue("@Delete", Delete);
                }
                if (Category != null)
                {
                    mySqlCommand.Parameters.AddWithValue("@CategoryId", Category);
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

                    ColumnName = "CategoryId";
                    CategoryId = Convert.ToInt32(ListItem[ColumnName]);

                    ColumnName = "CategoryName";
                    if (ListItem.IsNull(ColumnName))
                        CategoryName = "";
                    else
                        CategoryName = Convert.ToString(ListItem[ColumnName])!;

                    ColumnName = "Name";
                    if (ListItem.IsNull(ColumnName))
                        Name = "";
                    else
                        Name = Convert.ToString(ListItem[ColumnName])!;

                    ColumnName = "Quantity";
                    Quantity = Convert.ToInt32(ListItem[ColumnName]);

                    ColumnName = "Price";
                    Price = Convert.ToDecimal(ListItem[ColumnName]);

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

                    product = new()
                    {
                        Id = Id,
                        CategoryId = CategoryId,
                        CategoryName = CategoryName,
                        Name = Name,
                        Quantity = Quantity,
                        Price = Price,
                        Description = Desc,
                        CreateBy = CreateBy,
                        CreateTime = CreateAt,
                        UpdateBy = UpdateBy,
                        UpdateTime = UpdateAt,
                        IsDelete = IsDelete
                    };
                    AllProducts.Add(product);
                }

                return Ok(AllProducts);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

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

                // Check Create Name Duplicate
                sqlCheckDupName = mySqlConnection.CreateCommand();
                sqlCheckDupName.CommandTimeout = 0;
                strInsert = new StringBuilder();
                strInsert.Append("SELECT Id, Name FROM Products WHERE Name = @Name");
                sqlCheckDupName.CommandText = strInsert.ToString();
                sqlCheckDupName.Parameters.AddWithValue("@Name", newProductData.Name);
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

                // Create new product
                sqlInsertCommand = mySqlConnection.CreateCommand();
                sqlInsertCommand.CommandTimeout = 0;
                strInsert = new StringBuilder();
                strInsert.Append("INSERT INTO Products ");
                strInsert.Append("(CategoryId, Name, Quantity, Price, Description, CreateBy, CreateTime, UpdateBy, UpdateTime, IsDelete) ");
                strInsert.Append("VALUES (@CategoryId, @Name, @Quantity, @Price, @Description, @CreateBy, @CreateTime, @UpdateBy, @UpdateTime, @IsDelete)");
                sqlInsertCommand.CommandText = strInsert.ToString();
                sqlInsertCommand.Parameters.AddWithValue("@CategoryId", newProductData.CategoryId);
                sqlInsertCommand.Parameters.AddWithValue("@Name", newProductData.Name);
                sqlInsertCommand.Parameters.AddWithValue("@Quantity", newProductData.Quantity);
                sqlInsertCommand.Parameters.AddWithValue("@Price", newProductData.Price);
                sqlInsertCommand.Parameters.AddWithValue("@Description", newProductData.Description ?? "");
                sqlInsertCommand.Parameters.AddWithValue("@CreateBy", 0);
                sqlInsertCommand.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                sqlInsertCommand.Parameters.AddWithValue("@UpdateBy", 0);
                sqlInsertCommand.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                sqlInsertCommand.Parameters.AddWithValue("@IsDelete", 0);
                NumberOfRows = await sqlInsertCommand.ExecuteNonQueryAsync();

                return Ok("Add Product success!");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(EditProduct updateProductData)
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
                checkDupNameCommand.Append("SELECT Id, Name FROM Products WHERE Name = @Name AND Id != @Id");
                myCheckDupNameSqlCommand.CommandText = checkDupNameCommand.ToString();
                myCheckDupNameSqlCommand.Parameters.AddWithValue("@Name", updateProductData.Name);
                myCheckDupNameSqlCommand.Parameters.AddWithValue("@Id", updateProductData.Id);
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

                // Update product
                myInsertSqlCommand = mySqlConnection.CreateCommand();
                myInsertSqlCommand.CommandTimeout = 0;
                updateCommand = new StringBuilder();
                updateCommand.Append("UPDATE Products ");
                updateCommand.Append("SET CategoryId = @CategoryId, Name = @Name, Quantity = @Quantity, Price = @Price, Description = @Description, UpdateBy = @UpdateBy, UpdateTime = @UpdateTime, isDelete = @Delete ");
                updateCommand.Append("WHERE Id = @Id");
                myInsertSqlCommand.CommandText = updateCommand.ToString();
                myInsertSqlCommand.Parameters.AddWithValue("@CategoryId", updateProductData.CategoryId);
                myInsertSqlCommand.Parameters.AddWithValue("@Name", updateProductData.Name);
                myInsertSqlCommand.Parameters.AddWithValue("@Quantity", updateProductData.Quantity);
                myInsertSqlCommand.Parameters.AddWithValue("@Price", updateProductData.Price);
                myInsertSqlCommand.Parameters.AddWithValue("@Description", updateProductData.Description ?? "");
                myInsertSqlCommand.Parameters.AddWithValue("@UpdateBy", 0);
                myInsertSqlCommand.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                myInsertSqlCommand.Parameters.AddWithValue("@Id", updateProductData.Id);
                myInsertSqlCommand.Parameters.AddWithValue("@Delete", updateProductData.Delete);
                NumberOfRows = await myInsertSqlCommand.ExecuteNonQueryAsync();

                return Ok("Update Product success!");
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
                strCommand.Append("UPDATE Products ");
                strCommand.Append("SET IsDelete = 1, UpdateTime = @UpdateAt, UpdateBy = @UpdateBy ");
                strCommand.Append("WHERE Id = @Id");    
                mySqlCommand.CommandText = strCommand.ToString();
                mySqlCommand.Parameters.AddWithValue("@Id", id);
                mySqlCommand.Parameters.AddWithValue("@UpdateAt", DateTime.Now);
                mySqlCommand.Parameters.AddWithValue("@UpdateBy", 0);
                NumberOfRows = await mySqlCommand.ExecuteNonQueryAsync();

                return Ok("Delete Category success!");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
