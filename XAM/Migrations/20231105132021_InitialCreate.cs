using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace XAM.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataHoldersTable",
                columns: table => new
                {
                    DataHolderId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TimeUntilNextCocktail = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CurrentCocktail = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataHoldersTable", x => x.DataHolderId);
                });

            migrationBuilder.CreateTable(
                name: "ExamsTable",
                columns: table => new
                {
                    ExamId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChallengeHighscore = table.Column<int>(type: "integer", nullable: false),
                    DataHolderId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamsTable", x => x.ExamId);
                    table.ForeignKey(
                        name: "FK_ExamsTable_DataHoldersTable_DataHolderId",
                        column: x => x.DataHolderId,
                        principalTable: "DataHoldersTable",
                        principalColumn: "DataHolderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatisticsTable",
                columns: table => new
                {
                    StatisticsId = table.Column<int>(type: "integer", nullable: false),
                    LifetimeCreatedExamsCounter = table.Column<int>(type: "integer", nullable: false),
                    LifetimeCreatedFlashcardsCounter = table.Column<int>(type: "integer", nullable: false),
                    TodayCreatedExamsCounter = table.Column<int>(type: "integer", nullable: false),
                    TodayCreatedFlashcardsCounter = table.Column<int>(type: "integer", nullable: false),
                    TodayHighscoresBeatenCounter = table.Column<int>(type: "integer", nullable: false),
                    TodayChallengesTakenCounter = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticsTable", x => x.StatisticsId);
                    table.ForeignKey(
                        name: "FK_StatisticsTable_DataHoldersTable_StatisticsId",
                        column: x => x.StatisticsId,
                        principalTable: "DataHoldersTable",
                        principalColumn: "DataHolderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlashcardsTable",
                columns: table => new
                {
                    FlashcardId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FrontText = table.Column<string>(type: "text", nullable: false),
                    BackText = table.Column<string>(type: "text", nullable: false),
                    ExamId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashcardsTable", x => x.FlashcardId);
                    table.ForeignKey(
                        name: "FK_FlashcardsTable_ExamsTable_ExamId",
                        column: x => x.ExamId,
                        principalTable: "ExamsTable",
                        principalColumn: "ExamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamsTable_DataHolderId",
                table: "ExamsTable",
                column: "DataHolderId");

            migrationBuilder.CreateIndex(
                name: "IX_FlashcardsTable_ExamId",
                table: "FlashcardsTable",
                column: "ExamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlashcardsTable");

            migrationBuilder.DropTable(
                name: "StatisticsTable");

            migrationBuilder.DropTable(
                name: "ExamsTable");

            migrationBuilder.DropTable(
                name: "DataHoldersTable");
        }
    }
}
