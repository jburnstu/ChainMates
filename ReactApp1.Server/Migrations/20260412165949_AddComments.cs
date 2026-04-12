using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReactApp1.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "comment_status",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comment_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "comment_type",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comment_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "segment_comment",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comment_type_id = table.Column<int>(type: "integer", nullable: false),
                    segment_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_segment_comment", x => x.id);
                    table.ForeignKey(
                        name: "fk_segment_comment_segment_segment_id",
                        column: x => x.segment_id,
                        principalSchema: "chain_mates",
                        principalTable: "segment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "story_comment",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comment_type_id = table.Column<int>(type: "integer", nullable: false),
                    story_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_story_comment", x => x.id);
                    table.ForeignKey(
                        name: "fk_story_comment_story_story_id",
                        column: x => x.story_id,
                        principalSchema: "chain_mates",
                        principalTable: "story",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comment",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    author_id = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    comment_type_id = table.Column<int>(type: "integer", nullable: false),
                    comment_status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comment", x => x.id);
                    table.ForeignKey(
                        name: "fk_comment_author_author_id",
                        column: x => x.author_id,
                        principalSchema: "chain_mates",
                        principalTable: "author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_comment_comment_status_comment_status_id",
                        column: x => x.comment_status_id,
                        principalSchema: "chain_mates",
                        principalTable: "comment_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_comment_comment_type_comment_type_id",
                        column: x => x.comment_type_id,
                        principalSchema: "chain_mates",
                        principalTable: "comment_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comment_comment",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comment_type_id = table.Column<int>(type: "integer", nullable: false),
                    comment_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comment_comment", x => x.id);
                    table.ForeignKey(
                        name: "fk_comment_comment_comment_comment_id",
                        column: x => x.comment_id,
                        principalSchema: "chain_mates",
                        principalTable: "comment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comment_segment_comment",
                schema: "chain_mates",
                columns: table => new
                {
                    comments_id = table.Column<int>(type: "integer", nullable: false),
                    segment_comments_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comment_segment_comment", x => new { x.comments_id, x.segment_comments_id });
                    table.ForeignKey(
                        name: "fk_comment_segment_comment_comment_comments_id",
                        column: x => x.comments_id,
                        principalSchema: "chain_mates",
                        principalTable: "comment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_comment_segment_comment_segment_comment_segment_comments_id",
                        column: x => x.segment_comments_id,
                        principalSchema: "chain_mates",
                        principalTable: "segment_comment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comment_story_comment",
                schema: "chain_mates",
                columns: table => new
                {
                    comments_id = table.Column<int>(type: "integer", nullable: false),
                    story_comments_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comment_story_comment", x => new { x.comments_id, x.story_comments_id });
                    table.ForeignKey(
                        name: "fk_comment_story_comment_comment_comments_id",
                        column: x => x.comments_id,
                        principalSchema: "chain_mates",
                        principalTable: "comment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_comment_story_comment_story_comment_story_comments_id",
                        column: x => x.story_comments_id,
                        principalSchema: "chain_mates",
                        principalTable: "story_comment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comment_comment_comment",
                schema: "chain_mates",
                columns: table => new
                {
                    comment_comments_id = table.Column<int>(type: "integer", nullable: false),
                    comments_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comment_comment_comment", x => new { x.comment_comments_id, x.comments_id });
                    table.ForeignKey(
                        name: "fk_comment_comment_comment_comment_comment_comment_comments_id",
                        column: x => x.comment_comments_id,
                        principalSchema: "chain_mates",
                        principalTable: "comment_comment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_comment_comment_comment_comment_comments_id",
                        column: x => x.comments_id,
                        principalSchema: "chain_mates",
                        principalTable: "comment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_comment_author_id",
                schema: "chain_mates",
                table: "comment",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_comment_comment_status_id",
                schema: "chain_mates",
                table: "comment",
                column: "comment_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_comment_comment_type_id",
                schema: "chain_mates",
                table: "comment",
                column: "comment_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_comment_comment_comment_id",
                schema: "chain_mates",
                table: "comment_comment",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "ix_comment_comment_comment_comments_id",
                schema: "chain_mates",
                table: "comment_comment_comment",
                column: "comments_id");

            migrationBuilder.CreateIndex(
                name: "ix_comment_segment_comment_segment_comments_id",
                schema: "chain_mates",
                table: "comment_segment_comment",
                column: "segment_comments_id");

            migrationBuilder.CreateIndex(
                name: "ix_comment_story_comment_story_comments_id",
                schema: "chain_mates",
                table: "comment_story_comment",
                column: "story_comments_id");

            migrationBuilder.CreateIndex(
                name: "ix_segment_comment_segment_id",
                schema: "chain_mates",
                table: "segment_comment",
                column: "segment_id");

            migrationBuilder.CreateIndex(
                name: "ix_story_comment_story_id",
                schema: "chain_mates",
                table: "story_comment",
                column: "story_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comment_comment_comment",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "comment_segment_comment",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "comment_story_comment",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "comment_comment",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "segment_comment",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "story_comment",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "comment",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "comment_status",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "comment_type",
                schema: "chain_mates");
        }
    }
}
