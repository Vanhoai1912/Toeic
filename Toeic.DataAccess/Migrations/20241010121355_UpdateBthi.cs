using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toeic.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBthi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Giai_thich_bai_doc",
                table: "Cauhoibaithis");

            migrationBuilder.DropColumn(
                name: "Image_bai_doc",
                table: "Cauhoibaithis");

            migrationBuilder.RenameColumn(
                name: "Transcript_bai_nghe",
                table: "Cauhoibaithis",
                newName: "Transcript");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Transcript",
                table: "Cauhoibaithis",
                newName: "Transcript_bai_nghe");

            migrationBuilder.AddColumn<string>(
                name: "Giai_thich_bai_doc",
                table: "Cauhoibaithis",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image_bai_doc",
                table: "Cauhoibaithis",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
