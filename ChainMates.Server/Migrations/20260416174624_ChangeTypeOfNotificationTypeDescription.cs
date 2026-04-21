using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChainMates.Server.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTypeOfNotificationTypeDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "description",
                schema: "chain_mates",
                table: "notification_type",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "description",
                schema: "chain_mates",
                table: "notification_type",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
