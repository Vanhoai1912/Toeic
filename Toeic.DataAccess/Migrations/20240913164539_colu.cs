using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toeic.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class colu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Mabaitapdocs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Mabaitapdocs",
                keyColumn: "Id",
                keyValue: 1,
                column: "FilePath",
                value: "");

            migrationBuilder.UpdateData(
                table: "Mabaitapdocs",
                keyColumn: "Id",
                keyValue: 2,
                column: "FilePath",
                value: "");

            migrationBuilder.UpdateData(
                table: "Mabaitapdocs",
                keyColumn: "Id",
                keyValue: 3,
                column: "FilePath",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Mabaitapdocs");
        }
    }
}
