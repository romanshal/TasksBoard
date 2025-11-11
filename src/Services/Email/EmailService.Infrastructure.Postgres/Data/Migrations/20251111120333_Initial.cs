using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmailService.Infrastructure.Postgres.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "outbox_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "email_outbox",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    attempts = table.Column<int>(type: "integer", nullable: false),
                    next_attempt_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    last_error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    message_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    to = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    from = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    subject = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    is_html = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_email_outbox", x => x.id);
                    table.ForeignKey(
                        name: "fk_email_outbox_email_outbox_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "outbox_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "outbox_statuses",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 0, "Pending" },
                    { 1, "InProgress" },
                    { 2, "Sent" },
                    { 3, "Failed" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_email_outbox_message_id",
                table: "email_outbox",
                column: "message_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_email_outbox_status_id_next_attempt_at",
                table: "email_outbox",
                columns: new[] { "status_id", "next_attempt_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "email_outbox");

            migrationBuilder.DropTable(
                name: "outbox_statuses");
        }
    }
}
