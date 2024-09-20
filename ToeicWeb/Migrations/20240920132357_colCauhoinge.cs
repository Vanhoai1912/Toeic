using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToeicWeb.Migrations
{
    /// <inheritdoc />
    public partial class colCauhoinge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "Cauhoibaitapnges",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserAnswer",
                table: "Cauhoibaitapnges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cauhoibaitapnges",
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
                table: "Cauhoibaitapnges");

            migrationBuilder.DropColumn(
                name: "UserAnswer",
                table: "Cauhoibaitapnges");
        }
    }
}
