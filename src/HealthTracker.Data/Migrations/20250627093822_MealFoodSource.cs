using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class MealFoodSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;");

            migrationBuilder.AlterColumn<int>(
                name: "portions",
                table: "MEALS",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "food_source_id",
                table: "MEALS",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MEALS_food_source_id",
                table: "MEALS",
                column: "food_source_id");

            migrationBuilder.AddForeignKey(
                name: "FK_MEALS_FOOD_SOURCES_food_source_id",
                table: "MEALS",
                column: "food_source_id",
                principalTable: "FOOD_SOURCES",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MEALS_FOOD_SOURCES_food_source_id",
                table: "MEALS");

            migrationBuilder.DropIndex(
                name: "IX_MEALS_food_source_id",
                table: "MEALS");

            migrationBuilder.DropColumn(
                name: "food_source_id",
                table: "MEALS");

            migrationBuilder.AlterColumn<decimal>(
                name: "portions",
                table: "MEALS",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}
