using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _240920261906 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PageInfo_TextInfoId",
                table: "PageInfo");

            migrationBuilder.CreateIndex(
                name: "IX_PageInfo_TextInfoId",
                table: "PageInfo",
                column: "TextInfoId",
                unique: true,
                filter: "[TextInfoId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PageInfo_TextInfoId",
                table: "PageInfo");

            migrationBuilder.CreateIndex(
                name: "IX_PageInfo_TextInfoId",
                table: "PageInfo",
                column: "TextInfoId");
        }
    }
}
