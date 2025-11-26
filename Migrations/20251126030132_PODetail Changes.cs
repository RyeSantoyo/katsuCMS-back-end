using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace katsuCMS_backend.Migrations
{
    /// <inheritdoc />
    public partial class PODetailChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "PurchaseOrders",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AddColumn<int>(
                name: "InventoryStockId",
                table: "PurchaseOrderDetails",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PONumber",
                table: "PurchaseOrderDetails",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "PurchaseOrderDetails",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PurchaseOrderNumber",
                table: "PurchaseOrderDetails",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetails_InventoryStockId",
                table: "PurchaseOrderDetails",
                column: "InventoryStockId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderDetails_InventoryStocks_InventoryStockId",
                table: "PurchaseOrderDetails",
                column: "InventoryStockId",
                principalTable: "InventoryStocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderDetails_InventoryStocks_InventoryStockId",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderDetails_InventoryStockId",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropColumn(
                name: "InventoryStockId",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropColumn(
                name: "PONumber",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderNumber",
                table: "PurchaseOrderDetails");

            migrationBuilder.AlterColumn<double>(
                name: "TotalAmount",
                table: "PurchaseOrders",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");
        }
    }
}
