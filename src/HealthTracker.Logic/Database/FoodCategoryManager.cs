using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Food;
using HealthTracker.Logic.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class FoodCategoryManager : DatabaseManagerBase, IFoodCategoryManager
    {
        internal FoodCategoryManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first food category matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<FoodCategory> GetAsync(Expression<Func<FoodCategory, bool>> predicate)
        {
            var FoodCategories = await ListAsync(predicate, 1, int.MaxValue);
            return FoodCategories.FirstOrDefault();
        }

        /// <summary>
        /// Return all food categories matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<FoodCategory>> ListAsync(Expression<Func<FoodCategory, bool>> predicate, int pageNumber, int pageSize)
            => await Context.FoodCategories
                            .Where(predicate)
                            .OrderBy(x => x.Name)
                            .ToListAsync();

        /// <summary>
        /// Add a food category, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<FoodCategory> AddAsync(string name)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Creating new food category '{name}'");

            var clean = StringCleaner.Clean(name);
            await CheckFoodCategoryIsNotADuplicate(clean, 0);

            var foodCategory = new FoodCategory
            {
                Name = clean
            };

            await Context.FoodCategories.AddAsync(foodCategory);
            await Context.SaveChangesAsync();

            return foodCategory;
        }


        /// <summary>
        /// Update the properties of the specified food category
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<FoodCategory> UpdateAsync(int id, string name)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating food category with ID {id} to '{name}'");

            var foodCategory = Context.FoodCategories.FirstOrDefault(x => x.Id == id);
            if (foodCategory != null)
            {
                // Clean up the name and check the operation won't create a duplicate
                var clean = StringCleaner.Clean(name);
                await CheckFoodCategoryIsNotADuplicate(clean, id);

                // Save the changes
                foodCategory.Name = clean;
                await Context.SaveChangesAsync();
            }

            return foodCategory;
        }

        /// <summary>
        /// Delete the food category with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Deleting food category with ID {id}");

            var FoodCategory = await GetAsync(x => x.Id == id);
            if (FoodCategory != null)
            {
                // Check the food source isn't in use
                // TODO: Implement this once food items are implemented
                object? measurement = null;
                if (measurement != null)
                {
                    var message = $"Food category with Id {id} has food items associated with it and cannot be deleted";
                    throw new FoodCategoryInUseException(message);
                }

                // Delete the food source record and save changes
                Factory.Context.Remove(FoodCategory);
                await Factory.Context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a food category with a duplicate
        /// name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <exception cref="FoodCategoryInUseException"></exception>
        private async Task CheckFoodCategoryIsNotADuplicate(string name, int id)
        {
            var foodCategory = await Context.FoodCategories.FirstOrDefaultAsync(x => x.Name == name);
            if ((foodCategory != null) && (foodCategory.Id != id))
            {
                var message = $"Food source {name} already exists";
                throw new FoodCategoryExistsException(message);
            }
        }
    }
}
