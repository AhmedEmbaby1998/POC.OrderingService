using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POC.Migrations
{
    /// <inheritdoc />
    public partial class StoredEventAuditingAndCorreltionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrelationId",
                table: "StoredEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "StoredEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "StoredEvents");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StoredEvents");
        }
    }
}
