using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class BMIBands : Migration
    {
        [ExcludeFromCodeCoverage]
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BMI_BAND",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: true),
                    minimum_bmi = table.Column<decimal>(type: "TEXT", nullable: false),
                    maximum_bmi = table.Column<decimal>(type: "TEXT", nullable: false),
                    order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BMI_BAND", x => x.id);
                });

            // Populate the table with BMI bandings per NHS guidelines
            migrationBuilder.InsertData(
                table: "BMI_BAND",
                columns: new[] { "id", "name", "minimum_bmi", "maximum_bmi", "order" },
                values: new object[,]
                {
                    { 1, "Underweight", 0, 18.49, 1 },
                    { 2, "Normal", 18.5, 24.9, 1 },
                    { 3, "Overweight", 25, 29.9, 1 },
                    { 4, "Obese", 30, 34.9, 1 },
                    { 5, "Severely Obese", 35, 39.9, 1 },
                    { 6, "Morbidly Obese", 40, decimal.MaxValue, 1 },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BMI_BAND");
        }
    }
}
