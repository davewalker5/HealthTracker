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
                    { 1, "White Wine", 12.5 },
                    { 2, "Red Wine", 13.5 },
                    { 3, "Sparkling Wine", 11 },
                    { 4, "Beer", 4.5 },
                    { 5, "Lager", 4.0 },
                    { 6, "Vodka", 40.0 },
                    { 7, "Gin", 40.0 },
                    { 8, "Whiskey", 40.0 },
                    { 9, "Rum", 40.0 },
                    { 10, "Fino Sherry", 15.0 },
                    { 11, "Amontillado Sherry", 16.0 },
                    { 12, "Oloroso Sherry", 20.0 },
                    { 13, "Cream Sherry", 17.5 },
                    { 14, "Port", 20.0 }
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
