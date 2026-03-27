using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _260320262132 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageInfo_Films_FilmId",
                table: "PageInfo");

            migrationBuilder.DropIndex(
                name: "IX_PageInfo_FilmId",
                table: "PageInfo");

            migrationBuilder.RenameColumn(
                name: "FilmId",
                table: "PageInfo",
                newName: "FilmFileModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PageInfo_FilmFileModelId",
                table: "PageInfo",
                column: "FilmFileModelId",
                unique: true,
                filter: "[FilmFileModelId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PageInfo_Films_FilmFileModelId",
                table: "PageInfo",
                column: "FilmFileModelId",
                principalTable: "Films",
                principalColumn: "FilmFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageInfo_Films_FilmFileModelId",
                table: "PageInfo");

            migrationBuilder.DropIndex(
                name: "IX_PageInfo_FilmFileModelId",
                table: "PageInfo");

            migrationBuilder.RenameColumn(
                name: "FilmFileModelId",
                table: "PageInfo",
                newName: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_PageInfo_FilmId",
                table: "PageInfo",
                column: "FilmId",
                unique: true,
                filter: "[FilmId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PageInfo_Films_FilmId",
                table: "PageInfo",
                column: "FilmId",
                principalTable: "Films",
                principalColumn: "FilmFileId");
        }
    }
}
