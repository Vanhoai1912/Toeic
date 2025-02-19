using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toeic.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ArticleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_TestResults_TestResultId",
                table: "UserAnswers");

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten_bai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Noi_dung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "DATEADD(HOUR, 7, GETUTCDATE())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "DATEADD(HOUR, 7, GETUTCDATE())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_TestResults_TestResultId",
                table: "UserAnswers",
                column: "TestResultId",
                principalTable: "TestResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_TestResults_TestResultId",
                table: "UserAnswers");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_TestResults_TestResultId",
                table: "UserAnswers",
                column: "TestResultId",
                principalTable: "TestResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
