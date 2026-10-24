using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotifyX.Api.Data.Migrations;

/// <inheritdoc />
public partial class _20261024134731_AddOutboxMessages : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "OutboxMessages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EventType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                ProcessedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                Attempts = table.Column<int>(type: "int", nullable: false),
                NextAttemptAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                LastError = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                DeadLetteredAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OutboxMessages", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_OutboxMessages_ProcessedAtUtc_DeadLetteredAtUtc_NextAttemptAtUtc",
            table: "OutboxMessages",
            columns: new[] { "ProcessedAtUtc", "DeadLetteredAtUtc", "NextAttemptAtUtc" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "OutboxMessages");
    }
}
