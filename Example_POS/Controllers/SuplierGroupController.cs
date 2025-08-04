using Example_POS.DTOs.SuplierGroup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text;

namespace Example_POS.Controllers
{
    public class SuplierGroupController : Controller
    {
        private readonly IConfiguration _configuration;

        public SuplierGroupController(IConfiguration configuration)
        {
            _configuration = configuration;
            Console.WriteLine("Connection String: " + _configuration.GetConnectionString("DefaultConnection"));
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create(AddSuplierGroup newSuplierGroup)
        {
            string? strConnection = string.Empty;
            SqlConnection mySqlConnection = null;

            SqlCommand sqlCheckDupName = null;
            StringBuilder strCheckDupName = new("");

            SqlCommand sqlInsertCommand = null;
            StringBuilder strInsert = new("");

            int NumberOfRows = 0;
            SqlDataReader mySqlDataReader = null;

            try
            {
                // Database Connection
                strConnection = _configuration.GetConnectionString("DefaultConnection");
                mySqlConnection = new SqlConnection(strConnection);
                await mySqlConnection.OpenAsync();

                // Check Duplicate Name
                sqlCheckDupName = mySqlConnection.CreateCommand();
                sqlCheckDupName.CommandTimeout = 0;
                strInsert = new StringBuilder();
                strInsert.Append("SELECT Id, Name FROM Categories WHERE Name = @Name");
                sqlCheckDupName.CommandText = strInsert.ToString();
                sqlCheckDupName.Parameters.AddWithValue("@Name", newSuplierGroup.Name);
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

                // Create new SuplierGroup
                sqlInsertCommand = mySqlConnection.CreateCommand();
                sqlInsertCommand.CommandTimeout = 0;
                strInsert = new StringBuilder();
                strInsert.Append("INSERT INTO SuplierGroup ");
                strInsert.Append("(Name, Detail, Comment, Status, CreateBy, CreateTime, UpdateBy, UpdateTime) ");
                strInsert.Append("VALUES (@Name, @Detail, @Comment, @Status, @CreateBy, @CreateTime, @UpdateBy, @UpdateTime)");
                sqlInsertCommand.CommandText = strInsert.ToString();
                sqlInsertCommand.Parameters.AddWithValue("@Name", newSuplierGroup.Name);
                sqlInsertCommand.Parameters.AddWithValue("@Detail", newSuplierGroup.Detail ?? "");
                sqlInsertCommand.Parameters.AddWithValue("@Comment", newSuplierGroup.Comment ?? "");
                sqlInsertCommand.Parameters.AddWithValue("@Status", 0);
                sqlInsertCommand.Parameters.AddWithValue("@CreateBy", 0);
                sqlInsertCommand.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                sqlInsertCommand.Parameters.AddWithValue("@UpdateBy", 0);
                sqlInsertCommand.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                sqlInsertCommand.Parameters.AddWithValue("@IsDelete", 0);
                NumberOfRows = await sqlInsertCommand.ExecuteNonQueryAsync();

                return Ok("Add Suplier Group success!");

            }
            catch(SqlException ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
                return BadRequest("Database error occurred.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return BadRequest("An error occurred while processing your request.");
            }
            finally
            {
                if (mySqlDataReader != null)
                {
                    await mySqlDataReader.CloseAsync();
                    mySqlDataReader = null;
                }
                if (mySqlConnection != null && mySqlConnection.State == System.Data.ConnectionState.Open)
                {
                    await mySqlConnection.CloseAsync();
                    mySqlConnection.Dispose();
                    mySqlConnection = null;
                }
            }

        }
    }
}
