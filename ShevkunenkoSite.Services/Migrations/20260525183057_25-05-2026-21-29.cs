using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _250520262129 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FilmForBookOrArticleId",
                table: "BooksAndArticles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BooksAndArticles_FilmForBookOrArticleId",
                table: "BooksAndArticles",
                column: "FilmForBookOrArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_BooksAndArticles_MovieFile_FilmForBookOrArticleId",
                table: "BooksAndArticles",
                column: "FilmForBookOrArticleId",
                principalTable: "MovieFile",
                principalColumn: "MovieFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BooksAndArticles_MovieFile_FilmForBookOrArticleId",
                table: "BooksAndArticles");

            migrationBuilder.DropIndex(
                name: "IX_BooksAndArticles_FilmForBookOrArticleId",
                table: "BooksAndArticles");

            migrationBuilder.DropColumn(
                name: "FilmForBookOrArticleId",
                table: "BooksAndArticles");
        }
    }
}
