using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedOrdersToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_order_products_orders_order_id",
                table: "OrderProducts");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "OrderProducts",
                newName: "orders_id");

            migrationBuilder.AddForeignKey(
                name: "fk_order_products_orders_orders_id",
                table: "OrderProducts",
                column: "orders_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_order_products_orders_orders_id",
                table: "OrderProducts");

            migrationBuilder.RenameColumn(
                name: "orders_id",
                table: "OrderProducts",
                newName: "order_id");

            migrationBuilder.AddForeignKey(
                name: "fk_order_products_orders_order_id",
                table: "OrderProducts",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
