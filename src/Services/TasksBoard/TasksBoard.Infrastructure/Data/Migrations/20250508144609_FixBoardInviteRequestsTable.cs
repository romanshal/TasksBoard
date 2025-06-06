using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksBoard.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixBoardInviteRequestsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ToAccountEmail",
                table: "boardinviterequests",
                type: "text",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "ToAccountName",
                table: "boardinviterequests",
                type: "text",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToAccountEmail",
                table: "boardinviterequests");

            migrationBuilder.DropColumn(
                name: "ToAccountName",
                table: "boardinviterequests");
        }
    }
}
