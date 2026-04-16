using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReactApp1.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialRecreateWithSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "chain_mates");

            migrationBuilder.CreateTable(
                name: "author",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    display_name = table.Column<string>(type: "text", nullable: false),
                    email_address = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_author", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "segment_status",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_segment_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "story",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: true),
                    max_segments = table.Column<int>(type: "integer", nullable: true),
                    max_segment_length = table.Column<int>(type: "integer", nullable: true),
                    min_segment_length = table.Column<int>(type: "integer", nullable: true),
                    max_branches = table.Column<int>(type: "integer", nullable: true),
                    is_it_mature = table.Column<bool>(type: "boolean", nullable: true),
                    author_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_story", x => x.id);
                    table.ForeignKey(
                        name: "fk_story_author_author_id",
                        column: x => x.author_id,
                        principalSchema: "chain_mates",
                        principalTable: "author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "segment",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    content = table.Column<string>(type: "text", nullable: false),
                    segment_status_id = table.Column<int>(type: "integer", nullable: false),
                    author_id = table.Column<int>(type: "integer", nullable: false),
                    story_id = table.Column<int>(type: "integer", nullable: false),
                    previous_segment_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_segment", x => x.id);
                    table.ForeignKey(
                        name: "fk_segment_author_author_id",
                        column: x => x.author_id,
                        principalSchema: "chain_mates",
                        principalTable: "author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_segment_segment_previous_segment_id",
                        column: x => x.previous_segment_id,
                        principalSchema: "chain_mates",
                        principalTable: "segment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_segment_segment_status_segment_status_id",
                        column: x => x.segment_status_id,
                        principalSchema: "chain_mates",
                        principalTable: "segment_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_segment_story_story_id",
                        column: x => x.story_id,
                        principalSchema: "chain_mates",
                        principalTable: "story",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "moderation_assignment",
                schema: "chain_mates",
                columns: table => new
                {
                    author_id = table.Column<int>(type: "integer", nullable: false),
                    segment_id = table.Column<int>(type: "integer", nullable: false),
                    is_closed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_moderation_assignment", x => new { x.author_id, x.segment_id });
                    table.ForeignKey(
                        name: "fk_moderation_assignment_author_author_id",
                        column: x => x.author_id,
                        principalSchema: "chain_mates",
                        principalTable: "author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_moderation_assignment_segment_segment_id",
                        column: x => x.segment_id,
                        principalSchema: "chain_mates",
                        principalTable: "segment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_moderation_assignment_segment_id",
                schema: "chain_mates",
                table: "moderation_assignment",
                column: "segment_id");

            migrationBuilder.CreateIndex(
                name: "ix_segment_author_id",
                schema: "chain_mates",
                table: "segment",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_segment_previous_segment_id",
                schema: "chain_mates",
                table: "segment",
                column: "previous_segment_id");

            migrationBuilder.CreateIndex(
                name: "ix_segment_segment_status_id",
                schema: "chain_mates",
                table: "segment",
                column: "segment_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_segment_story_id",
                schema: "chain_mates",
                table: "segment",
                column: "story_id");

            migrationBuilder.CreateIndex(
                name: "ix_story_author_id",
                schema: "chain_mates",
                table: "story",
                column: "author_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "moderation_assignment",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "segment",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "segment_status",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "story",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "author",
                schema: "chain_mates");
        }
    }
}
