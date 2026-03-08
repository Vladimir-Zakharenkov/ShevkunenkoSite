using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _080320260900 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IconTypeModelId",
                table: "Icons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Icons_IconTypeModelId",
                table: "Icons",
                column: "IconTypeModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Icons_IconTypes_IconTypeModelId",
                table: "Icons",
                column: "IconTypeModelId",
                principalTable: "IconTypes",
                principalColumn: "IconTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Icons_IconTypes_IconTypeModelId",
                table: "Icons");

            migrationBuilder.DropIndex(
                name: "IX_Icons_IconTypeModelId",
                table: "Icons");

            migrationBuilder.DropColumn(
                name: "IconTypeModelId",
                table: "Icons");
        }
    }
}
