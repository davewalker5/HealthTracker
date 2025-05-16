using System.Globalization;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;

namespace HealthTracker.Mvc.Controllers
{
    public abstract class MeasurementControllerBase<C, L, M> : HealthTrackerControllerBase
        where C: IEntityRetriever<M>
        where L: MeasurementListViewModelBase<M>, new()
        where M: class, new()
    {
        protected readonly C _measurementClient;
        protected readonly IPersonClient _personClient;
        protected readonly IHealthTrackerApplicationSettings _settings;
        protected readonly IFilterGenerator _filterGenerator;

        public MeasurementControllerBase()
            => _personClient = null;

        public MeasurementControllerBase(IPersonClient personClient)
            => _personClient = personClient;

        public MeasurementControllerBase(
            IPersonClient personClient,
            C measurementClient,
            IHealthTrackerApplicationSettings settings,
            IFilterGenerator filterGenerator)
        {
            _personClient = personClient;
            _measurementClient = measurementClient;
            _settings = settings;
            _filterGenerator = filterGenerator;
        }

        /// <summary>
        /// Set filter details on a view model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        protected async Task SetFilterDetails(IMeasurementFiltersViewModel model, int personId, string from, string to)
        {
            var fromDate = DateTime.ParseExact(from, DateFormats.DateTime, CultureInfo.InvariantCulture);
            var toDate = DateTime.ParseExact(to, DateFormats.DateTime, CultureInfo.InvariantCulture);
            await SetFilterDetails(model, personId, fromDate, toDate);
        }

        /// <summary>
        /// Set filter details on a view model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        protected async Task SetFilterDetails(IMeasurementFiltersViewModel model, int personId, DateTime from, DateTime to)
        {
            // Retrieve the person with whom the new measurement is to be associated
            var people = await _personClient.ListPeopleAsync(1, int.MaxValue);
            var person = people.First(x => x.Id == personId);

            // Set the filter details on the model
            model.PersonName = person.Name;
            model.From = from;
            model.To = to;
        }

        /// <summary>
        /// Helper method to create a list view result when editing or deleting a measurement
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="measurementId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected async Task<L> CreateListViewModel(int personId, int measurementId, DateTime from, DateTime to, string message)
        {
            // Create the model
            L model = new()
            {
                PageNumber = 1,
                Filters = await _filterGenerator.Create()
            };

            // Populate the list of people and select the person associated with this measurement
            model.Filters.PersonId = personId;
            await _filterGenerator.PopulatePersonList(model.Filters);

            // Set the from and to dates
            model.Filters.From = from;
            model.Filters.To = to;

            // Populate the list of measurements with the one of interest, if an ID is specified
            if (measurementId > 0)
            {
                var measurement = await _measurementClient.Get(measurementId);
                model.SetEntities([measurement], 1, _settings.ResultsPageSize);
            }

            // Set the message
            model.Message = message;

            // Return the model
            return model;
        }

        /// <summary>
        /// Convert a "to" date supplied by the date pickers, and therefore have a time of 00:00:00, to
        /// a date that will capture all data up to midnight on the specified date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        protected DateTime ToDate(DateTime date)
            => new(date.Year, date.Month, date.Day, 23, 59, 59);
    }
}