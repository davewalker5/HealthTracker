using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class BeverageManager : DatabaseManagerBase, IBeverageManager
    {
        internal BeverageManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first beverage matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Beverage> GetAsync(Expression<Func<Beverage, bool>> predicate)
        {
            var beverages = await ListAsync(predicate, 1, int.MaxValue);
            return beverages.FirstOrDefault();
        }

        /// <summary>
        /// Return all beverages matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Beverage>> ListAsync(Expression<Func<Beverage, bool>> predicate, int pageNumber, int pageSize)
            => await Context.Beverages
                            .Where(predicate)
                            .OrderBy(x => x.Name)
                            .ToListAsync();

        /// <summary>
        /// Add a beverage, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typicalABV"></param>
        /// <returns></returns>
        public async Task<Beverage> AddAsync(string name, decimal typicalABV)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Creating new beverage '{name}'");

            var clean = StringCleaner.Clean(name);
            await CheckBeverageIsNotADuplicate(clean, 0);

            var beverage = new Beverage
            {
                Name = clean,
                TypicalABV = typicalABV
            };

            await Context.Beverages.AddAsync(beverage);
            await Context.SaveChangesAsync();

            return beverage;
        }


        /// <summary>
        /// Update the properties of the specified person
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="typicalABV"></param>
        /// <returns></returns>
        public async Task<Beverage> UpdateAsync(int id, string name, decimal typicalABV)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating beverage with ID {id} to '{name}'");

            var beverage = Context.Beverages.FirstOrDefault(x => x.Id == id);
            if (beverage != null)
            {
                // Clean up the name and check the operation won't create a duplicate
                var clean = StringCleaner.Clean(name);
                await CheckBeverageIsNotADuplicate(clean, id);

                // Save the changes
                beverage.Name = clean;
                beverage.TypicalABV = typicalABV;
                await Context.SaveChangesAsync();
            }

            return beverage;
        }

        /// <summary>
        /// Delete the person with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Deleting beverage with ID {id}");

            var beverage = await GetAsync(x => x.Id == id);
            if (beverage != null)
            {
                // Check the beverage isn't in use
                // TODO: Implement this once alcohol consumption records are in
                // var measurement = Context.AlcoholConsumptionMeasurements.FirstOrDefault(x => x.BeverageId == id);
                // if (measurement != null)
                // {
                //     var message = $"Beverage with Id {id} has measurements associated with it and cannot be deleted";
                //     throw new BeverageInUseException(message);
                // }

                // Delete the beverage record and save changes
                Factory.Context.Remove(beverage);
                await Factory.Context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update an beverage with a duplicate
        /// name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <exception cref="BeverageInUseException"></exception>
        private async Task CheckBeverageIsNotADuplicate(string name, int id)
        {
            var beverage = await Context.Beverages.FirstOrDefaultAsync(x => x.Name == name);
            if ((beverage != null) && (beverage.Id != id))
            {
                var message = $"Beverage {name} already exists";
                throw new BeverageExistsException(message);
            }
        }
    }
}
