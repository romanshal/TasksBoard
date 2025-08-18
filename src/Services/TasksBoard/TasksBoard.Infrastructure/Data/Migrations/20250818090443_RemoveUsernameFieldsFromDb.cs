using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksBoard.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUsernameFieldsFromDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "boardmembers");

            migrationBuilder.DropColumn(
                name: "FromAccountName",
                table: "boardinviterequests");

            migrationBuilder.DropColumn(
                name: "ToAccountEmail",
                table: "boardinviterequests");

            migrationBuilder.DropColumn(
                name: "ToAccountName",
                table: "boardinviterequests");

            migrationBuilder.DropColumn(
                name: "AccountEmail",
                table: "boardaccessrequests");

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "boardaccessrequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "boardmembers",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "default");

            migrationBuilder.AddColumn<string>(
                name: "FromAccountName",
                table: "boardinviterequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ToAccountEmail",
                table: "boardinviterequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ToAccountName",
                table: "boardinviterequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccountEmail",
                table: "boardaccessrequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "boardaccessrequests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
