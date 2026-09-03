using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeCanvas.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DayTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Weekday = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    PlannedStart = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    PlannedDurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActualMinutesUsed = table.Column<int>(type: "INTEGER", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemplateTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DayTemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlannedStart = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    PlannedDurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateTasks_DayTemplates_DayTemplateId",
                        column: x => x.DayTemplateId,
                        principalTable: "DayTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DayTemplates_Weekday",
                table: "DayTemplates",
                column: "Weekday",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Date",
                table: "Tasks",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateTasks_DayTemplateId",
                table: "TemplateTasks",
                column: "DayTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "TemplateTasks");

            migrationBuilder.DropTable(
                name: "DayTemplates");
        }
    }
}
