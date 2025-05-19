using Microsoft.EntityFrameworkCore;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using System.Linq.Expressions;
using HealthTracker.Entities.Medications;
using System.Globalization;
using HealthTracker.Entities.Logging;

namespace HealthTracker.Logic.Database
{
    public class PersonMedicationManager : DatabaseManagerBase, IPersonMedicationManager
    {
        private readonly TextInfo _info = CultureInfo.CurrentCulture.TextInfo;

        internal PersonMedicationManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return all associations matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<PersonMedication>> ListAsync(Expression<Func<PersonMedication, bool>> predicate, int pageNumber, int pageSize)
            => await Context.PersonMedications
                            .Where(predicate)
                            .OrderBy(x => x.PersonId)
                            .ThenBy(x => x.MedicationId)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

        /// <summary>
        /// Add an association
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="medicationId"></param>
        /// <param name="dose"></param>
        /// <param name="stock"></param>
        /// <param name="taken"></param>
        /// <returns></returns>
        public async Task<PersonMedication> AddAsync(int personId, int medicationId, int dose, int stock, DateTime? taken)
        {
            var reportTaken = taken == null ? "null" : taken.Value.ToShortDateString();
            Factory.Logger.LogMessage(Severity.Info, $"Adding medication association: Person ID {personId}, Medication ID {medicationId}, Daily Dose {dose}, Stock level {stock}, Last Taken {reportTaken}");

            // Make sure there isn't already an association between the person and the medication
            var association = await Context.PersonMedications.FirstOrDefaultAsync(x => (x.PersonId == personId) && (x.MedicationId == medicationId));
            if (association != null)
            {
                var message = $"Medication with ID {medicationId} is already associated with person with ID {personId}";
                throw new DuplicatePersonMedicationException(message);
            }

            // Validate the association properties
            await ValidateAssociationProperties(personId, medicationId, dose, stock, taken);

            // Add it to the database
            association = new PersonMedication
            {
                PersonId = personId,
                MedicationId = medicationId,
                DailyDose = dose,
                Stock = stock,
                LastTaken = taken,
                Active = true
            };
            await Context.PersonMedications.AddAsync(association);
            await Context.SaveChangesAsync();

            return association;
        }

        /// <summary>
        /// Make a person/medication association active
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PersonMedication> ActivateAsync(int id)
            => await SetActiveState(id, true);

        /// <summary>
        /// Make a person/medication association inactive
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PersonMedication> DeactivateAsync(int id)
            => await SetActiveState(id, false);
        
        /// <summary>
        /// Update the properties of the specified association
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="medicationId"></param>
        /// <param name="dose"></param>
        /// <param name="stock"></param>
        /// <param name="taken"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        /// <exception cref="DuplicateMedicationException"></exception>
        public async Task<PersonMedication> UpdateAsync(int id, int personId, int medicationId, int dose, int stock, DateTime? taken, bool active)
        {
            var reportTaken = taken == null ? "null" : taken.Value.ToShortDateString();
            Factory.Logger.LogMessage(Severity.Info, $"Updating medication association with ID {id}: Person ID {personId}, Medication ID {medicationId}, Daily Dose {dose}, Stock level {stock}, Last Taken {reportTaken}, Active {active}");

            // Make sure there isn't already an association between the person and the medication
            var association = await Context.PersonMedications
                                           .FirstOrDefaultAsync(x =>
                                                (x.PersonId == personId) &&
                                                (x.MedicationId == medicationId) &&
                                                (x.Id != id));
            if (association != null)
            {
                var message = $"Medication with ID {medicationId} is already associated with person with ID {personId}";
                throw new DuplicatePersonMedicationException(message);
            }

            // Validate the association properties
            await ValidateAssociationProperties(personId, medicationId, dose, stock, taken);

            // Retrieve the association and update it
            association = Context.PersonMedications.FirstOrDefault(x => x.Id == id);
            if (association != null)
            {
                association.PersonId = personId;
                association.MedicationId = medicationId;
                association.DailyDose = dose;
                association.Stock = stock;
                association.LastTaken = taken;
                association.Active = active;
                await Context.SaveChangesAsync();
            }

            return association;
        }

        /// <summary>
        /// Set the daily dose for the association with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tablets"></param>
        public async Task<PersonMedication> SetDoseAsync(int id, int tablets)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Setting the daily dose for medication association with ID {id} to {tablets}");

            // Make sure the dose is valid
            ValidateDose(tablets);

            // Retrieve and update the association
            var association = await Context.PersonMedications.FirstOrDefaultAsync(x => x.Id == id);
            if (association != null)
            {
                association.DailyDose = tablets;
                await Context.SaveChangesAsync();
            }

            return association;
        }

        /// <summary>
        /// Set the stock level and last taken date for an association
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tablets"></param>
        /// <returns></returns>
        public async Task<PersonMedication> SetStockAsync(int id, int tablets)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Setting the stock level for medication association with ID {id} to {tablets}");

            // Make sure the stock level and taken date are valid
            ValidateStock(tablets);

            // Retrieve and update the association
            var association = await Context.PersonMedications.FirstOrDefaultAsync(x => x.Id == id);
            if (association != null)
            {
                association.Stock = tablets;
                await Context.SaveChangesAsync();
            }

            return association;
        }

        /// <summary>
        /// Set the stock level and last taken date for an association
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tablets"></param>
        /// <param name="lastTaken"></param>
        /// <returns></returns>
        public async Task<PersonMedication> SetStockAsync(int id, int tablets, DateTime? lastTaken)
        {
            var reportTaken = lastTaken == null ? "null" : lastTaken.Value.ToShortDateString();
            Factory.Logger.LogMessage(Severity.Info, $"Setting the stock level for medication association with ID {id} to {tablets}, last taken on {reportTaken}");

            // Make sure the stock level and taken date are valid
            ValidateStock(tablets);
            ValidateLastTaken(lastTaken);

            // Retrieve and update the association
            var association = await Context.PersonMedications.FirstOrDefaultAsync(x => x.Id == id);
            if (association != null)
            {
                association.Stock = tablets;
                association.LastTaken = lastTaken;
                await Context.SaveChangesAsync();
            }

            return association;
        }
        
        /// <summary>
        /// Add a number of tablets to the stock for an association
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tablets"></param>
        public async Task<PersonMedication> AddStockAsync(int id, int tablets)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Adding {tablets} to the stock level for medication association with ID {id}");

            // Retrieve the association
            var association = await Context.PersonMedications.FirstOrDefaultAsync(x => x.Id == id);
            if (association != null)
            {
                // Make sure the resulting stock level will be valid
                ValidateStock(association.Stock + tablets);

                // Update the stock level
                association.Stock += tablets;
                await Context.SaveChangesAsync();
            }

            return association;
        }

        /// <summary>
        /// Delete an association
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Deleting the medication association with ID {id}");

            var associations = await ListAsync(x => x.Id == id, 1, int.MaxValue);
            if (associations.Any())
            {
                Factory.Context.Remove(associations.First());
                await Factory.Context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Validate the properties of an association
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="medicationId"></param>
        /// <param name="dose"></param>
        /// <param name="stock"></param>
        /// <param name="taken"></param>
        /// <exception cref="PersonNotFoundException"></exception>
        /// <exception cref="MedicationNotFoundException"></exception>
        /// <exception cref="InvalidDoseException"></exception>
        /// <exception cref="InvalidStockLevelException"></exception>
        /// <exception cref="InvalidDoseDateException"></exception>
        private async Task ValidateAssociationProperties(int personId, int medicationId, int dose, int stock, DateTime? taken)
        {
            // Make sure the person exists
            var person = await Factory.People.GetAsync(x => x.Id == personId);
            if (person == null)
            {
                var message = $"Person with ID {personId} does not exist";
                throw new PersonNotFoundException(message);
            }

            // Make sure the medication exists
            var medication = await Factory.Medications.GetAsync(x => x.Id == medicationId);
            if (medication == null)
            {
                var message = $"Medication with ID {medicationId} does not exist";
                throw new MedicationNotFoundException(message);
            }

            // Make sure the dose is valid
            ValidateDose(dose);

            // Make sure the stock level is valid
            ValidateStock(stock);

            // Make sure the date taken is not in the future
            ValidateLastTaken(taken);
        }

        /// <summary>
        /// Make sure the specified daily does is valid
        /// </summary>
        /// <param name="dose"></param>
        /// <exception cref="InvalidDoseException"></exception>
        private static void ValidateDose(int dose)
        {
            if (dose <= 0)
            {
                var message = $"{dose} is not a valid daily medication dose";
                throw new InvalidDoseException(message);
            }
        }

        /// <summary>
        /// Make sure the specified stock level is valid
        /// </summary>
        /// <param name="stock"></param>
        /// <exception cref="InvalidStockLevelException"></exception>
        private static void ValidateStock(int stock)
        {
            if (stock < 0)
            {
                var message = $"{stock} is not a valid medication stock level";
                throw new InvalidStockLevelException(message);
            }
        }

        /// <summary>
        /// Make sure the specified "last taken" date is valid
        /// </summary>
        /// <param name="taken"></param>
        /// <exception cref="InvalidDoseDateException"></exception>
        private static void ValidateLastTaken(DateTime? taken)
        {
            if ((taken != null) && (taken > DateTime.Now))
            {
                var message = $"{taken} is not a valid as a last taken date";
                throw new InvalidDoseDateException(message);
            }
        }

        /// <summary>
        /// Activate or deactivate a person medication
        /// </summary>
        /// <param name="id"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        private async Task<PersonMedication> SetActiveState(int id, bool active)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Seting active status for medication association with ID {id}: {active}");

            // Retrieve the association and update it
            var association = Context.PersonMedications.FirstOrDefault(x => x.Id == id);
            if (association != null)
            {
                association.Active = active;
                await Context.SaveChangesAsync();
            }

            return association;
        }
    }
}