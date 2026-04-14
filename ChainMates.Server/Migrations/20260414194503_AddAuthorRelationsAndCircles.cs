using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChainMates.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorRelationsAndCircles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "circle_id",
                schema: "chain_mates",
                table: "story",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "author_relation_type",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_author_relation_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "circle",
                schema: "chain_mates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_circle", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "author_relation",
                schema: "chain_mates",
                columns: table => new
                {
                    author_id = table.Column<int>(type: "integer", nullable: false),
                    related_author_id = table.Column<int>(type: "integer", nullable: false),
                    author_relation_type_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_author_relation", x => new { x.author_id, x.related_author_id });
                    table.ForeignKey(
                        name: "fk_author_relation_author_author_id",
                        column: x => x.author_id,
                        principalSchema: "chain_mates",
                        principalTable: "author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_author_relation_author_related_author_id",
                        column: x => x.related_author_id,
                        principalSchema: "chain_mates",
                        principalTable: "author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_author_relation_author_relation_type_author_relation_type_id",
                        column: x => x.author_relation_type_id,
                        principalSchema: "chain_mates",
                        principalTable: "author_relation_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "circle_assignment",
                schema: "chain_mates",
                columns: table => new
                {
                    circle_id = table.Column<int>(type: "integer", nullable: false),
                    author_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_circle_assignment", x => new { x.circle_id, x.author_id });
                    table.ForeignKey(
                        name: "fk_circle_assignment_author_circle_id",
                        column: x => x.circle_id,
                        principalSchema: "chain_mates",
                        principalTable: "author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_circle_assignment_circle_circle_id",
                        column: x => x.circle_id,
                        principalSchema: "chain_mates",
                        principalTable: "circle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_story_circle_id",
                schema: "chain_mates",
                table: "story",
                column: "circle_id");

            migrationBuilder.CreateIndex(
                name: "ix_author_relation_author_relation_type_id",
                schema: "chain_mates",
                table: "author_relation",
                column: "author_relation_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_author_relation_related_author_id",
                schema: "chain_mates",
                table: "author_relation",
                column: "related_author_id");

            migrationBuilder.AddForeignKey(
                name: "fk_story_circle_circle_id",
                schema: "chain_mates",
                table: "story",
                column: "circle_id",
                principalSchema: "chain_mates",
                principalTable: "circle",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_story_circle_circle_id",
                schema: "chain_mates",
                table: "story");

            migrationBuilder.DropTable(
                name: "author_relation",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "circle_assignment",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "author_relation_type",
                schema: "chain_mates");

            migrationBuilder.DropTable(
                name: "circle",
                schema: "chain_mates");

            migrationBuilder.DropIndex(
                name: "ix_story_circle_id",
                schema: "chain_mates",
                table: "story");

            migrationBuilder.DropColumn(
                name: "circle_id",
                schema: "chain_mates",
                table: "story");
        }
    }
}
