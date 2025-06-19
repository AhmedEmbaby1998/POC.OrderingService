using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POC.Migrations
{
    /// <inheritdoc />
    public partial class outOfBox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OutgoingEvents",
                table: "OutgoingEvents");

            migrationBuilder.RenameTable(
                name: "OutgoingEvents",
                newName: "AbpEventOutbox");

            migrationBuilder.AlterColumn<string>(
                name: "EventName",
                table: "AbpEventOutbox",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AbpEventOutbox",
                table: "AbpEventOutbox",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AbpEventOutbox_CreationTime",
                table: "AbpEventOutbox",
                column: "CreationTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AbpEventOutbox",
                table: "AbpEventOutbox");

            migrationBuilder.DropIndex(
                name: "IX_AbpEventOutbox_CreationTime",
                table: "AbpEventOutbox");

            migrationBuilder.RenameTable(
                name: "AbpEventOutbox",
                newName: "OutgoingEvents");

            migrationBuilder.AlterColumn<string>(
                name: "EventName",
                table: "OutgoingEvents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutgoingEvents",
                table: "OutgoingEvents",
                column: "Id");
        }
    }
}
