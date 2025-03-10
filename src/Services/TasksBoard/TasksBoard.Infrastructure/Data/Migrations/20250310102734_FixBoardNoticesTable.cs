using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksBoard.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixBoardNoticesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "boardnotices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "boardnotices",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
