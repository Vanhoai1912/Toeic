using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ToeicWeb.Migrations
{
    /// <inheritdoc />
    public partial class SeedBtapdoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cau_hoi_bai_tap_doc",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cau_hoi_bai_tap_doc",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cau_hoi_bai_tap_doc",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "Tieu_de_bai_doc",
                table: "Bai_tap_doc");

            migrationBuilder.AlterColumn<string>(
                name: "Part",
                table: "Bai_tap_doc",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Bai_tap_doc",
                keyColumn: "Id",
                keyValue: 1,
                column: "Part",
                value: "ETS 2024 - TEST 1 - PART 5");

            migrationBuilder.UpdateData(
                table: "Bai_tap_doc",
                keyColumn: "Id",
                keyValue: 2,
                column: "Part",
                value: "ETS 2024 - TEST 1 - PART 6");

            migrationBuilder.UpdateData(
                table: "Bai_tap_doc",
                keyColumn: "Id",
                keyValue: 3,
                column: "Part",
                value: "ETS 2024 - TEST 1 - PART 7");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Part",
                table: "Bai_tap_doc",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Tieu_de_bai_doc",
                table: "Bai_tap_doc",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Bai_tap_doc",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Part", "Tieu_de_bai_doc" },
                values: new object[] { 5, "" });

            migrationBuilder.UpdateData(
                table: "Bai_tap_doc",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Part", "Tieu_de_bai_doc" },
                values: new object[] { 6, "" });

            migrationBuilder.UpdateData(
                table: "Bai_tap_doc",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Part", "Tieu_de_bai_doc" },
                values: new object[] { 7, "" });

            migrationBuilder.InsertData(
                table: "Cau_hoi_bai_tap_doc",
                columns: new[] { "Id", "Bai_tap_docId", "Cau_hoi", "Dap_an_1", "Dap_an_2", "Dap_an_3", "Dap_an_4", "Dap_an_dung", "Giai_thich", "Photo_name", "So_thu_tu" },
                values: new object[,]
                {
                    { 1, 1, "Former Sendai Company CEO Ken Nakata spoke about -- career experiences.", "A. he", "B. his", "C. him", "D. himself", "B. his", "Cựu Giám đốc điều hành Công ty Sendai Ken Nakata đã nói về kinh nghiệm nghề nghiệp ------- .\r\n\r\n(A) anh ấy\r\n\r\n(B) của anh ấy\r\n\r\n(C) anh ấy\r\n\r\n(D) bản thân anh ấy\r\n\r\n Chỗ trống cần điền đứng sau giới từ và trước một cụm danh từ (career experiences) => cần điền một từ hạn định đứng trước cụm danh từ đó để bổ nghĩa. Trong 4 đáp án, chỉ có his là tính từ sở hữu mới có thể đóng vai trò là từ hạn định đứng trước danh từ.  \r\n\r\n=> Đáp án (B) ", "", "101" },
                    { 2, 1, "Former Sendai Company CEO Ken Nakata spoke about -- career experiences.", "A. he", "B. his", "C. him", "D. himself", "B. his", "Cựu Giám đốc điều hành Công ty Sendai Ken Nakata đã nói về kinh nghiệm nghề nghiệp ------- .\r\n\r\n(A) anh ấy\r\n\r\n(B) của anh ấy\r\n\r\n(C) anh ấy\r\n\r\n(D) bản thân anh ấy\r\n\r\n Chỗ trống cần điền đứng sau giới từ và trước một cụm danh từ (career experiences) => cần điền một từ hạn định đứng trước cụm danh từ đó để bổ nghĩa. Trong 4 đáp án, chỉ có his là tính từ sở hữu mới có thể đóng vai trò là từ hạn định đứng trước danh từ.  \r\n\r\n=> Đáp án (B) ", "", "102" },
                    { 3, 1, "Former Sendai Company CEO Ken Nakata spoke about -- career experiences.", "A. he", "B. his", "C. him", "D. himself", "B. his", "Cựu Giám đốc điều hành Công ty Sendai Ken Nakata đã nói về kinh nghiệm nghề nghiệp ------- .\r\n\r\n(A) anh ấy\r\n\r\n(B) của anh ấy\r\n\r\n(C) anh ấy\r\n\r\n(D) bản thân anh ấy\r\n\r\n Chỗ trống cần điền đứng sau giới từ và trước một cụm danh từ (career experiences) => cần điền một từ hạn định đứng trước cụm danh từ đó để bổ nghĩa. Trong 4 đáp án, chỉ có his là tính từ sở hữu mới có thể đóng vai trò là từ hạn định đứng trước danh từ.  \r\n\r\n=> Đáp án (B) ", "", "103" }
                });
        }
    }
}
