using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class BeverageMeasures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;");

            migrationBuilder.CreateTable(
                name: "BEVERAGE_MEASURES",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    volume = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BEVERAGE_MEASURES", x => x.id);
                });

            // Populate the table with the legacy enumeration-based measures
            migrationBuilder.InsertData(
                table: "BEVERAGE_MEASURES",
                columns: new[] { "id", "name", "volume" },
                values: new object[,]
                {
                    { 1, "Pint", 568.0 },
                    { 2, "Large Glass", 250.0 },
                    { 3, "Medium Glass", 175.0 },
                    { 4, "Small Glass", 125.0 },
                    { 5, "Shot", 25.0 },
                    { 6, "Half Pint", 284.0 },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BEVERAGE_MEASURES");
        }
    }
}
