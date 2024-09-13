using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ToeicWeb.Migrations
{
    /// <inheritdoc />
    public partial class Baitapnge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mabaitapnges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Part = table.Column<int>(type: "int", nullable: false),
                    Tieu_de = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mabaitapnges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cauhoibaitapnges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Thu_tu_cau = table.Column<int>(type: "int", nullable: false),
                    Audio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cau_hoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dap_an_1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dap_an_2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dap_an_3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dap_an_4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dap_an_dung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Giai_thich = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ma_bai_tap_ngeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cauhoibaitapnges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cauhoibaitapnges_Mabaitapnges_Ma_bai_tap_ngeId",
                        column: x => x.Ma_bai_tap_ngeId,
                        principalTable: "Mabaitapnges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Mabaitapnges",
                columns: new[] { "Id", "Part", "Tieu_de" },
                values: new object[,]
                {
                    { 1, 1, "Tiêu đề" },
                    { 2, 2, "Tiêu đề" },
                    { 3, 3, "Tiêu đề" },
                    { 4, 4, "Tiêu đề" }
                });

            migrationBuilder.InsertData(
                table: "Cauhoibaitapnges",
                columns: new[] { "Id", "Audio", "Cau_hoi", "Dap_an_1", "Dap_an_2", "Dap_an_3", "Dap_an_4", "Dap_an_dung", "Giai_thich", "Image", "Ma_bai_tap_ngeId", "Thu_tu_cau" },
                values: new object[] { 1, "", "H", "A", "B", "C", "D", "A", "A ĐÚNG", "", 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Cauhoibaitapnges_Ma_bai_tap_ngeId",
                table: "Cauhoibaitapnges",
                column: "Ma_bai_tap_ngeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cauhoibaitapnges");

            migrationBuilder.DropTable(
                name: "Mabaitapnges");
        }
    }
}
