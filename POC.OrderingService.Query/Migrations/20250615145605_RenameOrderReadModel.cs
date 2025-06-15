using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POC.OrderingService.Query.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrderReadModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemReadModel_OrderReadModels_OrderId",
                table: "OrderItemReadModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderReadModels",
                table: "OrderReadModels");

            migrationBuilder.RenameTable(
                name: "OrderReadModels",
                newName: "OrderReadModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderReadModel",
                table: "OrderReadModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemReadModel_OrderReadModel_OrderId",
                table: "OrderItemReadModel",
                column: "OrderId",
                principalTable: "OrderReadModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemReadModel_OrderReadModel_OrderId",
                table: "OrderItemReadModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderReadModel",
                table: "OrderReadModel");

            migrationBuilder.RenameTable(
                name: "OrderReadModel",
                newName: "OrderReadModels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderReadModels",
                table: "OrderReadModels",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemReadModel_OrderReadModels_OrderId",
                table: "OrderItemReadModel",
                column: "OrderId",
                principalTable: "OrderReadModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
