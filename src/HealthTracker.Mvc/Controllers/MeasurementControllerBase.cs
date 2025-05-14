using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    public abstract class MeasurementControllerBase<C, L, M> : HealthTrackerControllerBase
        where C: IMeasurementRetriever<M>
        where L: ListViewModelBase<M>, new()
        where M: class, new()
    {
        protected readonly C _measurementClient;
        protected readonly IHealthTrackerApplicationSettings _settings;
        protected readonly IFilterGenerator _filterGenerator;

        public MeasurementControllerBase(
            IPersonClient personClient,
            C measurementClient,
            IHealthTrackerApplicationSettings settings,
            IFilterGenerator filterGenerator) : base(personClient)
        {
            _measurementClient = measurementClient;
            _settings = settings;
            _filterGenerator = filterGenerator;
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
                var measurement = await _measurementClient.GetMeasurement(measurementId);
                model.SetEntities([measurement], 1, _settings.ResultsPageSize);
            }

            // Set the message
            model.Message = message;

            // Render the view
            return View("Index", model);
        }
    }
}