using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksBoard.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBoardImageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "boardimages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BoardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Extension = table.Column<string>(type: "text", nullable: false),
                    Image = table.Column<byte[]>(type: "bytea", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boardimages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_boardimages_boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_boardimages_BoardId",
                table: "boardimages",
                column: "BoardId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "boardimages");
        }
    }
}
