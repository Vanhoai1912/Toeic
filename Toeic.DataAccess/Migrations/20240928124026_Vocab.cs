using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toeic.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Vocab : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mabaituvungs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten_bai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mabaituvungs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Noidungbaituvungs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    So_thu_tu = table.Column<int>(type: "int", nullable: false),
                    Tu_vung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nghia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Audio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phien_am = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vi_du = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tu_dong_nghia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tu_trai_nghia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ma_bai_tu_vungId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Noidungbaituvungs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Noidungbaituvungs_Mabaituvungs_Ma_bai_tu_vungId",
                        column: x => x.Ma_bai_tu_vungId,
                        principalTable: "Mabaituvungs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Noidungbaituvungs_Ma_bai_tu_vungId",
                table: "Noidungbaituvungs",
                column: "Ma_bai_tu_vungId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Noidungbaituvungs");

            migrationBuilder.DropTable(
                name: "Mabaituvungs");
        }
    }
}
