using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _260120262221 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Films_ImageFile_FIlmImageId",
                table: "Films");

            migrationBuilder.RenameColumn(
                name: "FIlmUploadDate",
                table: "Films",
                newName: "FilmUploadDate");

            migrationBuilder.RenameColumn(
                name: "FIlmInLanguage1",
                table: "Films",
                newName: "FilmInLanguage1");

            migrationBuilder.RenameColumn(
                name: "FIlmImageId",
                table: "Films",
                newName: "FilmImageId");

            migrationBuilder.RenameColumn(
                name: "FIlmDirector1",
                table: "Films",
                newName: "FilmDirector1");

            migrationBuilder.RenameColumn(
                name: "MovieTotalParts",
                table: "Films",
                newName: "FilmTotalParts");

            migrationBuilder.RenameColumn(
                name: "MoviePart",
                table: "Films",
                newName: "FilmPart");

            migrationBuilder.RenameIndex(
                name: "IX_Films_FIlmImageId",
                table: "Films",
                newName: "IX_Films_FilmImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Films_ImageFile_FilmImageId",
                table: "Films",
                column: "FilmImageId",
                principalTable: "ImageFile",
                principalColumn: "ImageFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Films_ImageFile_FilmImageId",
                table: "Films");

            migrationBuilder.RenameColumn(
                name: "FilmUploadDate",
                table: "Films",
                newName: "FIlmUploadDate");

            migrationBuilder.RenameColumn(
                name: "FilmInLanguage1",
                table: "Films",
                newName: "FIlmInLanguage1");

            migrationBuilder.RenameColumn(
                name: "FilmImageId",
                table: "Films",
                newName: "FIlmImageId");

            migrationBuilder.RenameColumn(
                name: "FilmDirector1",
                table: "Films",
                newName: "FIlmDirector1");

            migrationBuilder.RenameColumn(
                name: "FilmTotalParts",
                table: "Films",
                newName: "MovieTotalParts");

            migrationBuilder.RenameColumn(
                name: "FilmPart",
                table: "Films",
                newName: "MoviePart");

            migrationBuilder.RenameIndex(
                name: "IX_Films_FilmImageId",
                table: "Films",
                newName: "IX_Films_FIlmImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Films_ImageFile_FIlmImageId",
                table: "Films",
                column: "FIlmImageId",
                principalTable: "ImageFile",
                principalColumn: "ImageFileId");
        }
    }
}
