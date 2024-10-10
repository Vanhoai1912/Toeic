using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toeic.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class colBaithi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "Cauhoibaithis",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserAnswer",
                table: "Cauhoibaithis",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "Cauhoibaithis");

            migrationBuilder.DropColumn(
                name: "UserAnswer",
                table: "Cauhoibaithis");
        }
    }
}
