using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toeic.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UserAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "Cauhoibaithis");

            migrationBuilder.DropColumn(
                name: "UserAnswer",
                table: "Cauhoibaithis");

            migrationBuilder.CreateTable(
                name: "UserAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestResultId = table.Column<int>(type: "int", nullable: false),
                    CauHoiId = table.Column<int>(type: "int", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAnswers_Cauhoibaithis_CauHoiId",
                        column: x => x.CauHoiId,
                        principalTable: "Cauhoibaithis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_CauHoiId",
                table: "UserAnswers",
                column: "CauHoiId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAnswers");

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
    }
}
