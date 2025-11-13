using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailService.Infrastructure.Postgres.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReconfigureKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_email_outbox",
                table: "email_outbox");

            migrationBuilder.DropIndex(
                name: "ix_email_outbox_message_id",
                table: "email_outbox");

            migrationBuilder.DropColumn(
                name: "id",
                table: "email_outbox");

            migrationBuilder.AddPrimaryKey(
                name: "pk_email_outbox",
                table: "email_outbox",
                column: "message_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_email_outbox",
                table: "email_outbox");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "email_outbox",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "pk_email_outbox",
                table: "email_outbox",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_email_outbox_message_id",
                table: "email_outbox",
                column: "message_id",
                unique: true);
        }
    }
}
