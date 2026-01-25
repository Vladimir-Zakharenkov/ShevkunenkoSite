using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _250120262307 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Films_ImageFile_ImageFileModelId",
                table: "Films");

            migrationBuilder.DropForeignKey(
                name: "FK_Films_ImageFile_MoviePosterImageFileModelId",
                table: "Films");

            migrationBuilder.DropIndex(
                name: "IX_Films_ImageFileModelId",
                table: "Films");

            migrationBuilder.DropIndex(
                name: "IX_Films_MoviePosterImageFileModelId",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "ImageFileModelId",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "MoviePosterImageFileModelId",
                table: "Films");

            migrationBuilder.CreateIndex(
                name: "IX_Films_FIlmImageId",
                table: "Films",
                column: "FIlmImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Films_FilmPosterId",
                table: "Films",
                column: "FilmPosterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Films_ImageFile_FIlmImageId",
                table: "Films",
                column: "FIlmImageId",
                principalTable: "ImageFile",
                principalColumn: "ImageFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Films_ImageFile_FilmPosterId",
                table: "Films",
                column: "FilmPosterId",
                principalTable: "ImageFile",
                principalColumn: "ImageFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Films_ImageFile_FIlmImageId",
                table: "Films");

            migrationBuilder.DropForeignKey(
                name: "FK_Films_ImageFile_FilmPosterId",
                table: "Films");

            migrationBuilder.DropIndex(
                name: "IX_Films_FIlmImageId",
                table: "Films");

            migrationBuilder.DropIndex(
                name: "IX_Films_FilmPosterId",
                table: "Films");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageFileModelId",
                table: "Films",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MoviePosterImageFileModelId",
                table: "Films",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Films_ImageFileModelId",
                table: "Films",
                column: "ImageFileModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Films_MoviePosterImageFileModelId",
                table: "Films",
                column: "MoviePosterImageFileModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Films_ImageFile_ImageFileModelId",
                table: "Films",
                column: "ImageFileModelId",
                principalTable: "ImageFile",
                principalColumn: "ImageFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Films_ImageFile_MoviePosterImageFileModelId",
                table: "Films",
                column: "MoviePosterImageFileModelId",
                principalTable: "ImageFile",
                principalColumn: "ImageFileId");
        }
    }
}
