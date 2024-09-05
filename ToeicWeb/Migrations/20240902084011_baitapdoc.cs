using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ToeicWeb.Migrations
{
    /// <inheritdoc />
    public partial class baitapdoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bai_tap_doc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Part = table.Column<int>(type: "int", nullable: false),
                    Tieu_de_bai_doc = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bai_tap_doc", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cau_hoi_bai_tap_doc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cau_hoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dap_an_dung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dap_an_1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dap_an_2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dap_an_3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dap_an_4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Giai_thich = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Photo_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    So_thu_tu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bai_tap_docId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cau_hoi_bai_tap_doc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cau_hoi_bai_tap_doc_Bai_tap_doc_Bai_tap_docId",
                        column: x => x.Bai_tap_docId,
                        principalTable: "Bai_tap_doc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Bai_tap_doc",
                columns: new[] { "Id", "Part", "Tieu_de_bai_doc" },
                values: new object[,]
                {
                    { 1, 5, "" },
                    { 2, 6, "" },
                    { 3, 7, "" }
                });

            migrationBuilder.InsertData(
                table: "Cau_hoi_bai_tap_doc",
                columns: new[] { "Id", "Bai_tap_docId", "Cau_hoi", "Dap_an_1", "Dap_an_2", "Dap_an_3", "Dap_an_4", "Dap_an_dung", "Giai_thich", "Photo_name", "So_thu_tu" },
                values: new object[,]
                {
                    { 1, 1, "Former Sendai Company CEO Ken Nakata spoke about -- career experiences.", "A. he", "B. his", "C. him", "D. himself", "B. his", "Cựu Giám đốc điều hành Công ty Sendai Ken Nakata đã nói về kinh nghiệm nghề nghiệp ------- .\r\n\r\n(A) anh ấy\r\n\r\n(B) của anh ấy\r\n\r\n(C) anh ấy\r\n\r\n(D) bản thân anh ấy\r\n\r\n Chỗ trống cần điền đứng sau giới từ và trước một cụm danh từ (career experiences) => cần điền một từ hạn định đứng trước cụm danh từ đó để bổ nghĩa. Trong 4 đáp án, chỉ có his là tính từ sở hữu mới có thể đóng vai trò là từ hạn định đứng trước danh từ.  \r\n\r\n=> Đáp án (B) ", "", "101" },
                    { 2, 1, "Former Sendai Company CEO Ken Nakata spoke about -- career experiences.", "A. he", "B. his", "C. him", "D. himself", "B. his", "Cựu Giám đốc điều hành Công ty Sendai Ken Nakata đã nói về kinh nghiệm nghề nghiệp ------- .\r\n\r\n(A) anh ấy\r\n\r\n(B) của anh ấy\r\n\r\n(C) anh ấy\r\n\r\n(D) bản thân anh ấy\r\n\r\n Chỗ trống cần điền đứng sau giới từ và trước một cụm danh từ (career experiences) => cần điền một từ hạn định đứng trước cụm danh từ đó để bổ nghĩa. Trong 4 đáp án, chỉ có his là tính từ sở hữu mới có thể đóng vai trò là từ hạn định đứng trước danh từ.  \r\n\r\n=> Đáp án (B) ", "", "102" },
                    { 3, 1, "Former Sendai Company CEO Ken Nakata spoke about -- career experiences.", "A. he", "B. his", "C. him", "D. himself", "B. his", "Cựu Giám đốc điều hành Công ty Sendai Ken Nakata đã nói về kinh nghiệm nghề nghiệp ------- .\r\n\r\n(A) anh ấy\r\n\r\n(B) của anh ấy\r\n\r\n(C) anh ấy\r\n\r\n(D) bản thân anh ấy\r\n\r\n Chỗ trống cần điền đứng sau giới từ và trước một cụm danh từ (career experiences) => cần điền một từ hạn định đứng trước cụm danh từ đó để bổ nghĩa. Trong 4 đáp án, chỉ có his là tính từ sở hữu mới có thể đóng vai trò là từ hạn định đứng trước danh từ.  \r\n\r\n=> Đáp án (B) ", "", "103" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cau_hoi_bai_tap_doc_Bai_tap_docId",
                table: "Cau_hoi_bai_tap_doc",
                column: "Bai_tap_docId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cau_hoi_bai_tap_doc");

            migrationBuilder.DropTable(
                name: "Bai_tap_doc");
        }
    }
}
