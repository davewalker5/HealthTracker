using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracker.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class RestrictNutritionalValueDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                PRAGMA foreign_keys = ON;

                -- Drop the food items table
                DROP TABLE FOOD_ITEMS;

                -- Recreate the table with the new constraint
                CREATE TABLE FOOD_ITEMS (
                    id INTEGER NOT NULL CONSTRAINT PK_FOOD_ITEMS PRIMARY KEY AUTOINCREMENT,
                    food_category_id INTEGER NOT NULL,
                    name VARCHAR(100) NOT NULL,
                    nutritional_value_id INTEGER NULL,
                    portion TEXT NOT NULL,
                    CONSTRAINT FK_FOOD_ITEMS_FOOD_CATEGORIES_food_category_id FOREIGN KEY (food_category_id) REFERENCES FOOD_CATEGORIES (id) ON DELETE RESTRICT,
                    CONSTRAINT FK_FOOD_ITEMS_NUTRITIONAL_VALUES_nutritional_value_id FOREIGN KEY (nutritional_value_id) REFERENCES NUTRITIONAL_VALUES (id) ON DELETE RESTRICT
                );

                -- Create unique constraints
                CREATE INDEX IX_FOOD_ITEMS_food_category_id ON FOOD_ITEMS (food_category_id);
                CREATE UNIQUE INDEX IX_FOOD_ITEMS_nutritional_value_id ON FOOD_ITEMS (nutritional_value_id);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
