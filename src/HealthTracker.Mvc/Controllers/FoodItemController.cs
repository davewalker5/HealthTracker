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
    public class FoodItemController : HealthTrackerControllerBase
    {
        private readonly ILogger<FoodItemController> _logger;
        private readonly IFoodItemHelper _helper;
        private readonly IHealthTrackerApplicationSettings _settings;
        private readonly IFoodCategoryListGenerator _listGenerator;
        private readonly IFoodCategoryFilterGenerator _filterGenerator;

        public FoodItemController(
            IFoodItemHelper helper,
            IHealthTrackerApplicationSettings settings,
            IFoodCategoryListGenerator listGenerator,
            IFoodCategoryFilterGenerator filterGenerator,
            ILogger<FoodItemController> logger)
        {
            _helper = helper;
            _settings = settings;
            _listGenerator = listGenerator;
            _filterGenerator = filterGenerator;
            _logger = logger;
        }

        /// <summary>
        /// Serve the items list page
        /// </summary>
        /// <param name="foodCategoryId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int foodCategoryId = 0)
        {
            _logger.LogDebug($"Rendering index view: Food Category ID = {foodCategoryId}");

            var model = new FoodItemListViewModel
            {
                PageNumber = 1,
                Filters = await _filterGenerator.Create(foodCategoryId, ViewFlags.ListView)
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
        public async Task<IActionResult> Index(FoodItemListViewModel model)
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
                            exportType = DataExchangeType.FoodItems
                        });
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                // Retrieve the matching records
                _logger.LogDebug($"Retrieving page {page} of food items for category with ID {model.Filters.FoodCategoryId}");

                var items = await _helper.ListAsync(model.Filters.FoodCategoryId, page, _settings.ResultsPageSize);
                model.SetEntities(items, page, _settings.ResultsPageSize);

                if (items.Count > 0)
                {
                    model.Filters.ShowExportButton = true;
                }

                _logger.LogDebug($"{items.Count} matching food items retrieved");
            }
            else
            {
                LogModelState(_logger);
            }

            // Populate the list of people and render the view
            await _filterGenerator.PopulateFoodCategoryList(model.Filters);
            model.Filters.ShowAddButton = true;
            model.Filters.ShowExportButton = true;
            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new item
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddFoodItemViewModel() { FoodCategories = await _listGenerator.Create() };
            model.CreateItem();
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new food items
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddFoodItemViewModel model)
        {
            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                // Add the food item and nutritional value record
                var item = await _helper.AddAsync(model.FoodItem);

                // Return the item list view containing only the new item and a confirmation message
                var listModel = new FoodItemListViewModel
                {
                    PageNumber = 1,
                    Filters = await _filterGenerator.Create(0, ViewFlags.ListView),
                    Message = "Food item added successfully",
                };

                listModel.SetEntities([item], 1, _settings.ResultsPageSize);

                return View("Index", listModel);
            }
            else
            {
                LogModelState(_logger);
            }

            // Populate the food category list and render the view
            model.FoodCategories = await _listGenerator.Create();
            return View(model);
        }

        /// <summary>
        /// Serve the page to edit an existing item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogDebug($"Rendering edit view: Food Item ID = {id}");

            var model = new EditFoodItemViewModel()
            {
                FoodItem = await _helper.GetAsync(id),
                FoodCategories = await _listGenerator.Create()
            };

            return View(model);
        }

        /// <summary>
        /// Handle POST events to save food items
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditFoodItemViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {

                // Update the food item and nutritional value record
                var item = await _helper.UpdateAsync(model.FoodItem);

                // Return the item list view containing only the new item and a confirmation message
                var listModel = new FoodItemListViewModel
                {
                    PageNumber = 1,
                    Filters = await _filterGenerator.Create(0, ViewFlags.ListView),
                    Message = "Food item successfully updated",
                };

                listModel.SetEntities([item], 1, _settings.ResultsPageSize);

                return View("Index", listModel);
            }
            else
            {
                LogModelState(_logger);
                result = View(model);
            }

            // Populate the food category list and render the view
            model.FoodCategories = await _listGenerator.Create();
            return result;
        }

        /// <summary>
        /// Handle POST events to delete an existing food item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Delete the item
            _logger.LogDebug($"Deleting food item: ID = {id}");
            await _helper.DeleteAsync(id);

            // Return the list view with an empty list of items
            var model = new FoodItemListViewModel
            {
                PageNumber = 1,
                Filters = await _filterGenerator.Create(0, ViewFlags.ListView),
                Message = "Food item successfully deleted"
            };

            return View("Index", model);
        }
    }
}