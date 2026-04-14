using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChainMates.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentOneToOneOptionalRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_segment_comment_segment_segment_id",
                schema: "chain_mates",
                table: "segment_comment");

            migrationBuilder.DropForeignKey(
                name: "fk_story_comment_story_story_id",
                schema: "chain_mates",
                table: "story_comment");

            migrationBuilder.DropTable(
                name: "comment_comment_comment",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "comment_segment_comment",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "comment_story_comment",
                schema: "chain_mates");

            migrationBuilder.DropPrimaryKey(
                name: "pk_story_comment",
                schema: "chain_mates",
                table: "story_comment");

            migrationBuilder.DropPrimaryKey(
                name: "pk_segment_comment",
                schema: "chain_mates",
                table: "segment_comment");

            migrationBuilder.DropPrimaryKey(
                name: "pk_comment_comment",
                schema: "chain_mates",
                table: "comment_comment");

            migrationBuilder.DropIndex(
                name: "ix_comment_comment_comment_id",
                schema: "chain_mates",
                table: "comment_comment");

            migrationBuilder.RenameColumn(
                name: "story_id",
                schema: "chain_mates",
                table: "story_comment",
                newName: "parent_story_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "chain_mates",
                table: "story_comment",
                newName: "comment_id");

            migrationBuilder.RenameIndex(
                name: "ix_story_comment_story_id",
                schema: "chain_mates",
                table: "story_comment",
                newName: "ix_story_comment_parent_story_id");

            migrationBuilder.RenameColumn(
                name: "segment_id",
                schema: "chain_mates",
                table: "segment_comment",
                newName: "parent_segment_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "chain_mates",
                table: "segment_comment",
                newName: "comment_id");

            migrationBuilder.RenameIndex(
                name: "ix_segment_comment_segment_id",
                schema: "chain_mates",
                table: "segment_comment",
                newName: "ix_segment_comment_parent_segment_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "chain_mates",
                table: "comment_comment",
                newName: "parent_comment_id");

            migrationBuilder.AlterColumn<int>(
                name: "comment_id",
                schema: "chain_mates",
                table: "story_comment",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "comment_id",
                schema: "chain_mates",
                table: "segment_comment",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "parent_comment_id",
                schema: "chain_mates",
                table: "comment_comment",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "comment_comment_comment_id",
                schema: "chain_mates",
                table: "comment",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "comment_comment_comment_type_id",
                schema: "chain_mates",
                table: "comment",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "segment_comment_comment_id",
                schema: "chain_mates",
                table: "comment",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "segment_comment_comment_type_id",
                schema: "chain_mates",
                table: "comment",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "story_comment_comment_id",
                schema: "chain_mates",
                table: "comment",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "story_comment_comment_type_id",
                schema: "chain_mates",
                table: "comment",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_story_comment",
                schema: "chain_mates",
                table: "story_comment",
                columns: new[] { "comment_id", "comment_type_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_segment_comment",
                schema: "chain_mates",
                table: "segment_comment",
                columns: new[] { "comment_id", "comment_type_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_comment_comment",
                schema: "chain_mates",
                table: "comment_comment",
                columns: new[] { "comment_id", "comment_type_id" });

            migrationBuilder.AddUniqueConstraint(
                name: "ak_comment_id_comment_type_id",
                schema: "chain_mates",
                table: "comment",
                columns: new[] { "id", "comment_type_id" });

            migrationBuilder.CreateIndex(
                name: "ix_comment_comment_comment_comment_id_comment_comment_comment_",
                schema: "chain_mates",
                table: "comment",
                columns: new[] { "comment_comment_comment_id", "comment_comment_comment_type_id" });

            migrationBuilder.CreateIndex(
                name: "ix_comment_segment_comment_comment_id_segment_comment_comment_",
                schema: "chain_mates",
                table: "comment",
                columns: new[] { "segment_comment_comment_id", "segment_comment_comment_type_id" });

            migrationBuilder.CreateIndex(
                name: "ix_comment_story_comment_comment_id_story_comment_comment_type",
                schema: "chain_mates",
                table: "comment",
                columns: new[] { "story_comment_comment_id", "story_comment_comment_type_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_comment_comment_comment_comment_comment_comment_id_comment_",
                schema: "chain_mates",
                table: "comment",
                columns: new[] { "comment_comment_comment_id", "comment_comment_comment_type_id" },
                principalSchema: "chain_mates",
                principalTable: "comment_comment",
                principalColumns: new[] { "comment_id", "comment_type_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_comment_segment_comment_segment_comment_comment_id_segment_",
                schema: "chain_mates",
                table: "comment",
                columns: new[] { "segment_comment_comment_id", "segment_comment_comment_type_id" },
                principalSchema: "chain_mates",
                principalTable: "segment_comment",
                principalColumns: new[] { "comment_id", "comment_type_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_comment_story_comment_story_comment_comment_id_story_commen",
                schema: "chain_mates",
                table: "comment",
                columns: new[] { "story_comment_comment_id", "story_comment_comment_type_id" },
                principalSchema: "chain_mates",
                principalTable: "story_comment",
                principalColumns: new[] { "comment_id", "comment_type_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_comment_comment_comment_comment_id_comment_type_id",
                schema: "chain_mates",
                table: "comment_comment",
                columns: new[] { "comment_id", "comment_type_id" },
                principalSchema: "chain_mates",
                principalTable: "comment",
                principalColumns: new[] { "id", "comment_type_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_segment_comment_comment_comment_id_comment_type_id",
                schema: "chain_mates",
                table: "segment_comment",
                columns: new[] { "comment_id", "comment_type_id" },
                principalSchema: "chain_mates",
                principalTable: "comment",
                principalColumns: new[] { "id", "comment_type_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_segment_comment_segment_parent_segment_id",
                schema: "chain_mates",
                table: "segment_comment",
                column: "parent_segment_id",
                principalSchema: "chain_mates",
                principalTable: "segment",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_story_comment_comment_comment_id_comment_type_id",
                schema: "chain_mates",
                table: "story_comment",
                columns: new[] { "comment_id", "comment_type_id" },
                principalSchema: "chain_mates",
                principalTable: "comment",
                principalColumns: new[] { "id", "comment_type_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_story_comment_story_parent_story_id",
                schema: "chain_mates",
                table: "story_comment",
                column: "parent_story_id",
                principalSchema: "chain_mates",
                principalTable: "story",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comment_comment_comment_comment_comment_comment_id_comment_",
                schema: "chain_mates",
                table: "comment");

            migrationBuilder.DropForeignKey(
                name: "fk_comment_segment_comment_segment_comment_comment_id_segment_",
                schema: "chain_mates",
                table: "comment");

            migrationBuilder.DropForeignKey(
                name: "fk_comment_story_comment_story_comment_comment_id_story_commen",
                schema: "chain_mates",
                table: "comment");

            migrationBuilder.DropForeignKey(
                name: "fk_comment_comment_comment_comment_id_comment_type_id",
                schema: "chain_mates",
                table: "comment_comment");

            migrationBuilder.DropForeignKey(
                name: "fk_segment_comment_comment_comment_id_comment_type_id",
                schema: "chain_mates",
                table: "segment_comment");

            migrationBuilder.DropForeignKey(
                name: "fk_segment_comment_segment_parent_segment_id",
                schema: "chain_mates",
                table: "segment_comment");

            migrationBuilder.DropForeignKey(
                name: "fk_story_comment_comment_comment_id_comment_type_id",
                schema: "chain_mates",
                table: "story_comment");

            migrationBuilder.DropForeignKey(
                name: "fk_story_comment_story_parent_story_id",
                schema: "chain_mates",
                table: "story_comment");

            migrationBuilder.DropPrimaryKey(
                name: "pk_story_comment",
                schema: "chain_mates",
                table: "story_comment");

            migrationBuilder.DropPrimaryKey(
                name: "pk_segment_comment",
                schema: "chain_mates",
                table: "segment_comment");

            migrationBuilder.DropPrimaryKey(
                name: "pk_comment_comment",
                schema: "chain_mates",
                table: "comment_comment");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_comment_id_comment_type_id",
                schema: "chain_mates",
                table: "comment");

            migrationBuilder.DropIndex(
                name: "ix_comment_comment_comment_comment_id_comment_comment_comment_",
                schema: "chain_mates",
                table: "comment");

            migrationBuilder.DropIndex(
                name: "ix_comment_segment_comment_comment_id_segment_comment_comment_",
                schema: "chain_mates",
                table: "comment");

            migrationBuilder.DropIndex(
                name: "ix_comment_story_comment_comment_id_story_comment_comment_type",
                schema: "chain_mates",
                table: "comment");

            migrationBuilder.DropColumn(
                name: "comment_comment_comment_id",
                schema: "chain_mates",
                table: "comment");

            migrationBuilder.DropColumn(
                name: "comment_comment_comment_type_id",
                schema: "chain_mates",
                table: "comment");

            migrationBuilder.DropColumn(
                name: "segment_comment_comment_id",
                schema: "chain_mates",
                table: "comment");

            migrationBuilder.DropColumn(
                name: "segment_comment_comment_type_id",
                schema: "chain_mates",
                table: "comment");

            migrationBuilder.DropColumn(
                name: "story_comment_comment_id",
                schema: "chain_mates",
                table: "comment");

            migrationBuilder.DropColumn(
                name: "story_comment_comment_type_id",
                schema: "chain_mates",
                table: "comment");

            migrationBuilder.RenameColumn(
                name: "parent_story_id",
                schema: "chain_mates",
                table: "story_comment",
                newName: "story_id");

            migrationBuilder.RenameColumn(
                name: "comment_id",
                schema: "chain_mates",
                table: "story_comment",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "ix_story_comment_parent_story_id",
                schema: "chain_mates",
                table: "story_comment",
                newName: "ix_story_comment_story_id");

            migrationBuilder.RenameColumn(
                name: "parent_segment_id",
                schema: "chain_mates",
                table: "segment_comment",
                newName: "segment_id");

            migrationBuilder.RenameColumn(
                name: "comment_id",
                schema: "chain_mates",
                table: "segment_comment",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "ix_segment_comment_parent_segment_id",
                schema: "chain_mates",
                table: "segment_comment",
                newName: "ix_segment_comment_segment_id");

            migrationBuilder.RenameColumn(
                name: "parent_comment_id",
                schema: "chain_mates",
                table: "comment_comment",
                newName: "id");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                schema: "chain_mates",
                table: "story_comment",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                schema: "chain_mates",
                table: "segment_comment",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                schema: "chain_mates",
                table: "comment_comment",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "pk_story_comment",
                schema: "chain_mates",
                table: "story_comment",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_segment_comment",
                schema: "chain_mates",
                table: "segment_comment",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_comment_comment",
                schema: "chain_mates",
                table: "comment_comment",
                column: "id");

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

            migrationBuilder.AddForeignKey(
                name: "fk_segment_comment_segment_segment_id",
                schema: "chain_mates",
                table: "segment_comment",
                column: "segment_id",
                principalSchema: "chain_mates",
                principalTable: "segment",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_story_comment_story_story_id",
                schema: "chain_mates",
                table: "story_comment",
                column: "story_id",
                principalSchema: "chain_mates",
                principalTable: "story",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
