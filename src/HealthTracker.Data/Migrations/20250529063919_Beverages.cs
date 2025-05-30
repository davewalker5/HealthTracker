using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class Beverages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;");

            migrationBuilder.CreateTable(
                name: "BEVERAGES",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    typical_abv = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BEVERAGES", x => x.id);
                });

            // Populate the table with a default set of beverages. The typical ABV is based
            // on the typical values in the UK, taking mid-range values
            migrationBuilder.InsertData(
                table: "BEVERAGES",
                columns: new[] { "id", "name", "typical_abv" },
                values: new object[,]
                {
                    { 1, "Water", 0.0 },
                    { 2, "White Wine", 12.5 },
                    { 3, "Red Wine", 13.5 },
                    { 4, "Sparkling Wine", 11 },
                    { 5, "Beer", 4.5 },
                    { 6, "Lager", 4.0 },
                    { 7, "Vodka", 40.0 },
                    { 8, "Gin", 40.0 },
                    { 9, "Whiskey", 40.0 },
                    { 10, "Rum", 40.0 },
                    { 11, "Fino Sherry", 15.0 },
                    { 12, "Amontillado Sherry", 16.0 },
                    { 13, "Oloroso Sherry", 20.0 },
                    { 14, "Cream Sherry", 17.5 },
                    { 15, "Port", 20.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BEVERAGES");
        }
    }
}
