using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToeicWeb.Migrations
{
    /// <inheritdoc />
    public partial class colCauhoi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "Cauhoibaitapdocs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserAnswer",
                table: "Cauhoibaitapdocs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cauhoibaitapdocs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "IsCorrect", "UserAnswer" },
                values: new object[] { false, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "Cauhoibaitapdocs");

            migrationBuilder.DropColumn(
                name: "UserAnswer",
                table: "Cauhoibaitapdocs");
        }
    }
}
