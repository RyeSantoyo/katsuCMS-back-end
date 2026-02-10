using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace katsuCMS_backend.Migrations
{
    /// <inheritdoc />
    public partial class PODetailAdjustment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InventoryStockId",
                table: "PurchaseOrderDetails",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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
    }
}
