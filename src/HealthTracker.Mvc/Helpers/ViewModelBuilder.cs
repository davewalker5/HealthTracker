using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Measurements;
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

        public ViewModelBuilder(
            IFilterGenerator filterGenerator,
            IHealthTrackerApplicationSettings settings,
            IPersonMedicationClient personMedicationClient,
            IWeightMeasurementClient weightMeasurementClient,
            IExerciseMeasurementClient exerciseMeasurementClient,
            ILogger<ViewModelBuilder> logger)
        {
            _filterGenerator = filterGenerator;
            _settings = settings;
            _personMedicationClient = personMedicationClient;
            _weightMeasurementClient = weightMeasurementClient;
            _exerciseMeasurementClient = exerciseMeasurementClient;
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
        /// <param name="editable"></param>
        /// <param name="showAddButton"></param>
        /// <returns></returns>
        public async Task<L> CreateFilteredListViewModel<C, L, M>(
            C client,
            int personId,
            int measurementId,
            IEnumerable<M> measurements,
            DateTime from,
            DateTime to,
            string message,
            bool editable,
            bool showAddButton)

            where C : IDateBasedEntityRetriever<M>
            where L: FilteredViewModelBase<M>, new()
            where M: class, new()
        {
            // Create the model
            L model = new()
            {
                PageNumber = 1,
                Editable = editable,
                Filters = await _filterGenerator.Create(personId, from, to, showAddButton)
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

            // Set the message
            model.Message = message;

            // Return the model
            return model;
        }

        /// <summary>
        /// Convenient wrapper to create a weight list view model
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="measurementId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <param name="editable"></param>
        /// <param name="showAddButton"></param>
        /// <returns></returns>
        public async Task<WeightListViewModel> CreateWeightListViewModel(
            int personId,
            int measurementId,
            IEnumerable<WeightMeasurement> measurements,
            DateTime from,
            DateTime to,
            string message,
            bool editable,
            bool showAddButton)
            => await CreateFilteredListViewModel<IWeightMeasurementClient, WeightListViewModel, WeightMeasurement>(
                _weightMeasurementClient,
                personId,
                measurementId,
                measurements,
                from,
                to,
                message,
                editable,
                showAddButton);

        /// <summary>
        /// Convenient wrapper to create an exercise list view model
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="measurementId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <param name="editable"></param>
        /// <param name="showAddButton"></param>
        /// <returns></returns>
        public async Task<ExerciseListViewModel> CreateExerciseListViewModel(
            int personId,
            int measurementId,
            IEnumerable<ExerciseMeasurement> measurements,
            DateTime from,
            DateTime to,
            string message,
            bool editable,
            bool showAddButton)
            => await CreateFilteredListViewModel<IExerciseMeasurementClient, ExerciseListViewModel, ExerciseMeasurement>(
                _exerciseMeasurementClient,
                personId,
                measurementId,
                measurements,
                from,
                to,
                message,
                editable,
                showAddButton);

        /// <summary>
        /// Helper method to create a list view result when editing or deleting an association
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="message"></param>
        /// <param name="includeInactive"></param>
        /// <param name="editable"></param>
        /// <returns></returns>
        public async Task<PersonMedicationListViewModel> CreatePersonMedicationListViewModel(
            int personId,
            string message,
            bool
            includeInactive,
            bool editable)
        {
            // Create the model
            var model = new PersonMedicationListViewModel
            {
                Filters = await _filterGenerator.Create(personId, true),
                Settings = _settings,
                SelectedAssociationIds = "",
                Message = message,
                Editable = editable
            };

            // Retrieve and set the medication associations for the specified person, if specified
            if (personId > 0)
            {
                // Retrive the associations
                _logger.LogDebug($"Retrieving person/medication associations for person with ID {model.Filters.PersonId}");
                var associations = await _personMedicationClient.ListAsync(personId, 1, int.MaxValue);

                // Remove inactive ones, if required
                if (!includeInactive)
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