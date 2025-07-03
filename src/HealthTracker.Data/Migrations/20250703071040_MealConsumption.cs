using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class MealConsumption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;");

            migrationBuilder.CreateTable(
                name: "MEAL_CONSUMPTION",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    meal_id = table.Column<int>(type: "INTEGER", nullable: false),
                    quantity = table.Column<decimal>(type: "TEXT", nullable: false),
                    nutritional_value_id = table.Column<int>(type: "INTEGER", nullable: true),
                    person_id = table.Column<int>(type: "INTEGER", nullable: false),
                    date = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MEAL_CONSUMPTION", x => x.id);
                    table.ForeignKey(
                        name: "FK_MEAL_CONSUMPTION_MEALS_meal_id",
                        column: x => x.meal_id,
                        principalTable: "MEALS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MEAL_CONSUMPTION_NUTRITIONAL_VALUES_nutritional_value_id",
                        column: x => x.nutritional_value_id,
                        principalTable: "NUTRITIONAL_VALUES",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MEAL_CONSUMPTION_meal_id",
                table: "MEAL_CONSUMPTION",
                column: "meal_id");

            migrationBuilder.CreateIndex(
                name: "IX_MEAL_CONSUMPTION_nutritional_value_id",
                table: "MEAL_CONSUMPTION",
                column: "nutritional_value_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MEAL_CONSUMPTION");
        }
    }
}
