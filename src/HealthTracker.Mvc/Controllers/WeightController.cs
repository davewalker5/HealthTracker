using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class WeightController : HealthTrackerControllerBase
    {

        private readonly IWeightMeasurementClient _measurementClient;
        private readonly IHealthTrackerApplicationSettings _settings;
        private readonly IFilterGenerator _filterGenerator;
        private readonly ILogger<WeightController> _logger;

        public WeightController(
            IPersonClient personClient,
            IWeightMeasurementClient measurementClient,
            IHealthTrackerApplicationSettings settings,
            IFilterGenerator filterGenerator,
            ILogger<WeightController> logger) : base(personClient)
        {
            _measurementClient = measurementClient;
            _settings = settings;
            _filterGenerator = filterGenerator;
            _logger = logger;
        }

        /// <summary>
        /// Serve the weight measurements page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new WeightListViewModel
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
        public async Task<IActionResult> Index(WeightListViewModel model)
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
                    $"Retrieving page {page} of weight measurements for person with ID {model.Filters.PersonId}" +
                    $" in the date range {model.Filters.From:dd-MMM-yyyy} to {model.Filters.To:dd-MMM-yyyy}");

                var measurements = await _measurementClient.ListWeightMeasurementsAsync(
                    model.Filters.PersonId, model.Filters.From, model.Filters.To, page, _settings.ResultsPageSize);
                model.SetEntities(measurements, page, _settings.ResultsPageSize);

                _logger.LogDebug($"{measurements.Count} matching weight measurements retrieved");
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
            var model = new AddWeightViewModel();
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
        public async Task<IActionResult> Add(AddWeightViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Capture person details
                var personId = model.Measurement.PersonId;
                var personName = model.PersonName;

                // Add the measurement
                _logger.LogDebug($"Adding weight measurement: Person = {personName}, Weight = {model.Measurement.Weight}");
                await _measurementClient.AddWeightMeasurementAsync(personId, DateTime.Now, model.Measurement.Weight);
                model.Message = $"Weight measurement of {model.Measurement.Weight:.##} for {personName} added successfully";

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
            var measurement = await _measurementClient.GetMeasurement(id);

            // Construct the view model
            var model = new EditWeightViewModel();
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
        public async Task<IActionResult> Edit(EditWeightViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                // Update the measurement
                _logger.LogDebug($"Updating weight measurement: ID = {model.Measurement.Id}, Person ID = {model.Measurement.PersonId}, Weight = {model.Measurement.Weight}");
                await _measurementClient.UpdateWeightMeasurementAsync(
                    model.Measurement.Id,
                    model.Measurement.PersonId,
                    model.Measurement.Date,
                    model.Measurement.Weight);

                // Serve the "list" view with the single measurement as the list of measurements.
                // First, create the model
                var listModel = new WeightListViewModel()
                {
                    PageNumber = 1,
                    Filters = await _filterGenerator.Create()
                };

                // Populate the list of people and select the person associated with this measurement
                listModel.Filters.PersonId = model.Measurement.PersonId;
                await _filterGenerator.PopulatePersonList(listModel.Filters);

                // Set the list of measurements
                var measurement = await _measurementClient.GetMeasurement(model.Measurement.Id);
                listModel.SetEntities([measurement], 1, _settings.ResultsPageSize);
                return View("Index", listModel);
            }
            else
            {
                LogModelStateErrors(_logger);
                result = View(model);
            }

            return result;
        }
    }
}