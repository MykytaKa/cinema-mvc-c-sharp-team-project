using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrationUpd3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Halls_FilmId",
                table: "Sessions");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_HallId",
                table: "Sessions",
                column: "HallId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Halls_HallId",
                table: "Sessions",
                column: "HallId",
                principalTable: "Halls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Halls_HallId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_HallId",
                table: "Sessions");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Halls_FilmId",
                table: "Sessions",
                column: "FilmId",
                principalTable: "Halls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
