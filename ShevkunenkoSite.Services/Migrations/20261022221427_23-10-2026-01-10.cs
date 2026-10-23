using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevkunenkoSite.Services.Migrations
{
    /// <inheritdoc />
    public partial class _231020260110 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "WebImageWidth",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "WebImageMimeType",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<long>(
                name: "WebImageHeight",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "WebImageHDWidth",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "WebImageHDHeight",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "WebImageHDFileSize",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "WebImageFileSize",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "WebImageFileNameExtension",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<long>(
                name: "WebIconWidth",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "WebIconMimeType",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<long>(
                name: "WebIconHeight",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "WebIconFileSize",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "WebIconFileNameExtension",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<long>(
                name: "WebIcon200Width",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "WebIcon200MimeType",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<long>(
                name: "WebIcon200Height",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "WebIcon200FileSize",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "WebIcon200FileNameExtension",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<long>(
                name: "WebIcon100Width",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "WebIcon100MimeType",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<long>(
                name: "WebIcon100Height",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "WebIcon100FileSize",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "WebIcon100FileNameExtension",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<long>(
                name: "ImageWidth",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ImageHeight",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ImageFileSize",
                table: "ImageFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "WebIcon100FileNameAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebIcon100FileNameExtensionAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WebIcon100FileSizeAI",
                table: "ImageFile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WebIcon100HeightAI",
                table: "ImageFile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebIcon100MimeTypeAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WebIcon100WidthAI",
                table: "ImageFile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebIcon200FileNameAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebIcon200FileNameExtensionAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WebIcon200FileSizeAI",
                table: "ImageFile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WebIcon200HeightAI",
                table: "ImageFile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebIcon200MimeTypeAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WebIcon200WidthAI",
                table: "ImageFile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebIconFileNameAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebIconFileNameExtensionAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WebIconFileSizeAI",
                table: "ImageFile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WebIconHeightAI",
                table: "ImageFile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebIconMimeTypeAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WebIconWidthAI",
                table: "ImageFile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebImageFileNameAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebImageFileNameExtensionAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WebImageFileSizeAI",
                table: "ImageFile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebImageHDFileNameAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WebImageHDFileSizeAI",
                table: "ImageFile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WebImageHDHeightAI",
                table: "ImageFile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebImageHDMimeTypeAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebImageHDNameExtensionAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WebImageHDWidthAI",
                table: "ImageFile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WebImageHeightAI",
                table: "ImageFile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebImageMimeTypeAI",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WebImageWidthAI",
                table: "ImageFile",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebIcon100FileNameAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebIcon100FileNameExtensionAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebIcon100FileSizeAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebIcon100HeightAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebIcon100MimeTypeAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebIcon100WidthAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebIcon200FileNameAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebIcon200FileNameExtensionAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebIcon200FileSizeAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebIcon200HeightAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebIcon200MimeTypeAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebIcon200WidthAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebIconFileNameAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebIconFileNameExtensionAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebIconFileSizeAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebIconHeightAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebIconMimeTypeAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebIconWidthAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebImageFileNameAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebImageFileNameExtensionAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebImageFileSizeAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebImageHDFileNameAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebImageHDFileSizeAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebImageHDHeightAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebImageHDMimeTypeAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebImageHDNameExtensionAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebImageHDWidthAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebImageHeightAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebImageMimeTypeAI",
                table: "ImageFile");

            migrationBuilder.DropColumn(
                name: "WebImageWidthAI",
                table: "ImageFile");

            migrationBuilder.AlterColumn<long>(
                name: "WebImageWidth",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebImageMimeType",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "WebImageHeight",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "WebImageHDWidth",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "WebImageHDHeight",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "WebImageHDFileSize",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "WebImageFileSize",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebImageFileNameExtension",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "WebIconWidth",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebIconMimeType",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "WebIconHeight",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "WebIconFileSize",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebIconFileNameExtension",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "WebIcon200Width",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebIcon200MimeType",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "WebIcon200Height",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "WebIcon200FileSize",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebIcon200FileNameExtension",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "WebIcon100Width",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebIcon100MimeType",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "WebIcon100Height",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "WebIcon100FileSize",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebIcon100FileNameExtension",
                table: "ImageFile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ImageWidth",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ImageHeight",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ImageFileSize",
                table: "ImageFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
