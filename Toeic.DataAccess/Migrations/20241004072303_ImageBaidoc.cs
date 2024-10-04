using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toeic.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ImageBaidoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "Mabaitapdocs",
                newName: "ImageBDFolderPath");

            migrationBuilder.AddColumn<string>(
                name: "ExcelFilePath",
                table: "Mabaitapdocs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Image_bai_doc",
                table: "Cauhoibaitapdocs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcelFilePath",
                table: "Mabaitapdocs");

            migrationBuilder.DropColumn(
                name: "Image_bai_doc",
                table: "Cauhoibaitapdocs");

            migrationBuilder.RenameColumn(
                name: "ImageBDFolderPath",
                table: "Mabaitapdocs",
                newName: "FilePath");
        }
    }
}
