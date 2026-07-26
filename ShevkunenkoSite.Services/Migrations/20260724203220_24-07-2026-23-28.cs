using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _240720262328 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Films_ImageFile_FilmImageId",
                table: "Films");

            migrationBuilder.AlterColumn<Guid>(
                name: "FilmImageId",
                table: "Films",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Films_ImageFile_FilmImageId",
                table: "Films",
                column: "FilmImageId",
                principalTable: "ImageFile",
                principalColumn: "ImageFileId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Films_ImageFile_FilmImageId",
                table: "Films");

            migrationBuilder.AlterColumn<Guid>(
                name: "FilmImageId",
                table: "Films",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Films_ImageFile_FilmImageId",
                table: "Films",
                column: "FilmImageId",
                principalTable: "ImageFile",
                principalColumn: "ImageFileId");
        }
    }
}
