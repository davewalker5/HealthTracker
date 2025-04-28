using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class MedicationTracking : Migration
    {
        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MEDICATION",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MEDICATION", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PERSON_MEDICATION",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    person_id = table.Column<int>(type: "INTEGER", nullable: false),
                    medication_id = table.Column<int>(type: "INTEGER", nullable: false),
                    daily_dose = table.Column<int>(type: "INTEGER", nullable: false),
                    stock = table.Column<int>(type: "INTEGER", nullable: false),
                    date = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PERSON_MEDICATION", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MEDICATION");

            migrationBuilder.DropTable(
                name: "PERSON_MEDICATION");
        }
    }
}
