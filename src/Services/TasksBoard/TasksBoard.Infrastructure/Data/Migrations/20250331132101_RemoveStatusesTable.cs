using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksBoard.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStatusesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_boardnotices_boardnoticestatuses_NoticeStatusId",
                table: "boardnotices");

            migrationBuilder.DropTable(
                name: "boardnoticestatuses");

            migrationBuilder.DropIndex(
                name: "IX_boardnotices_NoticeStatusId",
                table: "boardnotices");

            migrationBuilder.DropColumn(
                name: "NoticeStatusId",
                table: "boardnotices");

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "boardnotices",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "boardnotices");

            migrationBuilder.AddColumn<Guid>(
                name: "NoticeStatusId",
                table: "boardnotices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "boardnoticestatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boardnoticestatuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_boardnotices_NoticeStatusId",
                table: "boardnotices",
                column: "NoticeStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_boardnoticestatuses_Name",
                table: "boardnoticestatuses",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_boardnotices_boardnoticestatuses_NoticeStatusId",
                table: "boardnotices",
                column: "NoticeStatusId",
                principalTable: "boardnoticestatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
