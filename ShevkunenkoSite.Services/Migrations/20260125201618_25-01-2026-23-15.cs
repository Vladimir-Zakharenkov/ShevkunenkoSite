using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _250120262315 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FilmInfoId",
                table: "PageInfo",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageInfo_FilmInfoId",
                table: "PageInfo",
                column: "FilmInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PageInfo_Films_FilmInfoId",
                table: "PageInfo",
                column: "FilmInfoId",
                principalTable: "Films",
                principalColumn: "FilmFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageInfo_Films_FilmInfoId",
                table: "PageInfo");

            migrationBuilder.DropIndex(
                name: "IX_PageInfo_FilmInfoId",
                table: "PageInfo");

            migrationBuilder.DropColumn(
                name: "FilmInfoId",
                table: "PageInfo");
        }
    }
}
