using System.Globalization;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;

namespace HealthTracker.Mvc.Controllers
{
    public abstract class FilteredControllerBase<C, L, M> : HealthTrackerControllerBase
        where C: IDateBasedEntityRetriever<M>
        where L: FilteredViewModelBase<M>, new()
        where M: class, new()
    {
        protected readonly C _measurementClient;
        protected readonly IPersonClient _personClient;
        protected readonly IHealthTrackerApplicationSettings _settings;
        protected readonly IFilterGenerator _filterGenerator;
        protected readonly IViewModelBuilder _builder;

        public FilteredControllerBase(
            IPartialViewToStringRenderer renderer,
            ILogger logger)
            : base(renderer, logger)
        {
            _personClient = null;
        }

        public FilteredControllerBase(
            IPersonClient personClient,
            IPartialViewToStringRenderer renderer,
            ILogger logger)
            : base(renderer, logger)
        {
            _personClient = personClient;
        }

        public FilteredControllerBase(
            IPersonClient personClient,
            C measurementClient,
            IHealthTrackerApplicationSettings settings,
            IFilterGenerator filterGenerator,
            IViewModelBuilder builder,
            IPartialViewToStringRenderer renderer,
            ILogger logger)
            : base(renderer, logger)
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
        protected async Task SetFilterDetails(IFiltersViewModel model, int personId, string from, string to)
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
        protected async Task SetFilterDetails(IFiltersViewModel model, int personId, DateTime from, DateTime to)
        {
            // Retrieve the person with whom the new measurement is to be associated
            var people = await _personClient.ListAsync(1, int.MaxValue);
            var person = people.FirstOrDefault(x => x.Id == personId);

            // Set the filter details on the model
            model.PersonName = person?.Name;
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
        /// <param name="flags"></param>
        /// <returns></returns>
        protected async Task<L> CreateListViewModel(int personId, int measurementId, DateTime? from, DateTime? to, string message, ViewFlags flags)
            => await _builder.CreateFilteredListViewModel<C, L, M>(_measurementClient, personId, measurementId, null, from, to, message, flags);

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