using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailService.Infrastructure.Postgres.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixFieldsNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "to",
                table: "email_outbox",
                newName: "sender");

            migrationBuilder.RenameColumn(
                name: "from",
                table: "email_outbox",
                newName: "recipient");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "sender",
                table: "email_outbox",
                newName: "to");

            migrationBuilder.RenameColumn(
                name: "recipient",
                table: "email_outbox",
                newName: "from");
        }
    }
}
