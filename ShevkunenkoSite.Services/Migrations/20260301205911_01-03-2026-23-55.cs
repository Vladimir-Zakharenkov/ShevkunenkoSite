using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _010320262355 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IconFile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IconFile",
                columns: table => new
                {
                    IconFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IconFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconFileNameExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconFileSize = table.Column<int>(type: "int", nullable: false),
                    IconHeight = table.Column<int>(type: "int", nullable: false),
                    IconMimeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconRel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconWidth = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IconFile", x => x.IconFileId);
                });
        }
    }
}
