using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Food;
using HealthTracker.Logic.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class BeverageMeasureManager : DatabaseManagerBase, IBeverageMeasureManager
    {
        internal BeverageMeasureManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first beverage measure matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<BeverageMeasure> GetAsync(Expression<Func<BeverageMeasure, bool>> predicate)
        {
            var BeverageMeasures = await ListAsync(predicate, 1, int.MaxValue);
            return BeverageMeasures.FirstOrDefault();
        }

        /// <summary>
        /// Return all beverage measures matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<BeverageMeasure>> ListAsync(Expression<Func<BeverageMeasure, bool>> predicate, int pageNumber, int pageSize)
            => await Context.BeverageMeasures
                            .Where(predicate)
                            .OrderBy(x => x.Name)
                            .ToListAsync();

        /// <summary>
        /// Add an beverage measure, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        public async Task<BeverageMeasure> AddAsync(string name, decimal volume)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Creating new beverage measure: Name = '{name}', Volume = {volume}");

            var clean = StringCleaner.Clean(name);
            await CheckBeverageMeasureIsNotADuplicate(clean, 0);

            var BeverageMeasure = new BeverageMeasure
            {
                Name = clean,
                Volume = volume
            };

            await Context.BeverageMeasures.AddAsync(BeverageMeasure);
            await Context.SaveChangesAsync();

            return BeverageMeasure;
        }


        /// <summary>
        /// Update the properties of the specified person
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        public async Task<BeverageMeasure> UpdateAsync(int id, string name, decimal volume)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating beverage measure with ID {id} : Name = '{name}', Volume = {volume}");

            var BeverageMeasure = Context.BeverageMeasures.FirstOrDefault(x => x.Id == id);
            if (BeverageMeasure != null)
            {
                // Clean up the description and check the operation won't create a duplicate
                var clean = StringCleaner.Clean(name);
                await CheckBeverageMeasureIsNotADuplicate(clean, id);

                // Save the changes
                BeverageMeasure.Name = clean;
                BeverageMeasure.Volume = volume;
                await Context.SaveChangesAsync();
            }

            return BeverageMeasure;
        }

        /// <summary>
        /// Delete the beverage measure with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Deleting beverage measure with ID {id}");

            var BeverageMeasure = await GetAsync(x => x.Id == id);
            if (BeverageMeasure != null)
            {
                // Delete the beverage measure record and save changes
                Factory.Context.Remove(BeverageMeasure);
                await Factory.Context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update an beverage measure with a duplicate
        /// description
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <exception cref="BeverageMeasureInUseException"></exception>
        private async Task CheckBeverageMeasureIsNotADuplicate(string name, int id)
        {
            var BeverageMeasure = await Context.BeverageMeasures.FirstOrDefaultAsync(x => x.Name == name);
            if ((BeverageMeasure != null) && (BeverageMeasure.Id != id))
            {
                var message = $"Beverage measure {name} already exists";
                throw new BeverageMeasureExistsException(message);
            }
        }
    }
}
