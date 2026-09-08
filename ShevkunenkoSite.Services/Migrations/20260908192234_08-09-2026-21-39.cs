using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _080920262139 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookCaptionForURL_BooksAndArticles_BooksAndArticlesModelId",
                table: "BookCaptionForURL");

            migrationBuilder.DropForeignKey(
                name: "FK_Films_ImageFile_FilmImageId",
                table: "Films");

            migrationBuilder.DropForeignKey(
                name: "FK_Films_ImageFile_FilmPosterId",
                table: "Films");

            migrationBuilder.DropForeignKey(
                name: "FK_Icons_IconTypes_IconTypeModelId",
                table: "Icons");

            migrationBuilder.DropForeignKey(
                name: "FK_PageInfo_BackgroundFile_BackgroundFileModelId",
                table: "PageInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_PageInfo_IconTypes_IconTypeModelId",
                table: "PageInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_PageInfo_ImageFile_ImageFileModelId",
                table: "PageInfo");

            migrationBuilder.AddForeignKey(
                name: "FK_BookCaptionForURL_BooksAndArticles_BooksAndArticlesModelId",
                table: "BookCaptionForURL",
                column: "BooksAndArticlesModelId",
                principalTable: "BooksAndArticles",
                principalColumn: "BooksArticlesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Films_ImageFile_FilmImageId",
                table: "Films",
                column: "FilmImageId",
                principalTable: "ImageFile",
                principalColumn: "ImageFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Films_ImageFile_FilmPosterId",
                table: "Films",
                column: "FilmPosterId",
                principalTable: "ImageFile",
                principalColumn: "ImageFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Icons_IconTypes_IconTypeModelId",
                table: "Icons",
                column: "IconTypeModelId",
                principalTable: "IconTypes",
                principalColumn: "IconTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PageInfo_BackgroundFile_BackgroundFileModelId",
                table: "PageInfo",
                column: "BackgroundFileModelId",
                principalTable: "BackgroundFile",
                principalColumn: "BackgroundFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_PageInfo_IconTypes_IconTypeModelId",
                table: "PageInfo",
                column: "IconTypeModelId",
                principalTable: "IconTypes",
                principalColumn: "IconTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PageInfo_ImageFile_ImageFileModelId",
                table: "PageInfo",
                column: "ImageFileModelId",
                principalTable: "ImageFile",
                principalColumn: "ImageFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookCaptionForURL_BooksAndArticles_BooksAndArticlesModelId",
                table: "BookCaptionForURL");

            migrationBuilder.DropForeignKey(
                name: "FK_Films_ImageFile_FilmImageId",
                table: "Films");

            migrationBuilder.DropForeignKey(
                name: "FK_Films_ImageFile_FilmPosterId",
                table: "Films");

            migrationBuilder.DropForeignKey(
                name: "FK_Icons_IconTypes_IconTypeModelId",
                table: "Icons");

            migrationBuilder.DropForeignKey(
                name: "FK_PageInfo_BackgroundFile_BackgroundFileModelId",
                table: "PageInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_PageInfo_IconTypes_IconTypeModelId",
                table: "PageInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_PageInfo_ImageFile_ImageFileModelId",
                table: "PageInfo");

            migrationBuilder.AddForeignKey(
                name: "FK_BookCaptionForURL_BooksAndArticles_BooksAndArticlesModelId",
                table: "BookCaptionForURL",
                column: "BooksAndArticlesModelId",
                principalTable: "BooksAndArticles",
                principalColumn: "BooksArticlesId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Films_ImageFile_FilmImageId",
                table: "Films",
                column: "FilmImageId",
                principalTable: "ImageFile",
                principalColumn: "ImageFileId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Films_ImageFile_FilmPosterId",
                table: "Films",
                column: "FilmPosterId",
                principalTable: "ImageFile",
                principalColumn: "ImageFileId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Icons_IconTypes_IconTypeModelId",
                table: "Icons",
                column: "IconTypeModelId",
                principalTable: "IconTypes",
                principalColumn: "IconTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PageInfo_BackgroundFile_BackgroundFileModelId",
                table: "PageInfo",
                column: "BackgroundFileModelId",
                principalTable: "BackgroundFile",
                principalColumn: "BackgroundFileId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PageInfo_IconTypes_IconTypeModelId",
                table: "PageInfo",
                column: "IconTypeModelId",
                principalTable: "IconTypes",
                principalColumn: "IconTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PageInfo_ImageFile_ImageFileModelId",
                table: "PageInfo",
                column: "ImageFileModelId",
                principalTable: "ImageFile",
                principalColumn: "ImageFileId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
