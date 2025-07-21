using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class MealFoodItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;");

            migrationBuilder.CreateTable(
                name: "MEAL_FOOD_ITEMS",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    meal_id = table.Column<int>(type: "INTEGER", nullable: false),
                    food_item_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MEAL_FOOD_ITEMS", x => x.id);
                    table.ForeignKey(
                        name: "FK_MEAL_FOOD_ITEMS_FOOD_ITEMS_food_item_id",
                        column: x => x.food_item_id,
                        principalTable: "FOOD_ITEMS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MEAL_FOOD_ITEMS_MEALS_meal_id",
                        column: x => x.meal_id,
                        principalTable: "MEALS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MEAL_FOOD_ITEMS_food_item_id",
                table: "MEAL_FOOD_ITEMS",
                column: "food_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_MEAL_FOOD_ITEMS_meal_id",
                table: "MEAL_FOOD_ITEMS",
                column: "meal_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MEAL_FOOD_ITEMS");
        }
    }
}
