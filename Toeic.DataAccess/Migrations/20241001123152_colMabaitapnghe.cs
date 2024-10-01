using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Toeic.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class colMabaitapnghe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cauhoibaitapdocs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cauhoibaitapnges",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Mabaitapdocs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Mabaitapdocs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Mabaitapnges",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Mabaitapnges",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Mabaitapnges",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Mabaitapdocs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Mabaitapnges",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "Mabaitapnges",
                newName: "ImageFolderPath");

            migrationBuilder.AddColumn<string>(
                name: "AudioFolderPath",
                table: "Mabaitapnges",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExcelFilePath",
                table: "Mabaitapnges",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioFolderPath",
                table: "Mabaitapnges");

            migrationBuilder.DropColumn(
                name: "ExcelFilePath",
                table: "Mabaitapnges");

            migrationBuilder.RenameColumn(
                name: "ImageFolderPath",
                table: "Mabaitapnges",
                newName: "FilePath");

            migrationBuilder.InsertData(
                table: "Mabaitapdocs",
                columns: new[] { "Id", "FilePath", "Part", "Tieu_de" },
                values: new object[,]
                {
                    { 1, "", 5, "Tiêu đề" },
                    { 2, "", 6, "Tiêu đề" },
                    { 3, "", 7, "Tiêu đề" }
                });

            migrationBuilder.InsertData(
                table: "Mabaitapnges",
                columns: new[] { "Id", "FilePath", "Part", "Tieu_de" },
                values: new object[,]
                {
                    { 1, null, 1, "Tiêu đề" },
                    { 2, null, 2, "Tiêu đề" },
                    { 3, null, 3, "Tiêu đề" },
                    { 4, null, 4, "Tiêu đề" }
                });

            migrationBuilder.InsertData(
                table: "Cauhoibaitapdocs",
                columns: new[] { "Id", "Bai_doc", "Cau_hoi", "Dap_an_1", "Dap_an_2", "Dap_an_3", "Dap_an_4", "Dap_an_dung", "Giai_thich", "Giai_thich_bai_doc", "IsCorrect", "Ma_bai_tap_docId", "Thu_tu_cau", "UserAnswer" },
                values: new object[] { 1, "", "H", "A", "B", "C", "D", "A", "A ĐÚNG", null, false, 1, 101, null });

            migrationBuilder.InsertData(
                table: "Cauhoibaitapnges",
                columns: new[] { "Id", "Audio", "Cau_hoi", "Dap_an_1", "Dap_an_2", "Dap_an_3", "Dap_an_4", "Dap_an_dung", "Giai_thich", "Image", "IsCorrect", "Ma_bai_tap_ngeId", "Thu_tu_cau", "Transcript", "UserAnswer" },
                values: new object[] { 1, "", "H", "A", "B", "C", "D", "A", "A ĐÚNG", "", false, 1, 1, null, null });
        }
    }
}
