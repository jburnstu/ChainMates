using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChainMates.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddMessagesTagsAndLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "like_type",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_like_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "message",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    content = table.Column<string>(type: "text", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    author_id = table.Column<int>(type: "integer", nullable: false),
                    receiving_author_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_message", x => x.id);
                    table.ForeignKey(
                        name: "fk_message_author_author_id",
                        column: x => x.author_id,
                        principalSchema: "chain_mates",
                        principalTable: "author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_message_author_receiving_author_id",
                        column: x => x.receiving_author_id,
                        principalSchema: "chain_mates",
                        principalTable: "author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tag",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    author_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tag", x => x.id);
                    table.ForeignKey(
                        name: "fk_tag_author_author_id",
                        column: x => x.author_id,
                        principalSchema: "chain_mates",
                        principalTable: "author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "like",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    author_id = table.Column<int>(type: "integer", nullable: false),
                    like_type_id = table.Column<int>(type: "integer", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_like", x => x.id);
                    table.UniqueConstraint("ak_like_id_like_type_id", x => new { x.id, x.like_type_id });
                    table.ForeignKey(
                        name: "fk_like_author_author_id",
                        column: x => x.author_id,
                        principalSchema: "chain_mates",
                        principalTable: "author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_like_like_type_like_type_id",
                        column: x => x.like_type_id,
                        principalSchema: "chain_mates",
                        principalTable: "like_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tag_assignment",
                schema: "chain_mates",
                columns: table => new
                {
                    author_id = table.Column<int>(type: "integer", nullable: false),
                    tag_id = table.Column<int>(type: "integer", nullable: false),
                    segment_id = table.Column<int>(type: "integer", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tag_assignment", x => new { x.author_id, x.tag_id, x.segment_id });
                    table.ForeignKey(
                        name: "fk_tag_assignment_author_author_id",
                        column: x => x.author_id,
                        principalSchema: "chain_mates",
                        principalTable: "author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tag_assignment_segment_segment_id",
                        column: x => x.segment_id,
                        principalSchema: "chain_mates",
                        principalTable: "segment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tag_assignment_tag_tag_id",
                        column: x => x.tag_id,
                        principalSchema: "chain_mates",
                        principalTable: "tag",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "segment_like",
                schema: "chain_mates",
                columns: table => new
                {
                    like_id = table.Column<int>(type: "integer", nullable: false),
                    like_type_id = table.Column<int>(type: "integer", nullable: false),
                    parent_segment_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_segment_like", x => new { x.like_id, x.like_type_id });
                    table.ForeignKey(
                        name: "fk_segment_like_like_like_id_like_type_id",
                        columns: x => new { x.like_id, x.like_type_id },
                        principalSchema: "chain_mates",
                        principalTable: "like",
                        principalColumns: new[] { "id", "like_type_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_segment_like_like_type_like_type_id",
                        column: x => x.like_type_id,
                        principalSchema: "chain_mates",
                        principalTable: "like_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_segment_like_segment_parent_segment_id",
                        column: x => x.parent_segment_id,
                        principalSchema: "chain_mates",
                        principalTable: "segment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_story_comment_comment_type_id",
                schema: "chain_mates",
                table: "story_comment",
                column: "comment_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_segment_comment_comment_type_id",
                schema: "chain_mates",
                table: "segment_comment",
                column: "comment_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_comment_comment_comment_type_id",
                schema: "chain_mates",
                table: "comment_comment",
                column: "comment_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_like_author_id",
                schema: "chain_mates",
                table: "like",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_like_like_type_id",
                schema: "chain_mates",
                table: "like",
                column: "like_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_author_id",
                schema: "chain_mates",
                table: "message",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_receiving_author_id",
                schema: "chain_mates",
                table: "message",
                column: "receiving_author_id");

            migrationBuilder.CreateIndex(
                name: "ix_segment_like_like_type_id",
                schema: "chain_mates",
                table: "segment_like",
                column: "like_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_segment_like_parent_segment_id",
                schema: "chain_mates",
                table: "segment_like",
                column: "parent_segment_id");

            migrationBuilder.CreateIndex(
                name: "ix_tag_author_id",
                schema: "chain_mates",
                table: "tag",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_tag_assignment_segment_id",
                schema: "chain_mates",
                table: "tag_assignment",
                column: "segment_id");

            migrationBuilder.CreateIndex(
                name: "ix_tag_assignment_tag_id",
                schema: "chain_mates",
                table: "tag_assignment",
                column: "tag_id");

            migrationBuilder.AddForeignKey(
                name: "fk_comment_comment_comment_type_comment_type_id",
                schema: "chain_mates",
                table: "comment_comment",
                column: "comment_type_id",
                principalSchema: "chain_mates",
                principalTable: "comment_type",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_segment_comment_comment_type_comment_type_id",
                schema: "chain_mates",
                table: "segment_comment",
                column: "comment_type_id",
                principalSchema: "chain_mates",
                principalTable: "comment_type",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_story_comment_comment_type_comment_type_id",
                schema: "chain_mates",
                table: "story_comment",
                column: "comment_type_id",
                principalSchema: "chain_mates",
                principalTable: "comment_type",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comment_comment_comment_type_comment_type_id",
                schema: "chain_mates",
                table: "comment_comment");

            migrationBuilder.DropForeignKey(
                name: "fk_segment_comment_comment_type_comment_type_id",
                schema: "chain_mates",
                table: "segment_comment");

            migrationBuilder.DropForeignKey(
                name: "fk_story_comment_comment_type_comment_type_id",
                schema: "chain_mates",
                table: "story_comment");

            migrationBuilder.DropTable(
                name: "message",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "segment_like",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "tag_assignment",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "like",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "tag",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "like_type",
                schema: "chain_mates");

            migrationBuilder.DropIndex(
                name: "ix_story_comment_comment_type_id",
                schema: "chain_mates",
                table: "story_comment");

            migrationBuilder.DropIndex(
                name: "ix_segment_comment_comment_type_id",
                schema: "chain_mates",
                table: "segment_comment");

            migrationBuilder.DropIndex(
                name: "ix_comment_comment_comment_type_id",
                schema: "chain_mates",
                table: "comment_comment");
        }
    }
}
