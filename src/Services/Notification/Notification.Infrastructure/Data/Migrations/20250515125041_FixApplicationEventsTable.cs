using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixApplicationEventsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_events_EventId",
                table: "events");

            migrationBuilder.CreateIndex(
                name: "IX_events_EventId",
                table: "events",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_events_EventId",
                table: "events");

            migrationBuilder.CreateIndex(
                name: "IX_events_EventId",
                table: "events",
                column: "EventId",
                unique: true);
        }
    }
}
