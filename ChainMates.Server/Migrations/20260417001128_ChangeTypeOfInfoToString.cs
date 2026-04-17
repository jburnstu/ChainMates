using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChainMates.Server.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTypeOfInfoToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "info",
                schema: "chain_mates",
                table: "notification",
                type: "text",
                nullable: false,
                oldClrType: typeof(JsonDocument),
                oldType: "jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<JsonDocument>(
                name: "info",
                schema: "chain_mates",
                table: "notification",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
