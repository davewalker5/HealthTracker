using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class FoodItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;");

            migrationBuilder.CreateTable(
                name: "FOOD_ITEMS",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    portion = table.Column<decimal>(type: "TEXT", nullable: false),
                    food_category_id = table.Column<int>(type: "INTEGER", nullable: false),
                    nutritional_value_id = table.Column<int>(type: "INTEGER", nullable: true),
                    name = table.Column<string>(type: "VARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FOOD_ITEMS", x => x.id);
                    table.ForeignKey(
                        name: "FK_FOOD_ITEMS_FOOD_CATEGORIES_food_category_id",
                        column: x => x.food_category_id,
                        principalTable: "FOOD_CATEGORIES",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FOOD_ITEMS_NUTRITIONAL_VALUES_nutritional_value_id",
                        column: x => x.nutritional_value_id,
                        principalTable: "NUTRITIONAL_VALUES",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FOOD_ITEMS_food_category_id",
                table: "FOOD_ITEMS",
                column: "food_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_FOOD_ITEMS_nutritional_value_id",
                table: "FOOD_ITEMS",
                column: "nutritional_value_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FOOD_ITEMS");
        }
    }
}
