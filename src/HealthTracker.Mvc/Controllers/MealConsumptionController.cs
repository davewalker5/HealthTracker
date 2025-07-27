using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using HealthTracker.Enumerations.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class MealConsumptionController : FilteredControllerBase<IMealConsumptionMeasurementClient, MealConsumptionListViewModel, MealConsumptionMeasurement>
    {
        private readonly IMealListGenerator _mealListGenerator;
        private readonly IFoodSourceListGenerator _foodSourceListGenerator;
        private readonly IMealConsumptionMeasurementClient _client;

        public MealConsumptionController(
            IPersonClient personClient,
            IMealConsumptionMeasurementClient measurementClient,
            IMealConsumptionMeasurementClient client,
            IHealthTrackerApplicationSettings settings,
            IFilterGenerator filterGenerator,
            IMealListGenerator mealListGenerator,
            IFoodSourceListGenerator foodSourceListGenerator,
            IViewModelBuilder builder,
            IPartialViewToStringRenderer renderer,
            ILogger<MealConsumptionController> logger)
            : base(personClient, measurementClient, settings, filterGenerator, builder, renderer, logger)
        {
            _mealListGenerator = mealListGenerator;
            _foodSourceListGenerator = foodSourceListGenerator;
            _client = client;
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

            var model = new MealConsumptionListViewModel
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
        public async Task<IActionResult> Index(MealConsumptionListViewModel model)
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
                            exportType = DataExchangeType.MealConsumption
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
                    $"Retrieving page {page} of meal consumption measurements for person with ID {model.Filters.PersonId}" +
                    $" in the date range {model.Filters.From:dd-MMM-yyyy} to {model.Filters.To:dd-MMM-yyyy}");

                var measurements = await _measurementClient.ListAsync(
                    model.Filters.PersonId, model.Filters.From, ToDate(model.Filters.To), page, _settings.ResultsPageSize);
                model.SetEntities(measurements, page, _settings.ResultsPageSize);

                if (measurements.Count > 0)
                {
                    model.Filters.ShowExportButton = true;
                }

                _logger.LogDebug($"{measurements.Count} matching meal consumption measurements retrieved");
            }
            else
            {
                LogModelState();
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

            var model = new AddMealConsumptionViewModel()
            {
                Sources = await _foodSourceListGenerator.Create()
            };
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
        public async Task<IActionResult> Add(AddMealConsumptionViewModel model)
        {
            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index", new { personId = model.PersonId, start = model.From, end = model.To });
            }

            // The "Meal" entity is part of the hierarchy of the view model, allowing access to its properties in the
            // view without having to duplicate code, but this means validation of its properties will occur and result
            // in an invalid model state - remove errors for the properties we're not interested in
            ModelState.Remove("Measurement.Meal.Name");
            ModelState.Remove("Measurement.Meal.Portions");

            if (ModelState.IsValid)
            {
                // Combine the date and time strings to produce a timestamp
                var timestamp = model.Timestamp();

                // Add the measurement
                _logger.LogDebug(
                    $"Adding new meal consumption measurement: Person ID = {model.Measurement.PersonId}, " +
                    $"Meal ID = {model.Measurement.MealId}, " +
                    $"Timestamp = {timestamp}, " +
                    $"Quantity = {model.Measurement.Quantity}");

                var measurement = await _client.AddAsync(
                    model.Measurement.PersonId,
                    model.Measurement.MealId,
                    timestamp,
                    model.Measurement.Quantity);

                // Return the measurement list view containing only the new measurement and a confirmation message
                var message = $"Meal consumption measurement added successfully";
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
                LogModelState();
            }

            // Re-populate the drop downs and render the view
            model.Sources = await _foodSourceListGenerator.Create();
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
            var model = new EditMealConsumptionViewModel()
            {
                Sources = await _foodSourceListGenerator.Create()
            };
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
        public async Task<IActionResult> Edit(EditMealConsumptionViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index", new { personId = model.Measurement.PersonId, start = model.From, end = model.To });
            }

            // The "Meal" entity is part of the hierarchy of the view model, allowing access to its properties in the
            // view without having to duplicate code, but this means validation of its properties will occur and result
            // in an invalid model state - remove errors for the properties we're not interested in
            ModelState.Remove("Measurement.Meal.Name");
            ModelState.Remove("Measurement.Meal.Portions");

            if (ModelState.IsValid)
            {
                // Combine the date and time strings to produce a timestamp
                var timestamp = model.Timestamp();

                // Update the measurement
                _logger.LogDebug(
                    $"Updating meal consumption measurement: ID = {model.Measurement.Id}, " +
                    $"Person ID = {model.Measurement.PersonId}, " +
                    $"Beverage ID = {model.Measurement.Meal}, " +
                    $"Timestamp = {timestamp}, " +
                    $"Quantity = {model.Measurement.Quantity}");

                await _client.UpdateAsync(
                    model.Measurement.Id,
                    model.Measurement.PersonId,
                    model.Measurement.MealId,
                    timestamp,
                    model.Measurement.Quantity);

                // Return the measurement list view containing only the updated measurement and a confirmation message
                var listModel = await CreateListViewModel(
                    model.Measurement.PersonId,
                    model.Measurement.Id,
                    timestamp,
                    timestamp,
                    "Measurement successfully updated",
                    ViewFlags.ListView);

                result = View("Index", listModel);
            }
            else
            {
                LogModelState();
                result = View(model);
            }

            // Re-populate the drop downs and render the view
            model.Sources = await _foodSourceListGenerator.Create();
            return result;
        }

        /// <summary>
        /// Handle POST events to delete an existing measurement
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Retrieve the measurement and capture the person and date
            _logger.LogDebug($"Retrieving meal consumption measurement: ID = {id}");
            var measurement = await _measurementClient.GetAsync(id);
            var personId = measurement.PersonId;
            var date = measurement.Date;

            // Delete the measurement
            _logger.LogDebug($"Deleting meal consumption measurement: ID = {id}");
            await _measurementClient.DeleteAsync(id);

            // Return the list view with an empty list of measurements
            var model = await CreateListViewModel(personId, 0, date, date, "Measurement successfully deleted", ViewFlags.ListView);
            return View("Index", model);
        }

        /// <summary>
        /// Return the HTML markup for a drop-down list of meals, given a meal source
        /// </summary>
        /// <param name="foodSourceId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Meals(int foodSourceId)
        {
            var model = new MealsDropDownViewModel
            {
                Meals = await _mealListGenerator.Create(foodSourceId)
            };

            return PartialView(model);
        }
    }
}