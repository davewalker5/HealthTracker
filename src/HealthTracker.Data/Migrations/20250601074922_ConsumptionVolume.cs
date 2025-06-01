using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConsumptionVolume : Migration
    {
        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;");

            migrationBuilder.DropColumn(
                name: "measure",
                table: "BEVERAGE_CONSUMPTION");

            migrationBuilder.RenameColumn(
                name: "Volume",
                table: "BEVERAGE_CONSUMPTION",
                newName: "volume");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "volume",
                table: "BEVERAGE_CONSUMPTION",
                newName: "Volume");

            migrationBuilder.AddColumn<int>(
                name: "measure",
                table: "BEVERAGE_CONSUMPTION",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
