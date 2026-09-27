using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _25020260753 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PageInfo_IconTypeModelId",
                table: "PageInfo");

            migrationBuilder.CreateIndex(
                name: "IX_PageInfo_IconTypeModelId",
                table: "PageInfo",
                column: "IconTypeModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PageInfo_IconTypeModelId",
                table: "PageInfo");

            migrationBuilder.CreateIndex(
                name: "IX_PageInfo_IconTypeModelId",
                table: "PageInfo",
                column: "IconTypeModelId",
                unique: true);
        }
    }
}
