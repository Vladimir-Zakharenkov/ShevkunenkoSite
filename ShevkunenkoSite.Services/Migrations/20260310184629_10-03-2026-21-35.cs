using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _100320262135 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageInfo_IconTypes_IconTypeModelId",
                table: "PageInfo");

            migrationBuilder.DropIndex(
                name: "IX_PageInfo_IconTypeModelId",
                table: "PageInfo");

            migrationBuilder.DropColumn(
                name: "IconTypeModelId",
                table: "PageInfo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IconTypeModelId",
                table: "PageInfo",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PageInfo_IconTypeModelId",
                table: "PageInfo",
                column: "IconTypeModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_PageInfo_IconTypes_IconTypeModelId",
                table: "PageInfo",
                column: "IconTypeModelId",
                principalTable: "IconTypes",
                principalColumn: "IconTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
