using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class CholesterolTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CHOLESTEROL",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    total = table.Column<decimal>(type: "TEXT", nullable: false),
                    hdl = table.Column<decimal>(type: "TEXT", nullable: false),
                    ldl = table.Column<decimal>(type: "TEXT", nullable: false),
                    triglycerides = table.Column<decimal>(type: "TEXT", nullable: false),
                    person_id = table.Column<int>(type: "INTEGER", nullable: false),
                    date = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHOLESTEROL", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHOLESTEROL");
        }
    }
}
