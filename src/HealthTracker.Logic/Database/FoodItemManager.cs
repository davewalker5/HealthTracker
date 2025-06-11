using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Food;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Logic.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class FoodItemManager : MeasurementManagerBase, IFoodItemManager
    {
        internal FoodItemManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return all items matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<FoodItem>> ListAsync(Expression<Func<FoodItem, bool>> predicate, int pageNumber, int pageSize)
            => await Context.FoodItems
                            .Include(x => x.FoodCategory)
                            .Include(x => x.NutritionalValue)
                            .Where(predicate)
                            .OrderBy(x => x.Name)
                            .ThenBy(x => x.FoodCategory.Name)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

        /// <summary>
        /// Add a new item
        /// </summary>
        /// <param name="name"></param>
        /// <param name="portion"></param>
        /// <param name="foodCategoryId"></param>
        /// <param name="nutritionalValueId"></param>
        /// <returns></returns>
        public async Task<FoodItem> AddAsync(string name, decimal portion, int foodCategoryId, int? nutritionalValueId)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Adding food item: Name = {name}, Portion = {portion}, Category ID = {foodCategoryId}, Nutritional Value ID = {nutritionalValueId}");

            // Clean up the name and make sure we're not creating a duplicate and that the
            // related category and nutritional value exist
            var clean = StringCleaner.Clean(name);
            CheckCategoryExists(foodCategoryId);
            CheckNutritionalValueExists(nutritionalValueId);
            await CheckFoodItemIsNotADuplicate(clean, 0);

            // Create the item
            var item = new FoodItem
            {
                Name = clean,
                Portion = portion,
                FoodCategoryId = foodCategoryId,
                NutritionalValueId = nutritionalValueId
            };

            // Add it to the database
            await Context.FoodItems.AddAsync(item);
            await Context.SaveChangesAsync();

            return item;
        }

        /// <summary>
        /// Update the properties of the specified item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="portion"></param>
        /// <param name="foodCategoryId"></param>
        /// <param name="nutritionalValueId"></param>
        /// <returns></returns>
        public async Task<FoodItem> UpdateAsync(int id, string name, decimal portion, int foodCategoryId, int? nutritionalValueId)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating food item with ID {id}: Name = {name}, Portion = {portion}, Category ID = {foodCategoryId}, Nutritional Value ID = {nutritionalValueId}");

            // Retrieve the item for update
            var item = Context.FoodItems.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                // Clean up the name and make sure we're not creating a duplicate and that the
                // related category and nutritional value exist
                var clean = StringCleaner.Clean(name);
                CheckCategoryExists(foodCategoryId);
                CheckNutritionalValueExists(nutritionalValueId);
                await CheckFoodItemIsNotADuplicate(clean, id);

                // Update the item
                item.Name = clean;
                item.Portion = portion;
                item.FoodCategoryId = foodCategoryId;
                item.NutritionalValueId = nutritionalValueId;
                await Context.SaveChangesAsync();

                // Reload the associated food category and nutritional value
                await Context.Entry(item).Reference(x => x.FoodCategory).LoadAsync();
                await Context.Entry(item).Reference(x => x.NutritionalValue).LoadAsync();
            }

            return item;
        }

        /// <summary>
        /// Delete the measurement with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Deleting food item with ID {id}");

            var item = Context.FoodItems.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                Factory.Context.Remove(item);
                await Factory.Context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Check a food category with a specified ID exists and raise an exception if not
        /// </summary>
        /// <param name="foodCategoryId"></param>
        protected void CheckCategoryExists(int foodCategoryId)
        {
            var category = Context.FoodCategories.FirstOrDefault(x => x.Id == foodCategoryId);
            if (category == null)
            {
                var message = $"Food category with Id {foodCategoryId} does not exist";
                throw new FoodCategoryNotFoundException(message);
            }
        }

        /// <summary>
        /// Check a nutritional vaue with a specified ID exists and raise an exception if not
        /// </summary>
        /// <param name="nutritionalValueId"></param>
        protected void CheckNutritionalValueExists(int? nutritionalValueId)
        {
            if (nutritionalValueId != null)
            {
                var nutrition = Context.NutritionalValues.FirstOrDefault(x => x.Id == nutritionalValueId);
                if (nutrition == null)
                {
                    var message = $"Nutritional value with Id {nutritionalValueId} does not exist";
                    throw new NutritionalValueNotFoundException(message);
                }
            }
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a food item with a duplicate
        /// name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <exception cref="FoodItemExistsException"></exception>
        private async Task CheckFoodItemIsNotADuplicate(string name, int id)
        {
            var foodItem = await Context.FoodItems.FirstOrDefaultAsync(x => x.Name == name);
            if ((foodItem != null) && (foodItem.Id != id))
            {
                var message = $"Food item {name} already exists";
                throw new FoodItemExistsException(message);
            }
        }
    }
}
