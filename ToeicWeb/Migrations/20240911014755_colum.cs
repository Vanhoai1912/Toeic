using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToeicWeb.Migrations
{
    /// <inheritdoc />
    public partial class colum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tieu_de",
                table: "Cauhoibaitapdocs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cauhoibaitapdocs",
                keyColumn: "Id",
                keyValue: 1,
                column: "Tieu_de",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tieu_de",
                table: "Cauhoibaitapdocs");
        }
    }
}
