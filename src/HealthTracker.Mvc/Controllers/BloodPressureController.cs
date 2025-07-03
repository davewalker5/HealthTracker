using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using HealthTracker.Enumerations.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class BloodPressureController : FilteredControllerBase<IBloodPressureMeasurementClient, BloodPressureListViewModel, BloodPressureMeasurement>
    {
        private readonly ILogger<BloodPressureController> _logger;

        public BloodPressureController(
            IPersonClient personClient,
            IBloodPressureMeasurementClient measurementClient,
            IHealthTrackerApplicationSettings settings,
            IFilterGenerator filterGenerator,
            IViewModelBuilder builder,
            ILogger<BloodPressureController> logger) : base(personClient, measurementClient, settings, filterGenerator, builder)
        {
            _logger = logger;
        }

        /// <summary>
        /// Serve the measurements list page
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int personId = 0, DateTime? start = null, DateTime? end = null)
        {
            _logger.LogDebug($"Rendering index view: Person ID = {personId}, From = {start}, To = {end}");

            var model = new BloodPressureListViewModel
            {
                PageNumber = 1,
                Filters = await _filterGenerator.Create(personId, start, end, ViewFlags.Add)
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
                int page = 1;
                switch (model.Action)
                {
                    case ControllerActions.ActionPreviousPage:
                        page = model.PageNumber - 1;
                        break;
                    case ControllerActions.ActionNextPage:
                        page = model.PageNumber + 1;
                        break;
                    case ControllerActions.ActionAdd:
                        return RedirectToAction("Add", new
                        {
                            personId = model.Filters.PersonId,
                            start = model.Filters.From,
                            end = model.Filters.To
                        });
                    case ControllerActions.ActionExport:
                        return RedirectToAction("ExportMeasurements", "Export", new
                        {
                            personId = model.Filters.PersonId,
                            start = model.Filters.From,
                            end = model.Filters.To,
                            exportType = DataExchangeType.BloodPressure
                        });
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                // Retrieve the matching records
                _logger.LogDebug(
                    $"Retrieving page {page} of blood pressure measurements for person with ID {model.Filters.PersonId}" +
                    $" in the date range {model.Filters.From:dd-MMM-yyyy} to {model.Filters.To:dd-MMM-yyyy}");

                var measurements = await _measurementClient.ListAsync(
                    model.Filters.PersonId, model.Filters.From, ToDate(model.Filters.To), page, _settings.ResultsPageSize);
                model.SetEntities(measurements, page, _settings.ResultsPageSize);

                if (measurements.Count > 0)
                {
                    model.Filters.ShowExportButton = true;
                }

                _logger.LogDebug($"{measurements.Count} matching blood pressure measurements retrieved");
            }
            else
            {
                LogModelStateErrors(_logger);
            }

            // Populate the list of people and render the view
            await _filterGenerator.PopulatePersonList(model.Filters);
            model.Filters.ShowAddButton = true;
            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new measurement
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add(int personId, DateTime start, DateTime end)
        {
            _logger.LogDebug($"Rendering add view: Person ID = {personId}, From = {start}, To = {end}");

            var model = new AddBloodPressureViewModel();
            model.CreateMeasurement(personId);
            await SetFilterDetails(model, personId, start, end);
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
            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index", new { personId = model.PersonId, start = model.From, end = model.To });
            }

            if (ModelState.IsValid)
            {
                // Capture person details
                var personId = model.Measurement.PersonId;
                var personName = model.PersonName;

                // Combine the date and time strings to produce a timestamp
                var timestamp = model.Timestamp();

                // Add the measurement
                _logger.LogDebug($"Adding blood pressure measurement: Person = {personName}, Timestamp = {timestamp}, Blood Pressure = {model.Measurement.Systolic}/{model.Measurement.Diastolic}");
                var measurement = await _measurementClient.AddAsync(personId, timestamp, model.Measurement.Systolic, model.Measurement.Diastolic);

                // Return the measurement list view containing only the new measurement and a confirmation message
                var message = $"Blood pressure measurement of {model.Measurement.Systolic}/{model.Measurement.Diastolic} for {personName} added successfully";
                var listModel = await CreateListViewModel(
                    measurement.PersonId,
                    measurement.Id,
                    timestamp,
                    timestamp,
                    message,
                    ViewFlags.ListView);

                return View("Index", listModel);
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
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id, string start, string end)
        {
            _logger.LogDebug($"Rendering edit view: Measurement ID = {id}, From = {start}, To = {end}");

            // Load the measurement to edit
            var measurement = await _measurementClient.GetAsync(id);

            // Construct the view model
            var model = new EditBloodPressureViewModel();
            model.SetMeasurement(measurement);
            await SetFilterDetails(model, measurement.PersonId, start, end);
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

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index", new { personId = model.Measurement.PersonId, start = model.From, end = model.To });
            }

            if (ModelState.IsValid)
            {
                // Combine the date and time strings to produce a timestamp
                var timestamp = model.Timestamp();

                // Update the measurement
                _logger.LogDebug($"Updating blood pressure measurement: ID = {model.Measurement.Id}, Person ID = {model.Measurement.PersonId}, Timestamp = {timestamp}, BloodPressure = {model.Measurement.Systolic}/{model.Measurement.Diastolic}");
                await _measurementClient.UpdateAsync(
                    model.Measurement.Id,
                    model.Measurement.PersonId,
                    timestamp,
                    model.Measurement.Systolic,
                    model.Measurement.Diastolic);

                // Return the measurement list view containing only the updated measurement and a confirmation message
                var listModel = await CreateListViewModel(
                    model.Measurement.PersonId,
                    model.Measurement.Id,
                    timestamp,
                    timestamp,
                    "Measurement successfully updated",
                    ViewFlags.ListView);

                return View("Index", listModel);
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
            var measurement = await _measurementClient.GetAsync(id);
            var personId = measurement.PersonId;
            var date = measurement.Date;

            // Delete the measurement
            _logger.LogDebug($"Deleting blood pressure measurement: ID = {id}");
            await _measurementClient.DeleteAsync(id);

            // Return the list view with an empty list of measurements
            var model = await CreateListViewModel(personId, 0, date, date, "Measurement successfully deleted", ViewFlags.ListView);
            return View("Index", model);
        }
    }
}