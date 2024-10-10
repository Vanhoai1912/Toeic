using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toeic.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class BthiUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Transcript",
                table: "Cauhoibaithis",
                newName: "Transcript_bai_nghe");

            migrationBuilder.RenameColumn(
                name: "Giai_thich",
                table: "Cauhoibaithis",
                newName: "Giai_thich_dap_an");

            migrationBuilder.AddColumn<string>(
                name: "Image_bai_doc",
                table: "Cauhoibaithis",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image_bai_doc",
                table: "Cauhoibaithis");

            migrationBuilder.RenameColumn(
                name: "Transcript_bai_nghe",
                table: "Cauhoibaithis",
                newName: "Transcript");

            migrationBuilder.RenameColumn(
                name: "Giai_thich_dap_an",
                table: "Cauhoibaithis",
                newName: "Giai_thich");
        }
    }
}
