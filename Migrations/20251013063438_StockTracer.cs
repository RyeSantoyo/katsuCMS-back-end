using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace katsuCMS_backend.Migrations
{
    /// <inheritdoc />
    public partial class StockTracer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryStocks_Products_ProductId",
                table: "InventoryStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryStocks_Units_UnitId",
                table: "InventoryStocks");

            migrationBuilder.AddColumn<decimal>(
                name: "PreferredStockLevel",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReoderLevel",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReorderLevel",
                table: "InventoryStocks",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "InventoryStocks",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdated",
                table: "InventoryStocks",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStocks_Id",
                table: "InventoryStocks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryStocks_Products_ProductId",
                table: "InventoryStocks",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryStocks_Units_UnitId",
                table: "InventoryStocks",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryStocks_Products_ProductId",
                table: "InventoryStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryStocks_Units_UnitId",
                table: "InventoryStocks");

            migrationBuilder.DropIndex(
                name: "IX_InventoryStocks_Id",
                table: "InventoryStocks");

            migrationBuilder.DropColumn(
                name: "PreferredStockLevel",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ReoderLevel",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "ReorderLevel",
                table: "InventoryStocks",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "InventoryStocks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdated",
                table: "InventoryStocks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryStocks_Products_ProductId",
                table: "InventoryStocks",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryStocks_Units_UnitId",
                table: "InventoryStocks",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
