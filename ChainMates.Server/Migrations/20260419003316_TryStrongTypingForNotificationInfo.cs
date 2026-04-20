using ChainMates.Server.DTOs.Notification.Info;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChainMates.Server.Migrations
{
    /// <inheritdoc />
    public partial class TryStrongTypingForNotificationInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<NotificationInfoDto>(
                name: "info",
                schema: "chain_mates",
                table: "notification",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "info",
                schema: "chain_mates",
                table: "notification",
                type: "text",
                nullable: false,
                oldClrType: typeof(NotificationInfoDto),
                oldType: "jsonb");
        }
    }
}
