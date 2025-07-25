﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksBoard.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixBoardAccessRequestsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountName",
                table: "boardaccessrequests",
                type: "uuid",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "boardaccessrequests");
        }
    }
}
