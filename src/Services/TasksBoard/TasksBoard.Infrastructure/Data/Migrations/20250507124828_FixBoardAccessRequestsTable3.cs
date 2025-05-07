using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksBoard.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixBoardAccessRequestsTable3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountEmail",
                table: "boardaccessrequests",
                type: "text",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "boardaccessrequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountEmail",
                table: "boardaccessrequests");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "boardaccessrequests");
        }
    }
}
