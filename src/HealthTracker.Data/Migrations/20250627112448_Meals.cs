using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class Meals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;");

            migrationBuilder.CreateTable(
                name: "MEALS",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    portions = table.Column<int>(type: "INTEGER", nullable: false),
                    food_source_id = table.Column<int>(type: "INTEGER", nullable: false),
                    nutritional_value_id = table.Column<int>(type: "INTEGER", nullable: true),
                    name = table.Column<string>(type: "VARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MEALS", x => x.id);
                    table.ForeignKey(
                        name: "FK_MEALS_FOOD_SOURCES_food_source_id",
                        column: x => x.food_source_id,
                        principalTable: "FOOD_SOURCES",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MEALS_NUTRITIONAL_VALUES_nutritional_value_id",
                        column: x => x.nutritional_value_id,
                        principalTable: "NUTRITIONAL_VALUES",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MEALS_food_source_id",
                table: "MEALS",
                column: "food_source_id");

            migrationBuilder.CreateIndex(
                name: "IX_MEALS_nutritional_value_id",
                table: "MEALS",
                column: "nutritional_value_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MEALS");
        }
    }
}
