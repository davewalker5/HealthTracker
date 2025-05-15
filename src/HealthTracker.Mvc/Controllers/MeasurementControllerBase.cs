using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Mvc;

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
        /// Set person details on a view model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        protected async Task SetPersonDetails(IMeasurementPersonViewModel model, int personId)
        {
            // Retrieve the person with whom the new measurement is to be associated
            var people = await _personClient.ListPeopleAsync(1, int.MaxValue);
            var person = people.First(x => x.Id == personId);

            // Set the person details on the model
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
        /// <returns></returns>
        protected async Task<IActionResult> CreateMeasurementListResult(int personId, int measurementId, DateTime from, DateTime to, string message)
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

            // Render the view
            return View("Index", model);
        }
    }
}