using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toeic.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class colVoca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "Mabaituvungs",
                newName: "ImageFolderPath");

            migrationBuilder.AddColumn<string>(
                name: "AudioFolderPath",
                table: "Mabaituvungs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExcelFilePath",
                table: "Mabaituvungs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioFolderPath",
                table: "Mabaituvungs");

            migrationBuilder.DropColumn(
                name: "ExcelFilePath",
                table: "Mabaituvungs");

            migrationBuilder.RenameColumn(
                name: "ImageFolderPath",
                table: "Mabaituvungs",
                newName: "FilePath");
        }
    }
}
