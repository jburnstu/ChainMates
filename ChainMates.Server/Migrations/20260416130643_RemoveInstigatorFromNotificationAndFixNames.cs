using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChainMates.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInstigatorFromNotificationAndFixNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_notification_author_instigator_author_id",
                schema: "chain_mates",
                table: "notification");

            migrationBuilder.DropIndex(
                name: "ix_notification_instigator_author_id",
                schema: "chain_mates",
                table: "notification");

            migrationBuilder.DropColumn(
                name: "instigator_author_id",
                schema: "chain_mates",
                table: "notification");

            migrationBuilder.RenameColumn(
                name: "data",
                schema: "chain_mates",
                table: "notification",
                newName: "info");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "info",
                schema: "chain_mates",
                table: "notification",
                newName: "data");

            migrationBuilder.AddColumn<int>(
                name: "instigator_author_id",
                schema: "chain_mates",
                table: "notification",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_notification_instigator_author_id",
                schema: "chain_mates",
                table: "notification",
                column: "instigator_author_id");

            migrationBuilder.AddForeignKey(
                name: "fk_notification_author_instigator_author_id",
                schema: "chain_mates",
                table: "notification",
                column: "instigator_author_id",
                principalSchema: "chain_mates",
                principalTable: "author",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
