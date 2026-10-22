using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotifyX.Api.Data.Migrations;

/// <inheritdoc />
public partial class _20261022192343_IdempotencyKeyTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "IdempotencyKeys",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Key = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                RequestHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                NotificationId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IdempotencyKeys", x => x.Id);
                table.ForeignKey(
                    name: "FK_IdempotencyKeys_Notifications_NotificationId",
                    column: x => x.NotificationId,
                    principalTable: "Notifications",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_IdempotencyKeys_Notifications_NotificationId1",
                    column: x => x.NotificationId1,
                    principalTable: "Notifications",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_IdempotencyKeys_NotificationId",
            table: "IdempotencyKeys",
            column: "NotificationId");

        migrationBuilder.CreateIndex(
            name: "IX_IdempotencyKeys_NotificationId1",
            table: "IdempotencyKeys",
            column: "NotificationId1");

        migrationBuilder.CreateIndex(
            name: "IX_IdempotencyKeys_UserId_Key",
            table: "IdempotencyKeys",
            columns: new[] { "UserId", "Key" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "IdempotencyKeys");
    }
}
