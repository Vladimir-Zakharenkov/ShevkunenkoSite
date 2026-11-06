using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _061120261103 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WebImageHDFileNameAI",
                table: "ImageFile",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebImageHDFileName",
                table: "ImageFile",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebImageFileNameAI",
                table: "ImageFile",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebImageFileName",
                table: "ImageFile",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "WebIconFileNameAI",
                table: "ImageFile",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebIconFileName",
                table: "ImageFile",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "WebIcon200FileNameAI",
                table: "ImageFile",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebIcon200FileName",
                table: "ImageFile",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "WebIcon100FileNameAI",
                table: "ImageFile",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebIcon100FileName",
                table: "ImageFile",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ImageHDFileName",
                table: "ImageFile",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageFileName",
                table: "ImageFile",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IconFileName",
                table: "ImageFile",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Icon200FileName",
                table: "ImageFile",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Icon100FileName",
                table: "ImageFile",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageFile_Icon100FileName",
                table: "ImageFile",
                column: "Icon100FileName",
                unique: true,
                filter: "[Icon100FileName] IS NOT NULL AND [Icon100FileName] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_ImageFile_Icon200FileName",
                table: "ImageFile",
                column: "Icon200FileName",
                unique: true,
                filter: "[Icon200FileName] IS NOT NULL AND [Icon200FileName] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_ImageFile_IconFileName",
                table: "ImageFile",
                column: "IconFileName",
                unique: true,
                filter: "[IconFileName] IS NOT NULL AND [IconFileName] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_ImageFile_ImageFileName",
                table: "ImageFile",
                column: "ImageFileName",
                unique: true,
                filter: "[ImageFileName] IS NOT NULL AND [ImageFileName] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_ImageFile_ImageHDFileName",
                table: "ImageFile",
                column: "ImageHDFileName",
                unique: true,
                filter: "[ImageHDFileName] IS NOT NULL AND [ImageHDFileName] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_ImageFile_WebIcon100FileName",
                table: "ImageFile",
                column: "WebIcon100FileName",
                unique: true,
                filter: "[WebIcon100FileName] IS NOT NULL AND [WebIcon100FileName] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_ImageFile_WebIcon100FileNameAI",
                table: "ImageFile",
                column: "WebIcon100FileNameAI",
                unique: true,
                filter: "[WebIcon100FileNameAI] IS NOT NULL AND [WebIcon100FileNameAI] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_ImageFile_WebIcon200FileName",
                table: "ImageFile",
                column: "WebIcon200FileName",
                unique: true,
                filter: "[WebIcon200FileName] IS NOT NULL AND [WebIcon200FileName] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_ImageFile_WebIcon200FileNameAI",
                table: "ImageFile",
                column: "WebIcon200FileNameAI",
                unique: true,
                filter: "[WebIcon200FileNameAI] IS NOT NULL AND [WebIcon200FileNameAI] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_ImageFile_WebIconFileName",
                table: "ImageFile",
                column: "WebIconFileName",
                unique: true,
                filter: "[WebIconFileName] IS NOT NULL AND [WebIconFileName] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_ImageFile_WebIconFileNameAI",
                table: "ImageFile",
                column: "WebIconFileNameAI",
                unique: true,
                filter: "[WebIconFileNameAI] IS NOT NULL AND [WebIconFileNameAI] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_ImageFile_WebImageFileName",
                table: "ImageFile",
                column: "WebImageFileName",
                unique: true,
                filter: "[WebImageFileName] IS NOT NULL AND [WebImageFileName] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_ImageFile_WebImageFileNameAI",
                table: "ImageFile",
                column: "WebImageFileNameAI",
                unique: true,
                filter: "[WebImageFileNameAI] IS NOT NULL AND [WebImageFileNameAI] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_ImageFile_WebImageHDFileName",
                table: "ImageFile",
                column: "WebImageHDFileName",
                unique: true,
                filter: "[WebImageHDFileName] IS NOT NULL AND [WebImageHDFileName] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_ImageFile_WebImageHDFileNameAI",
                table: "ImageFile",
                column: "WebImageHDFileNameAI",
                unique: true,
                filter: "[WebImageHDFileNameAI] IS NOT NULL AND [WebImageHDFileNameAI] <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImageFile_Icon100FileName",
                table: "ImageFile");

            migrationBuilder.DropIndex(
                name: "IX_ImageFile_Icon200FileName",
                table: "ImageFile");

            migrationBuilder.DropIndex(
                name: "IX_ImageFile_IconFileName",
                table: "ImageFile");

            migrationBuilder.DropIndex(
                name: "IX_ImageFile_ImageFileName",
                table: "ImageFile");

            migrationBuilder.DropIndex(
                name: "IX_ImageFile_ImageHDFileName",
                table: "ImageFile");

            migrationBuilder.DropIndex(
                name: "IX_ImageFile_WebIcon100FileName",
                table: "ImageFile");

            migrationBuilder.DropIndex(
                name: "IX_ImageFile_WebIcon100FileNameAI",
                table: "ImageFile");

            migrationBuilder.DropIndex(
                name: "IX_ImageFile_WebIcon200FileName",
                table: "ImageFile");

            migrationBuilder.DropIndex(
                name: "IX_ImageFile_WebIcon200FileNameAI",
                table: "ImageFile");

            migrationBuilder.DropIndex(
                name: "IX_ImageFile_WebIconFileName",
                table: "ImageFile");

            migrationBuilder.DropIndex(
                name: "IX_ImageFile_WebIconFileNameAI",
                table: "ImageFile");

            migrationBuilder.DropIndex(
                name: "IX_ImageFile_WebImageFileName",
                table: "ImageFile");

            migrationBuilder.DropIndex(
                name: "IX_ImageFile_WebImageFileNameAI",
                table: "ImageFile");

            migrationBuilder.DropIndex(
                name: "IX_ImageFile_WebImageHDFileName",
                table: "ImageFile");

            migrationBuilder.DropIndex(
                name: "IX_ImageFile_WebImageHDFileNameAI",
                table: "ImageFile");

            migrationBuilder.AlterColumn<string>(
                name: "WebImageHDFileNameAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebImageHDFileName",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebImageFileNameAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebImageFileName",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "WebIconFileNameAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebIconFileName",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "WebIcon200FileNameAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebIcon200FileName",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "WebIcon100FileNameAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebIcon100FileName",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ImageHDFileName",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageFileName",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IconFileName",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Icon200FileName",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Icon100FileName",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
