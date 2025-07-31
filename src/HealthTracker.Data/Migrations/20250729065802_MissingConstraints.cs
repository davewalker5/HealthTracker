using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class MissingConstraints : Migration
    {
        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WEIGHT_person_id",
                table: "WEIGHT",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_SPO2_MEASUREMENT_person_id",
                table: "SPO2_MEASUREMENT",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_PLANNED_MEALS_person_id",
                table: "PLANNED_MEALS",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_PERSON_MEDICATION_medication_id",
                table: "PERSON_MEDICATION",
                column: "medication_id");

            migrationBuilder.CreateIndex(
                name: "IX_PERSON_MEDICATION_person_id",
                table: "PERSON_MEDICATION",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_MEAL_CONSUMPTION_person_id",
                table: "MEAL_CONSUMPTION",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_EXERCISE_person_id",
                table: "EXERCISE",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_CHOLESTEROL_person_id",
                table: "CHOLESTEROL",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_BLOOD_PRESSURE_person_id",
                table: "BLOOD_PRESSURE",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_BLOOD_GLUCOSE_person_id",
                table: "BLOOD_GLUCOSE",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_BEVERAGE_CONSUMPTION_person_id",
                table: "BEVERAGE_CONSUMPTION",
                column: "person_id");

            migrationBuilder.AddForeignKey(
                name: "FK_BEVERAGE_CONSUMPTION_BEVERAGES_person_id",
                table: "BEVERAGE_CONSUMPTION",
                column: "person_id",
                principalTable: "BEVERAGES",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BEVERAGE_CONSUMPTION_PEOPLE_person_id",
                table: "BEVERAGE_CONSUMPTION",
                column: "person_id",
                principalTable: "PEOPLE",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BLOOD_GLUCOSE_PEOPLE_person_id",
                table: "BLOOD_GLUCOSE",
                column: "person_id",
                principalTable: "PEOPLE",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BLOOD_PRESSURE_PEOPLE_person_id",
                table: "BLOOD_PRESSURE",
                column: "person_id",
                principalTable: "PEOPLE",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CHOLESTEROL_PEOPLE_person_id",
                table: "CHOLESTEROL",
                column: "person_id",
                principalTable: "PEOPLE",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EXERCISE_PEOPLE_person_id",
                table: "EXERCISE",
                column: "person_id",
                principalTable: "PEOPLE",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MEAL_CONSUMPTION_PEOPLE_person_id",
                table: "MEAL_CONSUMPTION",
                column: "person_id",
                principalTable: "PEOPLE",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PERSON_MEDICATION_MEDICATION_medication_id",
                table: "PERSON_MEDICATION",
                column: "medication_id",
                principalTable: "MEDICATION",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PERSON_MEDICATION_PEOPLE_person_id",
                table: "PERSON_MEDICATION",
                column: "person_id",
                principalTable: "PEOPLE",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PLANNED_MEALS_PEOPLE_person_id",
                table: "PLANNED_MEALS",
                column: "person_id",
                principalTable: "PEOPLE",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SPO2_MEASUREMENT_PEOPLE_person_id",
                table: "SPO2_MEASUREMENT",
                column: "person_id",
                principalTable: "PEOPLE",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WEIGHT_PEOPLE_person_id",
                table: "WEIGHT",
                column: "person_id",
                principalTable: "PEOPLE",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BEVERAGE_CONSUMPTION_BEVERAGES_person_id",
                table: "BEVERAGE_CONSUMPTION");

            migrationBuilder.DropForeignKey(
                name: "FK_BEVERAGE_CONSUMPTION_PEOPLE_person_id",
                table: "BEVERAGE_CONSUMPTION");

            migrationBuilder.DropForeignKey(
                name: "FK_BLOOD_GLUCOSE_PEOPLE_person_id",
                table: "BLOOD_GLUCOSE");

            migrationBuilder.DropForeignKey(
                name: "FK_BLOOD_PRESSURE_PEOPLE_person_id",
                table: "BLOOD_PRESSURE");

            migrationBuilder.DropForeignKey(
                name: "FK_CHOLESTEROL_PEOPLE_person_id",
                table: "CHOLESTEROL");

            migrationBuilder.DropForeignKey(
                name: "FK_EXERCISE_PEOPLE_person_id",
                table: "EXERCISE");

            migrationBuilder.DropForeignKey(
                name: "FK_MEAL_CONSUMPTION_PEOPLE_person_id",
                table: "MEAL_CONSUMPTION");

            migrationBuilder.DropForeignKey(
                name: "FK_PERSON_MEDICATION_MEDICATION_medication_id",
                table: "PERSON_MEDICATION");

            migrationBuilder.DropForeignKey(
                name: "FK_PERSON_MEDICATION_PEOPLE_person_id",
                table: "PERSON_MEDICATION");

            migrationBuilder.DropForeignKey(
                name: "FK_PLANNED_MEALS_PEOPLE_person_id",
                table: "PLANNED_MEALS");

            migrationBuilder.DropForeignKey(
                name: "FK_SPO2_MEASUREMENT_PEOPLE_person_id",
                table: "SPO2_MEASUREMENT");

            migrationBuilder.DropForeignKey(
                name: "FK_WEIGHT_PEOPLE_person_id",
                table: "WEIGHT");

            migrationBuilder.DropIndex(
                name: "IX_WEIGHT_person_id",
                table: "WEIGHT");

            migrationBuilder.DropIndex(
                name: "IX_SPO2_MEASUREMENT_person_id",
                table: "SPO2_MEASUREMENT");

            migrationBuilder.DropIndex(
                name: "IX_PLANNED_MEALS_person_id",
                table: "PLANNED_MEALS");

            migrationBuilder.DropIndex(
                name: "IX_PERSON_MEDICATION_medication_id",
                table: "PERSON_MEDICATION");

            migrationBuilder.DropIndex(
                name: "IX_PERSON_MEDICATION_person_id",
                table: "PERSON_MEDICATION");

            migrationBuilder.DropIndex(
                name: "IX_MEAL_CONSUMPTION_person_id",
                table: "MEAL_CONSUMPTION");

            migrationBuilder.DropIndex(
                name: "IX_EXERCISE_person_id",
                table: "EXERCISE");

            migrationBuilder.DropIndex(
                name: "IX_CHOLESTEROL_person_id",
                table: "CHOLESTEROL");

            migrationBuilder.DropIndex(
                name: "IX_BLOOD_PRESSURE_person_id",
                table: "BLOOD_PRESSURE");

            migrationBuilder.DropIndex(
                name: "IX_BLOOD_GLUCOSE_person_id",
                table: "BLOOD_GLUCOSE");

            migrationBuilder.DropIndex(
                name: "IX_BEVERAGE_CONSUMPTION_person_id",
                table: "BEVERAGE_CONSUMPTION");
        }
    }
}
