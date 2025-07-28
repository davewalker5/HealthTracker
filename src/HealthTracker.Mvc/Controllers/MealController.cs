using HealthTracker.Configuration.Interfaces;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using HealthTracker.Enumerations.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthTracker.Client.Interfaces;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class MealController : HealthTrackerControllerBase
    {
        private readonly IMealHelper _helper;
        private readonly IHealthTrackerApplicationSettings _settings;
        private readonly IFoodSourceListGenerator _listGenerator;
        private readonly IFoodSourceFilterGenerator _filterGenerator;

        public MealController(
            IMealHelper helper,
            IHealthTrackerApplicationSettings settings,
            IFoodSourceListGenerator listGenerator,
            IFoodSourceFilterGenerator filterGenerator,
            IPartialViewToStringRenderer renderer,
            ILogger<MealController> logger) : base(renderer, logger)
        {
            _helper = helper;
            _settings = settings;
            _listGenerator = listGenerator;
            _filterGenerator = filterGenerator;
        }

        /// <summary>
        /// Serve the meal list page
        /// </summary>
        /// <param name="foodSourceId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int foodSourceId = 0)
        {
            _logger.LogDebug($"Rendering index view: Food Source ID = {foodSourceId}");

            var model = new MealListViewModel
            {
                PageNumber = 1,
                Filters = await _filterGenerator.Create(foodSourceId, ViewFlags.ListView)
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
        public async Task<IActionResult> Index(MealListViewModel model)
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
                        return RedirectToAction("Add");
                    case ControllerActions.ActionExport:
                        return RedirectToAction("Export", "Export", new
                        {
                            exportType = DataExchangeType.Meals
                        });
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                // Retrieve the matching records
                _logger.LogDebug($"Retrieving page {page} of meals for source with ID {model.Filters.FoodSourceId}");

                var meals = await _helper.ListAsync(model.Filters.FoodSourceId, page, _settings.ResultsPageSize);
                model.SetEntities(meals, page, _settings.ResultsPageSize);

                if (meals.Count > 0)
                {
                    model.Filters.ShowExportButton = true;
                }

                _logger.LogDebug($"{meals.Count} matching meals retrieved");
            }
            else
            {
                LogModelState();
            }

            // Populate the list of people and render the view
            await _filterGenerator.PopulateFoodSourceList(model.Filters);
            model.Filters.ShowAddButton = true;
            model.Filters.ShowExportButton = true;
            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new meal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddMealViewModel() { FoodSources = await _listGenerator.Create() };
            model.CreateMeal();
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new meals
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddMealViewModel model)
        {
            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                // Add the meal and nutritional value record
                var meal = await _helper.AddAsync(model.Meal);

                // Return the meal list view containing only the new meal and a confirmation message
                var listModel = new MealListViewModel
                {
                    PageNumber = 1,
                    Filters = await _filterGenerator.Create(0, ViewFlags.ListView),
                    Message = "Meal added successfully",
                };

                listModel.SetEntities([meal], 1, _settings.ResultsPageSize);

                return View("Index", listModel);
            }
            else
            {
                LogModelState();
            }

            // Populate the food source list and render the view
            model.FoodSources = await _listGenerator.Create();
            return View(model);
        }

        /// <summary>
        /// Serve the page to edit an existing meal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogDebug($"Rendering edit view: Meal ID = {id}");

            var model = new EditMealViewModel()
            {
                Meal = await _helper.GetAsync(id),
                FoodSources = await _listGenerator.Create()
            };

            return View(model);
        }

        /// <summary>
        /// Handle POST events to save meals
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditMealViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                // Update the meal and nutritional value record
                var meal = await _helper.UpdateAsync(model.Meal);

                // Return the meal list view containing only the new meal and a confirmation message
                var listModel = new MealListViewModel
                {
                    PageNumber = 1,
                    Filters = await _filterGenerator.Create(0, ViewFlags.ListView),
                    Message = "Meal successfully updated",
                };

                listModel.SetEntities([meal], 1, _settings.ResultsPageSize);

                return View("Index", listModel);
            }
            else
            {
                LogModelState();
                result = View(model);
            }

            // Populate the food source list and render the view
            model.FoodSources = await _listGenerator.Create();
            return result;
        }

        /// <summary>
        /// Handle POST events to delete an existing meal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Delete the meal
            _logger.LogDebug($"Deleting meal: ID = {id}");
            await _helper.DeleteAsync(id);

            // Return the list view with an empty list of meals
            var model = new MealListViewModel
            {
                PageNumber = 1,
                Filters = await _filterGenerator.Create(0, ViewFlags.ListView),
                Message = "Meal successfully deleted"
            };

            return View("Index", model);
        }

        /// <summary>
        /// Show the modal dialog containing the nutritional values for the specified meal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ShowNutritionalValues(int id)
        {
            var meal = await _helper.GetAsync(id);
            var model = new NutritionalValueTableViewModel()
            {
                Portion = meal.Portions,
                Values = meal.NutritionalValue
            };

            var title = $"Nutritional Values for {meal.Name}";
            return await LoadModalContent("_NutritionalValues", model, title);
        }
    }
}