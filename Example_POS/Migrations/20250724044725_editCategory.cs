using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Example_POS.Migrations
{
    /// <inheritdoc />
    public partial class editCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsDelete",
                table: "Categories",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Categories");
        }
    }
}
