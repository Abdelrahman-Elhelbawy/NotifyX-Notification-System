using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotifyX.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationRecipientTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Notifications_ReadState",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ReadAtUtc",
                table: "Notifications");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Notifications",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NotificationRecipients",
                columns: table => new
                {
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    ReadAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationRecipients", x => new { x.NotificationId, x.UserId });
                    table.CheckConstraint("CK_Notifications_ReadState", "([IsRead] = 0 AND [ReadAtUtc] IS NULL) OR ([IsRead] = 1 AND [ReadAtUtc] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_NotificationRecipients_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationRecipients");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Notifications");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReadAtUtc",
                table: "Notifications",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Notifications_ReadState",
                table: "Notifications",
                sql: "([IsRead] = 0 AND [ReadAtUtc] IS NULL) OR ([IsRead] = 1 AND [ReadAtUtc] IS NOT NULL)");
        }
    }
}
