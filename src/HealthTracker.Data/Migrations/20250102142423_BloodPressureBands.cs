using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class BloodPressureBands : Migration
    {
        [ExcludeFromCodeCoverage]
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BLOOD_PRESSURE_BAND",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: true),
                    minimum_systolic = table.Column<int>(type: "INTEGER", nullable: false),
                    maximum_systolic = table.Column<int>(type: "INTEGER", nullable: false),
                    minimum_diastolic = table.Column<int>(type: "INTEGER", nullable: false),
                    maximum_diastolic = table.Column<int>(type: "INTEGER", nullable: false),
                    order = table.Column<int>(type: "INTEGER", nullable: false),
                    match_all = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BLOOD_PRESSURE_BAND", x => x.id);
                });

            // Populate the table with blood pressure bandings per ESH (European Society of Hypertension) guidelines
            migrationBuilder.InsertData(
                table: "BLOOD_PRESSURE_BAND",
                columns: new[] { "id", "name", "minimum_systolic", "maximum_systolic", "minimum_diastolic", "maximum_diastolic", "order", "match_all" },
                values: new object[,]
                {
                    { 1, "Isolated Systolic Hypertension", 140, int.MaxValue, 0, 89, 1, true },
                    { 2, "Grade 3 Hypertension (Severe)", 180, int.MaxValue, 110, int.MaxValue, 2, false },
                    { 3, "Grade 2 Hypertension (Moderate)", 160, 179, 100, 109, 3, false },
                    { 4, "Grade 1 Hypertension (Mild)", 140, 159, 90, 99, 4, false },
                    { 5, "High Normal", 130, 139, 85, 89, 5, false },
                    { 6, "Normal", 120, 129, 80, 84, 6, false },
                    { 7, "Optimal", 0, 119, 0, 79, 7, false },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BLOOD_PRESSURE_BAND");
        }
    }
}
