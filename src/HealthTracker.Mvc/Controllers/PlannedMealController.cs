using HealthTracker.Configuration.Interfaces;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Entities;
using HealthTracker.Entities.Food;
using HealthTracker.Enumerations.Enumerations;
using System.Globalization;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class PlannedMealController : FilteredControllerBase<IPlannedMealClient, PlannedMealListViewModel, PlannedMeal>
    {
        private const ViewFlags IndexViewFlags = ViewFlags.Add | ViewFlags.FutureDates | ViewFlags.Purge;
        private readonly IMealClient _mealClient;
        private readonly IMealListGenerator _mealListGenerator;
        private readonly IFoodSourceListGenerator _foodSourceListGenerator;

        public PlannedMealController(
            IPersonClient personClient,
            IMealClient mealClient,
            IPlannedMealClient measurementClient,
            IHealthTrackerApplicationSettings settings,
            IFilterGenerator filterGenerator,
            IFoodSourceListGenerator foodSourceListGenerator,
            IMealListGenerator mealListGenerator,
            IViewModelBuilder builder,
            IPartialViewToStringRenderer renderer,
            ILogger<ExerciseController> logger)
            : base(personClient, measurementClient, settings, filterGenerator, builder, renderer, logger)
        {
            _mealClient = mealClient;
            _foodSourceListGenerator = foodSourceListGenerator;
            _mealListGenerator = mealListGenerator;
        }

        /// <summary>
        /// Serve the scheduled meal list page
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int personId = 0, DateTime? start = null, DateTime? end = null)
        {
            _logger.LogDebug($"Rendering index view: Person ID = {personId}, From = {start}, To = {end}");

            var model = new PlannedMealListViewModel
            {
                PageNumber = 1,
                Filters = await _filterGenerator.Create(personId, start, end, IndexViewFlags)
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
        public async Task<IActionResult> Index(PlannedMealListViewModel model)
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
                            exportType = DataExchangeType.PlannedMeals
                        });
                    case ControllerActions.ActionShoppingList:
                        return RedirectToAction("Index", "ShoppingList", new
                        {
                            personId = model.Filters.PersonId,
                            from = model.Filters.From,
                            to = model.Filters.To
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
                    $"Retrieving page {page} of scheduled meals for person with ID {model.Filters.PersonId}" +
                    $" in the date range {model.Filters.From:dd-MMM-yyyy} to {model.Filters.To:dd-MMM-yyyy}");

                var plannedMeals = await _measurementClient.ListAsync(
                    model.Filters.PersonId, model.Filters.From, ToDate(model.Filters.To), page, _settings.ResultsPageSize);
                model.SetEntities(plannedMeals, page, _settings.ResultsPageSize);
                model.Filters.ShowPurgeButton = true;

                if (plannedMeals.Count > 0)
                {
                    model.Filters.ShowExportButton = true;
                }

                _logger.LogDebug($"{plannedMeals.Count} matching scheduled meals retrieved");
            }
            else
            {
                LogModelState();
            }

            // Populate the list of people and render the view
            await _filterGenerator.PopulatePersonList(model.Filters);
            model.Filters.ShowAddButton = true;
            model.Filters.ShowPurgeButton = true;
            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new scheduled meal
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add(int personId, DateTime start, DateTime end)
        {
            _logger.LogDebug($"Rendering add view: Person ID = {personId}, From = {start}, To = {end}");

            // Construct the view model
            var model = new AddPlannedMealViewModel()
            {
                People = await _filterGenerator.CreatePersonSelectList(),
                FoodSources = await _foodSourceListGenerator.Create()
            };

            model.CreatePlannedMeal();
            await SetFilterDetails(model, personId, start, end);
            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new scheuled meal given a meal ID
        /// </summary>
        /// <param name="mealId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AddFromMeal(int mealId)
        {
            _logger.LogDebug($"Rendering add from meal view: Meal ID = {mealId}");

            // Construct the view model
            var model = new AddPlannedMealViewModel()
            {
                People = await _filterGenerator.CreatePersonSelectList(),
                FoodSources = await _foodSourceListGenerator.Create()
            };

            var meal = await _mealClient.GetAsync(mealId);
            model.CreatePlannedMeal(meal);
            return View("Add", model);
        }

        /// <summary>
        /// Handle POST events to save new scheduled meals
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddPlannedMealViewModel model)
        {
            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index", new { personId = model.PersonId, start = model.From, end = model.To });
            }

            // The "Meal" entity is part of the hierarchy of the view model, allowing access to its properties in the
            // view without having to duplicate code, but this means validation of its properties will occur and result
            // in an invalid model state - remove errors for the properties we're not interested in
            ModelState.Remove("PlannedMeal.Meal.Name");
            ModelState.Remove("PlannedMeal.Meal.Portions");

            if (ModelState.IsValid)
            {
                // Convert the string representation of the date to a DateTime object
                var date = DateTime.ParseExact($"{model.Date} 00:00", DateFormats.DateTime, CultureInfo.InvariantCulture);

                // Update the scheduled meal
                _logger.LogDebug(
                    $"Adding scheduled meal: Person ID = {model.PlannedMeal.PersonId}, " +
                    $"Meal ID = {model.PlannedMeal.MealId}, " +
                    $"Meal Type = {model.PlannedMeal.MealType}, " +
                    $"Date = {date}");

                await _measurementClient.AddAsync(
                    model.PlannedMeal.PersonId,
                    model.PlannedMeal.MealType,
                    date,
                    model.PlannedMeal.MealId);

                // Return the measurement list view containing only the updated scheduled meal and a confirmation message
                var listModel = await CreateListViewModel(
                    model.PlannedMeal.PersonId,
                    model.PlannedMeal.Id,
                    date,
                    date,
                    "Scheduled meal updated",
                    IndexViewFlags | ViewFlags.Export);

                return View("Index", listModel);
            }
            else
            {
                LogModelState();
            }

            // Populate the activity types and render the view
            model.People = await _filterGenerator.CreatePersonSelectList();
            model.FoodSources = await _foodSourceListGenerator.Create();
            return View(model);
        }

        /// <summary>
        /// Serve the page to edit an existing scheduled meal
        /// </summary>
        /// <param name="id"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id, string start, string end)
        {
            _logger.LogDebug($"Rendering edit view: Scheduled meal ID = {id}, From = {start}, To = {end}");

            // Load the meal to edit
            var plannedMeal = await _measurementClient.GetAsync(id);

            // Construct the view model
            var model = new EditPlannedMealViewModel()
            {
                People = await _filterGenerator.CreatePersonSelectList(),
                FoodSources = await _foodSourceListGenerator.Create()
            };

            model.SetPlannedMeal(plannedMeal);
            await SetFilterDetails(model, plannedMeal.PersonId, start, end);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated scheduled meals
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditPlannedMealViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index", new { personId = model.PlannedMeal.PersonId, start = model.From, end = model.To });
            }

            // The "Meal" entity is part of the hierarchy of the view model, allowing access to its properties in the
            // view without having to duplicate code, but this means validation of its properties will occur and result
            // in an invalid model state - remove errors for the properties we're not interested in
            ModelState.Remove("PlannedMeal.Meal.Name");
            ModelState.Remove("PlannedMeal.Meal.Portions");

            if (ModelState.IsValid)
            {
                // Convert the string representation of the date to a DateTime object
                var date = DateTime.ParseExact($"{model.Date} 00:00", DateFormats.DateTime, CultureInfo.InvariantCulture);

                // Update the scheduled meal
                _logger.LogDebug(
                    $"Updating scheduled meal: ID = {model.PlannedMeal.Id}, " +
                    $"Person ID = {model.PlannedMeal.PersonId}, " +
                    $"Meal ID = {model.PlannedMeal.MealId}, " +
                    $"Meal Type = {model.PlannedMeal.MealType}, " +
                    $"Date = {date}");

                await _measurementClient.UpdateAsync(
                    model.PlannedMeal.Id,
                    model.PlannedMeal.PersonId,
                    model.PlannedMeal.MealType,
                    date,
                    model.PlannedMeal.MealId);

                // Return the measurement list view containing only the updated scheduled meal and a confirmation message
                var listModel = await CreateListViewModel(
                    model.PlannedMeal.PersonId,
                    model.PlannedMeal.Id,
                    date,
                    date,
                    "Scheduled meal successfully updated",
                    IndexViewFlags | ViewFlags.Export);

                return View("Index", listModel);
            }
            else
            {
                LogModelState();
                result = View(model);
            }

            // Populate the people and food source lists and render the view
            model.People = await _filterGenerator.CreatePersonSelectList();
            model.FoodSources = await _foodSourceListGenerator.Create();
            return result;
        }

        /// <summary>
        /// Handle POST events to delete an existing scheduled meal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Retrieve the scheduled meal and capture the person and date
            _logger.LogDebug($"Retrieving scheduled meal: ID = {id}");
            var measurement = await _measurementClient.GetAsync(id);
            var personId = measurement.PersonId;
            var date = measurement.Date;

            // Delete the scheduled meal
            _logger.LogDebug($"Deleting scheduled meal: ID = {id}");
            await _measurementClient.DeleteAsync(id);

            // Return the list view with an empty list of measurements
            var model = await CreateListViewModel(personId, 0, date, date, "Scheduled meal successfully deleted", IndexViewFlags);
            return View("Index", model);
        }

        /// <summary>
        /// Handle POST events to purge historical scheduled meals for a person
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purge(int personId)
        {
            // Submit the request to purge historical meals
            await _measurementClient.PurgeAsync(personId, DateTime.Now);
            var model = await CreateListViewModel(personId, 0, null, null, "The request to purge historical scheduled meals has been submitted", IndexViewFlags);
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
                TargetField = "PlannedMeal_MealId",
                Meals = await _mealListGenerator.Create(foodSourceId)
            };

            return PartialView("_MealsDropdownList", model);
        }
    }
}