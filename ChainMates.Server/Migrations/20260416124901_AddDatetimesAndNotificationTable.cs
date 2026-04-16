using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChainMates.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddDatetimesAndNotificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                schema: "chain_mates",
                table: "story",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                schema: "chain_mates",
                table: "segment",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                schema: "chain_mates",
                table: "moderation_assignment",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                schema: "chain_mates",
                table: "comment",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                schema: "chain_mates",
                table: "circle_assignment",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                schema: "chain_mates",
                table: "circle",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                schema: "chain_mates",
                table: "author_relation",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                schema: "chain_mates",
                table: "author",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "notification_type",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    notification_type_id = table.Column<int>(type: "integer", nullable: false),
                    recipient_author_id = table.Column<int>(type: "integer", nullable: false),
                    instigator_author_id = table.Column<int>(type: "integer", nullable: false),
                    data = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification", x => x.id);
                    table.ForeignKey(
                        name: "fk_notification_author_instigator_author_id",
                        column: x => x.instigator_author_id,
                        principalSchema: "chain_mates",
                        principalTable: "author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_notification_author_recipient_author_id",
                        column: x => x.recipient_author_id,
                        principalSchema: "chain_mates",
                        principalTable: "author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_notification_notification_type_notification_type_id",
                        column: x => x.notification_type_id,
                        principalSchema: "chain_mates",
                        principalTable: "notification_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_notification_instigator_author_id",
                schema: "chain_mates",
                table: "notification",
                column: "instigator_author_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_notification_type_id",
                schema: "chain_mates",
                table: "notification",
                column: "notification_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_recipient_author_id",
                schema: "chain_mates",
                table: "notification",
                column: "recipient_author_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "notification_type",
                schema: "chain_mates");

            migrationBuilder.DropColumn(
                name: "date_created",
                schema: "chain_mates",
                table: "story");

            migrationBuilder.DropColumn(
                name: "date_created",
                schema: "chain_mates",
                table: "segment");

            migrationBuilder.DropColumn(
                name: "date_created",
                schema: "chain_mates",
                table: "moderation_assignment");

            migrationBuilder.DropColumn(
                name: "date_created",
                schema: "chain_mates",
                table: "comment");

            migrationBuilder.DropColumn(
                name: "date_created",
                schema: "chain_mates",
                table: "circle_assignment");

            migrationBuilder.DropColumn(
                name: "date_created",
                schema: "chain_mates",
                table: "circle");

            migrationBuilder.DropColumn(
                name: "date_created",
                schema: "chain_mates",
                table: "author_relation");

            migrationBuilder.DropColumn(
                name: "date_created",
                schema: "chain_mates",
                table: "author");
        }
    }
}
