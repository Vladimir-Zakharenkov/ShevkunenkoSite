using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _250120262300 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Films",
                columns: table => new
                {
                    FilmFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilmCaption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmCaptionOriginal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmDescriptionForSchemaOrg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmDescriptionHtml = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmNote = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmInMainList = table.Column<bool>(type: "bit", nullable: false),
                    FilmDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    FilmWidth = table.Column<int>(type: "int", nullable: false),
                    FilmHeight = table.Column<int>(type: "int", nullable: false),
                    FilmFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmFileExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmMimeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmFileSize = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    SearchFilterForFilm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmGenre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmIsFamilyFriendly = table.Column<bool>(type: "bit", nullable: false),
                    FilmAdult = table.Column<bool>(type: "bit", nullable: false),
                    FullFilmId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FilmDateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FilmDatePublished = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FIlmUploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FIlmInLanguage1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmInLanguage2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmSubtitles1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmSubtitles2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmРroductionCompany = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FIlmDirector1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    MovieTotalParts = table.Column<int>(type: "int", nullable: true),
                    MoviePart = table.Column<int>(type: "int", nullable: true),
                    SeriesSearchFilter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FIlmImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ImageFileModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FilmPosterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MoviePosterImageFileModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                        name: "FK_Films_ImageFile_ImageFileModelId",
                        column: x => x.ImageFileModelId,
                        principalTable: "ImageFile",
                        principalColumn: "ImageFileId");
                    table.ForeignKey(
                        name: "FK_Films_ImageFile_MoviePosterImageFileModelId",
                        column: x => x.MoviePosterImageFileModelId,
                        principalTable: "ImageFile",
                        principalColumn: "ImageFileId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Films_FullFilmId",
                table: "Films",
                column: "FullFilmId");

            migrationBuilder.CreateIndex(
                name: "IX_Films_ImageFileModelId",
                table: "Films",
                column: "ImageFileModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Films_MoviePosterImageFileModelId",
                table: "Films",
                column: "MoviePosterImageFileModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Films");
        }
    }
}
