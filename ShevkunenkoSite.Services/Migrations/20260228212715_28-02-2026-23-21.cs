using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _280220262321 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FilmId",
                table: "PageInfo",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Films",
                columns: table => new
                {
                    FilmFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilmDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    FilmWidth = table.Column<int>(type: "int", nullable: false),
                    FilmHeight = table.Column<int>(type: "int", nullable: false),
                    FilmFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmFileExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmMimeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmFileSize = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    FullFilmId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FilmCaption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmCaptionOriginal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmDescriptionForSchemaOrg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmDescriptionHtml = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmInMainList = table.Column<bool>(type: "bit", nullable: false),
                    SearchFilterForFilm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmGenre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmIsFamilyFriendly = table.Column<bool>(type: "bit", nullable: false),
                    FilmAdult = table.Column<bool>(type: "bit", nullable: false),
                    FilmDateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FilmDatePublished = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FilmUploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FilmInLanguage1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmInLanguage2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmSubtitles1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmSubtitles2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmРroductionCompany = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmDirector1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmDirector2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmMusicBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmActor01 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmActor02 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmActor03 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmActor04 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmActor05 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmActor06 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmActor07 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmActor08 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmActor09 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmActor10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmContentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmYouTube = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmVkVideo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmMailRuVideo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmOkVideo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmYandexDiskVideo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmKinoTeatrRu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmKinoPoisk = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmImbd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmTotalParts = table.Column<int>(type: "int", nullable: true),
                    FilmPart = table.Column<int>(type: "int", nullable: true),
                    SeriesSearchFilter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FilmPosterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Films", x => x.FilmFileId);
                    table.ForeignKey(
                        name: "FK_Films_Films_FullFilmId",
                        column: x => x.FullFilmId,
                        principalTable: "Films",
                        principalColumn: "FilmFileId");
                    table.ForeignKey(
                        name: "FK_Films_ImageFile_FilmImageId",
                        column: x => x.FilmImageId,
                        principalTable: "ImageFile",
                        principalColumn: "ImageFileId");
                    table.ForeignKey(
                        name: "FK_Films_ImageFile_FilmPosterId",
                        column: x => x.FilmPosterId,
                        principalTable: "ImageFile",
                        principalColumn: "ImageFileId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageInfo_FilmId",
                table: "PageInfo",
                column: "FilmId",
                unique: true,
                filter: "[FilmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Films_FilmImageId",
                table: "Films",
                column: "FilmImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Films_FilmPosterId",
                table: "Films",
                column: "FilmPosterId");

            migrationBuilder.CreateIndex(
                name: "IX_Films_FullFilmId",
                table: "Films",
                column: "FullFilmId");

            migrationBuilder.AddForeignKey(
                name: "FK_PageInfo_Films_FilmId",
                table: "PageInfo",
                column: "FilmId",
                principalTable: "Films",
                principalColumn: "FilmFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageInfo_Films_FilmId",
                table: "PageInfo");

            migrationBuilder.DropTable(
                name: "Films");

            migrationBuilder.DropIndex(
                name: "IX_PageInfo_FilmId",
                table: "PageInfo");

            migrationBuilder.DropColumn(
                name: "FilmId",
                table: "PageInfo");
        }
    }
}
