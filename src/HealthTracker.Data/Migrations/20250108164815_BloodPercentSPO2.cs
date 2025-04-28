using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class BloodPercentSPO2 : Migration
    {
        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SPO2_BAND",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: true),
                    minimum_spo2 = table.Column<decimal>(type: "TEXT", nullable: false),
                    maximum_spo2 = table.Column<decimal>(type: "TEXT", nullable: false),
                    minimum_age = table.Column<int>(type: "INTEGER", nullable: false),
                    maximum_age = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPO2_BAND", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SPO2_MEASUREMENT",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    percentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    person_id = table.Column<int>(type: "INTEGER", nullable: false),
                    date = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPO2_MEASUREMENT", x => x.id);
                });

            // Populate the banding table
            migrationBuilder.InsertData(
                table: "SPO2_BAND",
                columns: new[] { "id", "name", "minimum_spo2", "maximum_spo2", "minimum_age", "maximum_age" },
                values: new object[,]
                {
                    { 1, "Normal", 93.0, decimal.MaxValue, 0, 12 },
                    { 2, "Low", 0.0, 92.99, 0, 12 },
                    { 3, "Normal", 96.0, decimal.MaxValue, 13, 69 },
                    { 4, "Low", 0.0, 95.99, 13, 69 },
                    { 5, "Normal", 94.0, decimal.MaxValue, 70, int.MaxValue },
                    { 6, "Low", 0.0, 93.99, 70, int.MaxValue }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SPO2_BAND");

            migrationBuilder.DropTable(
                name: "SPO2_MEASUREMENT");
        }
    }
}
