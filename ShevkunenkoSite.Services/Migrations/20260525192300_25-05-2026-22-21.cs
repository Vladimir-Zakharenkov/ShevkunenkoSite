using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _250520262221 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BooksAndArticles_MovieFile_FilmForBookOrArticleId",
                table: "BooksAndArticles");

            migrationBuilder.AddForeignKey(
                name: "FK_BooksAndArticles_Films_FilmForBookOrArticleId",
                table: "BooksAndArticles",
                column: "FilmForBookOrArticleId",
                principalTable: "Films",
                principalColumn: "FilmFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BooksAndArticles_Films_FilmForBookOrArticleId",
                table: "BooksAndArticles");

            migrationBuilder.AddForeignKey(
                name: "FK_BooksAndArticles_MovieFile_FilmForBookOrArticleId",
                table: "BooksAndArticles",
                column: "FilmForBookOrArticleId",
                principalTable: "MovieFile",
                principalColumn: "MovieFileId");
        }
    }
}
