using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;

namespace HealthTracker.Mvc.Controllers
{
    public abstract class FilteredByPersonControllerBase<C, L, M> : HealthTrackerControllerBase
        where C: IPersonBasedEntityRetriever<M>
        where L: FilteredByPersonViewModelBase<M>, new()
        where M: class, new()
    {
        protected readonly C _measurementClient;
        protected readonly IPersonClient _personClient;
        protected readonly IHealthTrackerApplicationSettings _settings;
        protected readonly IFilterGenerator _filterGenerator;
        protected readonly IViewModelBuilder _builder;

        public FilteredByPersonControllerBase()
            => _personClient = null;

        public FilteredByPersonControllerBase(IPersonClient personClient)
            => _personClient = personClient;

        public FilteredByPersonControllerBase(
            IPersonClient personClient,
            C measurementClient,
            IHealthTrackerApplicationSettings settings,
            IFilterGenerator filterGenerator,
            IViewModelBuilder builder)
        {
            _personClient = personClient;
            _measurementClient = measurementClient;
            _settings = settings;
            _filterGenerator = filterGenerator;
            _builder = builder;
        }

        /// <summary>
        /// Set filter details on a view model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        protected async Task SetFilterDetails(IPersonFilterViewModel model, int personId)
        {
            // Retrieve the person with whom the new measurement is to be associated
            var people = await _personClient.ListAsync(1, int.MaxValue);
            var person = people.First(x => x.Id == personId);

            // Set the filter details on the model
            model.PersonName = person.Name;
        }

        /// <summary>
        /// Helper method to create a list view result when editing or deleting a measurement
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="measurementId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <param name="showAddButton"></param>
        /// <returns></returns>
        protected async Task<L> CreateListViewModel(int personId, int measurementId, string message, bool showAddButton)
        {
            // Create the model
            L model = new()
            {
                PageNumber = 1,
                Filters = await _filterGenerator.Create(personId, showAddButton)
            };

            // Populate the list of people and select the person associated with this measurement
            model.Filters.PersonId = personId;
            await _filterGenerator.PopulatePersonList(model.Filters);

            // Populate the list of measurements with the one of interest, if an ID is specified
            if (measurementId > 0)
            {
                var measurement = await _measurementClient.GetAsync(measurementId);
                model.SetEntities([measurement], 1, _settings.ResultsPageSize);
            }

            // Set the message
            model.Message = message;

            // Return the model
            return model;
        }
    }
}