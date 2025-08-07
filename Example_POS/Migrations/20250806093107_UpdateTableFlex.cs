using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Example_POS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableFlex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SysFlex",
                columns: table => new
                {
                    FlexId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlexCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FlexName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<int>(type: "int", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelete = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysFlex", x => x.FlexId);
                });

            migrationBuilder.CreateTable(
                name: "SysFlexItem",
                columns: table => new
                {
                    FlexItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlexId = table.Column<int>(type: "int", nullable: false),
                    FlexItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FlexItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<int>(type: "int", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelete = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysFlexItem", x => x.FlexItemId);
                    table.ForeignKey(
                        name: "FK_SysFlexItem_SysFlex_FlexId",
                        column: x => x.FlexId,
                        principalTable: "SysFlex",
                        principalColumn: "FlexId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SysFlex_FlexCode",
                table: "SysFlex",
                column: "FlexCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SysFlexItem_FlexId",
                table: "SysFlexItem",
                column: "FlexId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SysFlexItem");

            migrationBuilder.DropTable(
                name: "SysFlex");
        }
    }
}
