using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toeic.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Coll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Giai_thich_bai_doc",
                table: "Cauhoibaitapdocs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cauhoibaitapdocs",
                keyColumn: "Id",
                keyValue: 1,
                column: "Giai_thich_bai_doc",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Giai_thich_bai_doc",
                table: "Cauhoibaitapdocs");
        }
    }
}
