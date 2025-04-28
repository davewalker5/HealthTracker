using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Extensions;
using HealthTracker.Entities.Medications;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Logging;

namespace HealthTracker.Logic.Medications
{
    public class MedicationStockUpdater : IMedicationStockUpdater
    {
        private readonly IHealthTrackerFactory _factory;

        internal MedicationStockUpdater(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Add a number of tablets to a medication association
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tablets"></param>
        public async Task<PersonMedication> AddStockAsync(int id, int tablets)
            => await _factory.PersonMedications.AddStockAsync(id, tablets);

        /// <summary>
        /// Set the stock level for  medication association
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tablets"></param>
        public async Task<PersonMedication> SetStockAsync(int id, int tablets)
            => await _factory.PersonMedications.SetStockAsync(id, tablets);

        /// <summary>
        /// Decrement stock of a medication association by the a number of doses
        /// </summary>
        /// <param name="id"></param>
        /// <param name="doses"></param>
        public async Task<PersonMedication> DecrementAsync(int id, int doses)
            => await UpdateStock(id, -doses);

        /// <summary>
        /// Decrement the stock of all active medications associated with a person by a number of doses
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="doses"></param>
        public async Task DecrementAllAsync(int personId, int doses)
        {
            var today = HealthTrackerDateExtensions.TodayWithoutTime();
            var associations = await _factory.PersonMedications
                                             .ListAsync(x =>    x.Active &&
                                                                (x.PersonId == personId) &&
                                                                ((x.LastTaken == null) || (x.LastTaken < today)));
            foreach (var association in associations)
            {
                await DecrementAsync(association.Id, doses);
            }
        }

        /// <summary>
        /// Increment stock of a medication association by a number of doses
        /// </summary>
        /// <param name="id"></param>
        /// <param name="doses"></param>
        public async Task<PersonMedication> IncrementAsync(int id, int doses)
            => await UpdateStock(id, doses);

        /// <summary>
        /// Increment the stock of all active medication associations for a person by a number of doses
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="doses"></param>
        public async Task IncrementAllAsync(int personId, int doses)
        {
            var associations = await _factory.PersonMedications.ListAsync(x => x.PersonId == personId);
            foreach (var association in associations.Where(x => x.Active))
            {
                await IncrementAsync(association.Id, doses);
            }
        }

        /// <summary>
        /// Fast forward a medication association stock level to the current date
        /// </summary>
        /// <param name="id"></param>
        public async Task<PersonMedication> FastForwardAsync(int id)
        {
            var today = HealthTrackerDateExtensions.TodayWithoutTime();
            var associations = await _factory.PersonMedications
                                            .ListAsync(x =>
                                                (x.Id == id) &&
                                                ((x.LastTaken == null) || (x.LastTaken < today)));
            await FastForward(associations);
            return associations.FirstOrDefault();
        }

        /// <summary>
        /// Fast forward stock levels for all active medication associations for a person to the current date
        /// </summary>
        /// <param name="personId"></param>
        public async Task FastForwardAllAsync(int personId)
        {
            var today = HealthTrackerDateExtensions.TodayWithoutTime();
            var associations = await _factory.PersonMedications
                                             .ListAsync(x =>
                                                x.Active &&
                                                (x.PersonId == personId) &&
                                                ((x.LastTaken == null) || (x.LastTaken < today)));
            await FastForward(associations);
        }

        /// <summary>
        /// Skip a dose for a specified medication association, just updating the stock date
        /// </summary>
        /// <param name="id"></param>
        public async Task<PersonMedication> SkipAsync(int id)
            => await UpdateStock(id, 0);

        /// <summary>
        /// Skip a dose for all active medication associations for a person, just updating the stock date
        /// </summary>
        /// <param name="personId"></param>
        public async Task SkipAllAsync(int personId)
        {
            var associations = await _factory.PersonMedications.ListAsync(x => x.PersonId == personId);
            foreach (var association in associations.Where(x => x.Active))
            {
                await SkipAsync(association.Id);
            }
        }

        /// <summary>
        /// Fast forward stock levels for a collection of medication associations to the current date
        /// </summary>
        /// <param name="associations"></param>
        /// <returns></returns>
        private async Task FastForward(IEnumerable<PersonMedication> associations)
        {
            foreach (var association in associations)
            {
                // Calculate the number of days between now and the last updated date - this is the number
                // of doses to fast-forward by (assuming one daily dose)
                var today = HealthTrackerDateExtensions.TodayWithoutTime();
                var stockDate = HealthTrackerDateExtensions.DateWithoutTime(association.LastTaken ?? today);
                var doses = (int)(today - stockDate).TotalDays;

                // Update the stock of each medication by decrementing this many doses
                if (doses > 0)
                {
                    await UpdateStock(association.Id, -doses);
                }
            }
        }

        /// <summary>
        /// Update the stock level for a medication association
        /// </summary>
        /// <param name="id"></param>
        /// <param name="doses"></param>
        private async Task<PersonMedication> UpdateStock(int id, int doses)
        {
            _factory.Logger.LogMessage(Severity.Info, $"Adding {doses} doses to the medication association with ID {id}");

            // Retrieve the association
            var associations = await _factory.PersonMedications.ListAsync(x => x.Id == id);
            var association = associations.FirstOrDefault();
            if (association != null)
            {
                // Calculate the updated stock level, making sure it doesn't drop below 0
                var updatedStock = association.Stock + doses * association.DailyDose;
                if (updatedStock < 0)
                {
                    updatedStock = 0;
                }

                _factory.Logger.LogMessage(Severity.Info, $"Stock level will be updated from {association.Stock} to {updatedStock}");
            
                // Calculate the updated "last taken" date
                var lastTaken = CalculateStockDate(association.LastTaken, doses);

                // Apply the updates
                association = await _factory.PersonMedications.SetStockAsync(id, updatedStock, lastTaken);

                // Determine actions for this medication
                _factory.MedicationActionGenerator.DetermineActions(association);
            }

            return association;
        }

        /// <summary>
        /// Calculate the updated stock date based on the current date and the number of doses
        /// </summary>
        /// <param name="stockDate"></param>
        /// <param name="doses"></param>
        /// <returns></returns>
        private DateTime CalculateStockDate(DateTime? stockDate, int doses)
        {
            DateTime date;

            // Report the parameters used to calculate the new stock date
            var reportLastTaken = stockDate == null ? "null" : stockDate.Value.ToShortDateString();
            _factory.Logger.LogMessage(Severity.Info, $"Calculating new stock date: Current stock date = {reportLastTaken}, doses = {doses}");

            // Scenarios:
            //
            // 1: The current date is not set, in which case the date is set to today
            // 2: "doses" is > 0, so we're adding tablets back onto stock i.e. "un-taking" and need to roll back the date
            // 3: "doses" is < 0, so we're subtracting tablets from the stock i.e. "taking" and need to roll forward the date
            // 4: "doses" is 0, so we're skipping and need to roll forward the date by one day
            if (stockDate != null)
            {
                int daysToAdvance = doses != 0 ? -doses : 1;
                date = HealthTrackerDateExtensions.DateWithoutTime(stockDate.Value.AddDays(daysToAdvance));

                _factory.Logger.LogMessage(Severity.Info, $"Advancing stock date by {daysToAdvance} days to {date.ToShortDateString()}");

                // If the resulting date is greater than now, then this is attempting to take doses beyond
                // today, which isn't allowed
                if (date > DateTime.Now)
                {
                    var message = $"Stock date {date.ToShortDateString()} is in the future";
                    throw new StockDateOutOfRangeException(message);
                }
            }
            else
            {
                date = HealthTrackerDateExtensions.TodayWithoutTime();
            }

            return date;
        }
    }
}