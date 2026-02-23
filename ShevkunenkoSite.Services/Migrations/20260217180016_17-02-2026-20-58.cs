using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _170220262058 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Icons",
                columns: table => new
                {
                    IconId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IconFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PathToIcon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconMimeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelForIcon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconPurpose = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Icons", x => x.IconId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Icons");
        }
    }
}
