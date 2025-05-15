using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class BloodPressureController : MeasurementControllerBase<IBloodPressureMeasurementClient, BloodPressureListViewModel, BloodPressureMeasurement>
    {
        private readonly ILogger<BloodPressureController> _logger;

        public BloodPressureController(
            IPersonClient personClient,
            IBloodPressureMeasurementClient measurementClient,
            IHealthTrackerApplicationSettings settings,
            IFilterGenerator filterGenerator,
            ILogger<BloodPressureController> logger) : base(personClient, measurementClient, settings, filterGenerator)
        {
            _logger = logger;
        }

        /// <summary>
        /// Serve the weight measurements page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new BloodPressureListViewModel
            {
                PageNumber = 1,
                Filters = await _filterGenerator.Create()
            };

            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(BloodPressureListViewModel model)
        {
            if (ModelState.IsValid)
            {
                int page = model.PageNumber;
                switch (model.Action)
                {
                    case ControllerActions.ActionPreviousPage:
                        page -= 1;
                        break;
                    case ControllerActions.ActionNextPage:
                        page += 1;
                        break;
                    case ControllerActions.ActionAdd:
                        return RedirectToAction("Add", new { personId = model.Filters.PersonId });
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                // Retrieve the matching weight records
                _logger.LogDebug(
                    $"Retrieving page {page} of blood pressure measurements for person with ID {model.Filters.PersonId}" +
                    $" in the date range {model.Filters.From:dd-MMM-yyyy} to {model.Filters.To:dd-MMM-yyyy}");

                var measurements = await _measurementClient.ListBloodPressureMeasurementsAsync(
                    model.Filters.PersonId, model.Filters.From, model.Filters.To, page, _settings.ResultsPageSize);
                model.SetEntities(measurements, page, _settings.ResultsPageSize);

                _logger.LogDebug($"{measurements.Count} matching blood pressure measurements retrieved");
            }
            else
            {
                LogModelStateErrors(_logger);
            }

            // Populate the list of people and render the view
            await _filterGenerator.PopulatePersonList(model.Filters);
            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new measurement
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add(int personId)
        {
            var model = new AddBloodPressureViewModel();
            model.Measurement.PersonId = personId;
            await SetPersonDetails(model, personId);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new measurements
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddBloodPressureViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Capture person details
                var personId = model.Measurement.PersonId;
                var personName = model.PersonName;

                // Add the measurement
                _logger.LogDebug($"Adding blood pressure measurement: Person = {personName}, Blood Pressure = {model.Measurement.Systolic}/{model.Measurement.Diastolic}");
                await _measurementClient.AddBloodPressureMeasurementAsync(personId, DateTime.Now, model.Measurement.Systolic, model.Measurement.Diastolic);
                model.Message = $"Blood pressure measurement of {model.Measurement.Systolic}/{model.Measurement.Diastolic} for {personName} added successfully";

                // Clear model state and configure the model
                ModelState.Clear();
                model.Clear();
            }
            else
            {
                LogModelStateErrors(_logger);
            }

            return View(model);
        }

        /// <summary>
        /// Serve the page to edit an existing measurement
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Load the measurement to edit
            var measurement = await _measurementClient.Get(id);

            // Construct the view model
            var model = new EditBloodPressureViewModel();
            model.Measurement = measurement;
            await SetPersonDetails(model, measurement.PersonId);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated measurements
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditBloodPressureViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                // Update the measurement
                _logger.LogDebug($"Updating blood pressure measurement: ID = {model.Measurement.Id}, Person ID = {model.Measurement.PersonId}, BloodPressure = {model.Measurement.Systolic}/{model.Measurement.Diastolic}");
                await _measurementClient.UpdateBloodPressureMeasurementAsync(
                    model.Measurement.Id,
                    model.Measurement.PersonId,
                    model.Measurement.Date,
                    model.Measurement.Systolic,
                    model.Measurement.Diastolic);

                // Return the measurement list view containing only the updated measurement and a confirmation message
                result = await CreateMeasurementListResult(
                    model.Measurement.PersonId,
                    model.Measurement.Id,
                    model.Measurement.Date,
                    model.Measurement.Date,
                    "Measurement successfully updated");

                return result;
            }
            else
            {
                LogModelStateErrors(_logger);
                result = View(model);
            }

            return result;
        }

        /// <summary>
        /// Handle POST events to delete an existing measurement
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Retrieve the measurement and capture the person and date
            _logger.LogDebug($"Retrieving blood pressure measurement: ID = {id}");
            var measurement = await _measurementClient.Get(id);
            var personId = measurement.PersonId;
            var date = measurement.Date;

            // Delete the measurement
            _logger.LogDebug($"Deleting blood pressure measurement: ID = {id}");
            await _measurementClient.DeleteBloodPressureMeasurementAsync(id);

            // Return the list view with an empty list of measurements
            var result = await CreateMeasurementListResult(personId, 0, date, date, "Measurement successfully deleted");
            return result;
        }
    }
}