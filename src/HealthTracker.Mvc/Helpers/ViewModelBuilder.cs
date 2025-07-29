using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Entities.Measurements;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;

namespace HealthTracker.Mvc.Helpers
{
    public class ViewModelBuilder : IViewModelBuilder
    {
        private readonly ILogger<ViewModelBuilder> _logger;
        private readonly IFilterGenerator _filterGenerator;
        protected readonly IHealthTrackerApplicationSettings _settings;
        private readonly IPersonMedicationClient _personMedicationClient;
        private readonly IWeightMeasurementClient _weightMeasurementClient;
        private readonly IExerciseMeasurementClient _exerciseMeasurementClient;
        private readonly IBeverageConsumptionMeasurementClient _beverageConsumptionMeasurementClient;

        public ViewModelBuilder(
            IFilterGenerator filterGenerator,
            IHealthTrackerApplicationSettings settings,
            IPersonMedicationClient personMedicationClient,
            IWeightMeasurementClient weightMeasurementClient,
            IExerciseMeasurementClient exerciseMeasurementClient,
            IBeverageConsumptionMeasurementClient beverageConsumptionMeasurementClient,
            ILogger<ViewModelBuilder> logger)
        {
            _filterGenerator = filterGenerator;
            _settings = settings;
            _personMedicationClient = personMedicationClient;
            _weightMeasurementClient = weightMeasurementClient;
            _exerciseMeasurementClient = exerciseMeasurementClient;
            _beverageConsumptionMeasurementClient = beverageConsumptionMeasurementClient;
            _logger = logger;
        }

        /// <summary>
        /// Helper method to create a list view result when editing or deleting a measurement
        /// </summary>
        /// <param name="client"></param>
        /// <param name="personId"></param>
        /// <param name="measurementId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<L> CreateFilteredListViewModel<C, L, M>(
            C client,
            int personId,
            int measurementId,
            IEnumerable<M> measurements,
            DateTime? from,
            DateTime? to,
            string message,
            ViewFlags flags)

            where C : IDateBasedEntityRetriever<M>
            where L: FilteredViewModelBase<M>, new()
            where M: class, new()
        {
            // Create the model
            L model = new()
            {
                PageNumber = 1,
                Editable = flags.HasFlag(ViewFlags.Editable),
                Filters = await _filterGenerator.Create(personId, from, to, flags)
            };

            // Populate the list of people and select the person associated with this measurement
            model.Filters.PersonId = personId;
            await _filterGenerator.PopulatePersonList(model.Filters);

            // Populate the measurements list
            if (measurements != null)
            {
                // If some measurements are supplied, use those
                model.SetEntities(measurements, 1, _settings.ResultsPageSize);
            }
            else if (measurementId > 0)
            {
                // If a measurement ID is specified, load that single measurement
                var measurement = await client.GetAsync(measurementId);
                model.SetEntities([measurement], 1, _settings.ResultsPageSize);
            }
            else if (personId > 0)
            {
                // If a person is supplied, load the measurements for that person in the specified date range
                measurements = await client.ListAsync(personId, from, to, 1, _settings.ResultsPageSize);
                model.SetEntities(measurements, 1, _settings.ResultsPageSize);
            }

            // If requested, suppress the "no matches" message
            if (flags.HasFlag(ViewFlags.SuppressNoMatches))
            {
                model.HasNoMatchingResults = false;
            }

            // Set the message
                model.Message = message;

            // Return the model
            return model;
        }

        /// <summary>
        /// Helper method to create a weight list view model
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="measurementId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<WeightListViewModel> CreateWeightListViewModel(
            int personId,
            int measurementId,
            IEnumerable<WeightMeasurement> measurements,
            DateTime from,
            DateTime to,
            string message,
            ViewFlags flags)
            => await CreateFilteredListViewModel<IWeightMeasurementClient, WeightListViewModel, WeightMeasurement>(
                _weightMeasurementClient,
                personId,
                measurementId,
                measurements,
                from,
                to,
                message,
                flags);

        /// <summary>
        /// Helper method to create a beverage consumption list view model
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="measurementId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<BeverageConsumptionListViewModel> CreateBeverageConsumptionListViewModel(
            int personId,
            int measurementId,
            IEnumerable<BeverageConsumptionMeasurement> measurements,
            DateTime from,
            DateTime to,
            string message,
            ViewFlags flags)
            => await CreateFilteredListViewModel<IBeverageConsumptionMeasurementClient, BeverageConsumptionListViewModel, BeverageConsumptionMeasurement>(
                _beverageConsumptionMeasurementClient,
                personId,
                measurementId,
                measurements,
                from,
                to,
                message,
                flags);

        /// <summary>
        /// Helper method to create a list view result when editing or deleting an association
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<PersonMedicationListViewModel> CreatePersonMedicationListViewModel(
            int personId,
            string message,
            ViewFlags flags)
        {
            // Create the model
            var model = new PersonMedicationListViewModel
            {
                Filters = await _filterGenerator.Create(personId, flags),
                Settings = _settings,
                SelectedAssociationIds = "",
                Message = message,
                Editable = flags.HasFlag(ViewFlags.Editable)
            };

            // Retrieve and set the medication associations for the specified person, if specified
            if (personId > 0)
            {
                // Retrive the associations
                _logger.LogDebug($"Retrieving person/medication associations for person with ID {model.Filters.PersonId}");
                var associations = await _personMedicationClient.ListAsync(personId, 1, int.MaxValue);

                // Remove inactive ones, if required
                if (!flags.HasFlag(ViewFlags.IncludeInactive))
                {
                    associations.RemoveAll(x => !x.Active);
                }

                // Add the associations to the model
                model.SetEntities(associations, 1, int.MaxValue);
                _logger.LogDebug($"{associations.Count} matching person/medication associations retrieved");
            }

            // Return the model
            return model;
        }
    }
}