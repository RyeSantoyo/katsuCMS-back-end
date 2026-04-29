using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace katsuCMS_backend.Migrations
{
    /// <inheritdoc />
    public partial class MinimalUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "ProductSuppliers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "ProductSuppliers");
        }
    }
}
