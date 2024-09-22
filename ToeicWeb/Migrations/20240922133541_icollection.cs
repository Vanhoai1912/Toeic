using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToeicWeb.Migrations
{
    /// <inheritdoc />
    public partial class icollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Mabaitapnges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Mabaitapnges",
                keyColumn: "Id",
                keyValue: 1,
                column: "FilePath",
                value: null);

            migrationBuilder.UpdateData(
                table: "Mabaitapnges",
                keyColumn: "Id",
                keyValue: 2,
                column: "FilePath",
                value: null);

            migrationBuilder.UpdateData(
                table: "Mabaitapnges",
                keyColumn: "Id",
                keyValue: 3,
                column: "FilePath",
                value: null);

            migrationBuilder.UpdateData(
                table: "Mabaitapnges",
                keyColumn: "Id",
                keyValue: 4,
                column: "FilePath",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Mabaitapnges");
        }
    }
}
