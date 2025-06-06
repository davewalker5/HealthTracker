using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class NutritionalValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;");

            migrationBuilder.CreateTable(
                name: "NUTRITIONAL_VALUES",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    calories = table.Column<decimal>(type: "TEXT", nullable: true),
                    fat = table.Column<decimal>(type: "TEXT", nullable: true),
                    saturated_fat = table.Column<decimal>(type: "TEXT", nullable: true),
                    protein = table.Column<decimal>(type: "TEXT", nullable: true),
                    carbohydrates = table.Column<decimal>(type: "TEXT", nullable: true),
                    sugar = table.Column<decimal>(type: "TEXT", nullable: true),
                    fibre = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NUTRITIONAL_VALUES", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NUTRITIONAL_VALUES");
        }
    }
}
