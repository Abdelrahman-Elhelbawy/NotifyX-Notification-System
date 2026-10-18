using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotifyX.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConstraintsForNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReadAtUtc",
                table: "Notifications",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Notifications_ReadState",
                table: "Notifications",
                sql: "([IsRead] = 0 AND [ReadAtUtc] IS NULL) OR ([IsRead] = 1 AND [ReadAtUtc] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Notifications_Type",
                table: "Notifications",
                sql: "[Type] IN (1, 2, 3, 4)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Notifications_ReadState",
                table: "Notifications");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Notifications_Type",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ReadAtUtc",
                table: "Notifications");
        }
    }
}
