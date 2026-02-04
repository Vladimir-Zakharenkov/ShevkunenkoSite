using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _040220261919 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FilmId",
                table: "PageInfo",
                type: "uniqueidentifier",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageInfo_Films_FilmId",
                table: "PageInfo");

            migrationBuilder.DropIndex(
                name: "IX_PageInfo_FilmId",
                table: "PageInfo");

            migrationBuilder.DropColumn(
                name: "FilmId",
                table: "PageInfo");
        }
    }
}
