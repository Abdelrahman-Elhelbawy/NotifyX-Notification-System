using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotifyX.Api.Data.Migrations;

/// <inheritdoc />
public partial class _20261022192927_FixIdempotencyNotificationRelation : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_IdempotencyKeys_Notifications_NotificationId1",
            table: "IdempotencyKeys");

        migrationBuilder.DropIndex(
            name: "IX_IdempotencyKeys_NotificationId1",
            table: "IdempotencyKeys");

        migrationBuilder.DropColumn(
            name: "NotificationId1",
            table: "IdempotencyKeys");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "NotificationId1",
            table: "IdempotencyKeys",
            type: "uniqueidentifier",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_IdempotencyKeys_NotificationId1",
            table: "IdempotencyKeys",
            column: "NotificationId1");

        migrationBuilder.AddForeignKey(
            name: "FK_IdempotencyKeys_Notifications_NotificationId1",
            table: "IdempotencyKeys",
            column: "NotificationId1",
            principalTable: "Notifications",
            principalColumn: "Id");
    }
}
