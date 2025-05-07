using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksBoard.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixBoardAccessRequestsTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "boardaccessrequests",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "AccountName",
                table: "boardaccessrequests",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
