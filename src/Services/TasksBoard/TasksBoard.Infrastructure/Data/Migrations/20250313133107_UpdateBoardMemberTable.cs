using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksBoard.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBoardMemberTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BoardId",
                table: "boardmembers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_boardmembers_BoardId_AccountId",
                table: "boardmembers",
                columns: new[] { "BoardId", "AccountId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_boardmembers_boards_BoardId",
                table: "boardmembers",
                column: "BoardId",
                principalTable: "boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_boardmembers_boards_BoardId",
                table: "boardmembers");

            migrationBuilder.DropIndex(
                name: "IX_boardmembers_BoardId_AccountId",
                table: "boardmembers");

            migrationBuilder.DropColumn(
                name: "BoardId",
                table: "boardmembers");
        }
    }
}
