using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSimilarityMatrixToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FilmSimilarities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Film1Id = table.Column<int>(type: "int", nullable: false),
                    Film2Id = table.Column<int>(type: "int", nullable: false),
                    SimilarityScore = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmSimilarities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilmSimilarities_Films_Film1Id",
                        column: x => x.Film1Id,
                        principalTable: "Films",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FilmSimilarities_Films_Film2Id",
                        column: x => x.Film2Id,
                        principalTable: "Films",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilmSimilarities_Film1Id",
                table: "FilmSimilarities",
                column: "Film1Id");

            migrationBuilder.CreateIndex(
                name: "IX_FilmSimilarities_Film2Id",
                table: "FilmSimilarities",
                column: "Film2Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilmSimilarities");
        }
    }
}
