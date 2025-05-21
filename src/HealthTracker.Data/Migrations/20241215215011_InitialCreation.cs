using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class InitialCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;");

            migrationBuilder.CreateTable(
                name: "PEOPLE",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    firstnames = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    surname = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    dob = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    height = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEOPLE", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ACTIVITY_TYPES",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    description = table.Column<string>(type: "VARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ACTIVITY_TYPES", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "BLOOD_PRESSURE",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    systolic = table.Column<int>(type: "INTEGER", nullable: false),
                    diastolic = table.Column<int>(type: "INTEGER", nullable: false),
                    person_id = table.Column<int>(type: "INTEGER", nullable: false),
                    date = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BLOOD_PRESSURE", x => x.id);
                    table.ForeignKey(
                        name: "FK_BLOOD_PRESSURE_PEOPLE_person_id",
                        column: x => x.person_id,
                        principalTable: "PEOPLE",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EXERCISE",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    activity_id = table.Column<int>(type: "INTEGER", nullable: false),
                    duration = table.Column<int>(type: "INTEGER", nullable: false),
                    distance = table.Column<decimal>(type: "TEXT", nullable: true),
                    calories = table.Column<int>(type: "INTEGER", nullable: false),
                    minimum_heart_rate = table.Column<int>(type: "INTEGER", nullable: false),
                    maximum_heart_rate = table.Column<int>(type: "INTEGER", nullable: false),
                    person_id = table.Column<int>(type: "INTEGER", nullable: false),
                    date = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXERCISE", x => x.id);
                    table.ForeignKey(
                        name: "FK_EXERCISE_PEOPLE_person_id",
                        column: x => x.person_id,
                        principalTable: "PEOPLE",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EXERCISE_ACTIVITY_activity_id",
                        column: x => x.activity_id,
                        principalTable: "ACTIVITY_TYPES",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "USER",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "WEIGHT",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    weight = table.Column<decimal>(type: "TEXT", nullable: false),
                    person_id = table.Column<int>(type: "INTEGER", nullable: false),
                    date = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WEIGHT", x => x.id);
                    table.ForeignKey(
                        name: "FK_WEIGHT_PEOPLE_person_id",
                        column: x => x.person_id,
                        principalTable: "PEOPLE",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ACTIVITY_TYPES");

            migrationBuilder.DropTable(
                name: "BLOOD_PRESSURE");

            migrationBuilder.DropTable(
                name: "EXERCISE");

            migrationBuilder.DropTable(
                name: "PEOPLE");

            migrationBuilder.DropTable(
                name: "USER");

            migrationBuilder.DropTable(
                name: "WEIGHT");
        }
    }
}
