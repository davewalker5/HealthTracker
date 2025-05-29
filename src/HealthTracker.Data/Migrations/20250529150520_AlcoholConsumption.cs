using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class AlcoholConsumption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;");

            migrationBuilder.CreateTable(
                name: "ALCOHOL_CONSUMPTION",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    beverage_id = table.Column<int>(type: "INTEGER", nullable: false),
                    measure = table.Column<int>(type: "INTEGER", nullable: false),
                    quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    abv = table.Column<decimal>(type: "TEXT", nullable: false),
                    person_id = table.Column<int>(type: "INTEGER", nullable: false),
                    date = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALCOHOL_CONSUMPTION", x => x.id);
                    table.ForeignKey(
                        name: "FK_ALCOHOL_CONSUMPTION_PEOPLE_person_id",
                        column: x => x.person_id,
                        principalTable: "PEOPLE",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ALCOHOL_CONSUMPTION_BEVERAGE_beverage_id",
                        column: x => x.beverage_id,
                        principalTable: "BEVERAGES",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ALCOHOL_CONSUMPTION");
        }
    }
}
