using Microsoft.EntityFrameworkCore;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using System.Linq.Expressions;
using HealthTracker.Entities.Medications;
using System.Globalization;
using HealthTracker.Logic.Extensions;
using HealthTracker.Entities.Logging;

namespace HealthTracker.Logic.Database
{
    public class MedicationManager : DatabaseManagerBase, IMedicationManager
    {
        private readonly TextInfo _info = CultureInfo.CurrentCulture.TextInfo;

        internal MedicationManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first medication matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Medication> GetAsync(Expression<Func<Medication, bool>> predicate)
        {
            var medications = await ListAsync(predicate);
            return medications.FirstOrDefault();
        }

        /// <summary>
        /// Return all medications matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<List<Medication>> ListAsync(Expression<Func<Medication, bool>> predicate)
            => await Context.Medications
                            .Where(predicate)
                            .OrderBy(x => x.Name)
                            .ToListAsync();

        /// <summary>
        /// Add a medication
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="DuplicateMedicationException"></exception>
        public async Task<Medication> AddAsync(string name)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Adding medication '{name}'");

            // Check the medication doesn't already exist
            var clean = StringCleaner.Clean(name);
            var medications = await ListAsync(x => x.Name == clean);
            if (medications.Any())
            {
                var message = $"Medication '{name}' already exists";
                throw new DuplicateMedicationException(message);
            }

            // Add it to the database
            var medication = new Medication { Name = name };
            await Context.Medications.AddAsync(medication);
            await Context.SaveChangesAsync();

            return medication;
        }

        
        /// <summary>
        /// Update the properties of the specified medication
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Medication> UpdateAsync(int id, string name)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating medication with ID {id} to '{name}'");

            // Check there isn't already a medication with the updated name
            var clean = StringCleaner.Clean(name);
            var medications = await ListAsync(x => (x.Name == clean) && (x.Id != id));
            if (medications.Any())
            {
                var message = $"Medication '{name}' already exists";
                throw new DuplicateMedicationException(message);
            }

            // Retrieve the medication and update it
            var medication = await Context.Medications.FirstAsync(x => x.Id == id);
            if (medication != null)
            {
                medication.Name = name;
                await Context.SaveChangesAsync();
            }

            return medication;
        }

        /// <summary>
        /// Delete the medication with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Deleting medication with ID {id}");

            // Find the medication record to check it exists
            var medication = await GetAsync(x => x.Id == id);
            if (medication != null)
            {
                // Check the medication isn't associated with a person
                var association = await Context.PersonMedications.FirstOrDefaultAsync(x => x.MedicationId == id);
                if (association != null)
                {
                    var message = $"Medication '{medication.Name}' with ID {id} is in use and cannot be deleted";
                    throw new MedicationInUseException(message);
                }

                // Delete the medication record and save changes
                Factory.Context.Remove(medication);
                await Factory.Context.SaveChangesAsync();
            }
        }
    }
}