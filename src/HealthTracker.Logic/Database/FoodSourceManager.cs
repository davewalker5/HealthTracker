using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Food;
using HealthTracker.Logic.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class FoodSourceManager : DatabaseManagerBase, IFoodSourceManager
    {
        internal FoodSourceManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first food source matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<FoodSource> GetAsync(Expression<Func<FoodSource, bool>> predicate)
        {
            var FoodSources = await ListAsync(predicate, 1, int.MaxValue);
            return FoodSources.FirstOrDefault();
        }

        /// <summary>
        /// Return all food sources matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<FoodSource>> ListAsync(Expression<Func<FoodSource, bool>> predicate, int pageNumber, int pageSize)
            => await Context.FoodSources
                            .Where(predicate)
                            .OrderBy(x => x.Name)
                            .ToListAsync();

        /// <summary>
        /// Add a food source, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<FoodSource> AddAsync(string name)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Creating new food source '{name}'");

            var clean = StringCleaner.Clean(name);
            await CheckFoodSourceIsNotADuplicate(clean, 0);

            var foodSource = new FoodSource
            {
                Name = clean
            };

            await Context.FoodSources.AddAsync(foodSource);
            await Context.SaveChangesAsync();

            return foodSource;
        }


        /// <summary>
        /// Update the properties of the specified person
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<FoodSource> UpdateAsync(int id, string name)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating food source with ID {id} to '{name}'");

            var foodSource = Context.FoodSources.FirstOrDefault(x => x.Id == id);
            if (foodSource != null)
            {
                // Clean up the name and check the operation won't create a duplicate
                var clean = StringCleaner.Clean(name);
                await CheckFoodSourceIsNotADuplicate(clean, id);

                // Save the changes
                foodSource.Name = clean;
                await Context.SaveChangesAsync();
            }

            return foodSource;
        }

        /// <summary>
        /// Delete the food source with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Deleting food source with ID {id}");

            var FoodSource = await GetAsync(x => x.Id == id);
            if (FoodSource != null)
            {
                // Check the food source isn't in use
                // TODO: Implement this once food consumption measurements are implemented
                object? measurement = null;
                if (measurement != null)
                {
                    var message = $"Food source with Id {id} has consumption records associated with it and cannot be deleted";
                    throw new FoodSourceInUseException(message);
                }

                // Delete the food source record and save changes
                Factory.Context.Remove(FoodSource);
                await Factory.Context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a food source with a duplicate
        /// name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <exception cref="FoodSourceInUseException"></exception>
        private async Task CheckFoodSourceIsNotADuplicate(string name, int id)
        {
            var foodSource = await Context.FoodSources.FirstOrDefaultAsync(x => x.Name == name);
            if ((foodSource != null) && (foodSource.Id != id))
            {
                var message = $"Food source {name} already exists";
                throw new FoodSourceExistsException(message);
            }
        }
    }
}
