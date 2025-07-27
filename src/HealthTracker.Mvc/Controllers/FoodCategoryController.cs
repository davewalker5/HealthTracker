using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class FoodCategoryController : ReferenceDataControllerBase<FoodCategoryListViewModel, FoodCategory>
    {
        private readonly IFoodCategoryClient _client;

        public FoodCategoryController(
            IFoodCategoryClient client,
            IHealthTrackerApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<FoodCategoryController> logger) : base(settings, renderer, logger)
        {
            _client = client;
        }

        /// <summary>
        /// Serve the current list of food categories
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await CreateFoodCategoryListViewModel("");
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(FoodCategoryListViewModel model)
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
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                // Retrieve the matching records
                var foodCategorys = await _client.ListAsync(page, _settings.ResultsPageSize);
                model.SetEntities(foodCategorys, page, _settings.ResultsPageSize);
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }
        
        /// <summary>
        /// Serve the page to add a new food category
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            var model = new AddFoodCategoryViewModel();
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new food categories
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddFoodCategoryViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Adding food category: Name = {model.FoodCategory.Name}");
                var foodCategory = await _client.AddAsync(model.FoodCategory.Name);

                result = CreateListResult(foodCategory, $"{foodCategory.Name} successfully added");
            }
            else
            {
                LogModelState();
                result = View(model);
            }

            return result;
        }
        
        /// <summary>
        /// Serve the page to edit an existing food category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var foodCategorys = await _client.ListAsync(1, int.MaxValue);
            var plural = foodCategorys.Count == 1 ? "category" : "categories";
            _logger.LogDebug($"{foodCategorys.Count} food {plural} loaded via the service");

            var foodCategory = foodCategorys.First(x => x.Id == id);
            _logger.LogDebug($"Food category with ID {id} identified for editing");

            var model = new EditFoodCategoryViewModel();
            model.FoodCategory = foodCategory;
            return View(model);
        }

        /// <summary>
        /// Handle POST events to update an existing food category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditFoodCategoryViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Updating food category: Id = {model.FoodCategory.Id}, Name = {model.FoodCategory.Name}");
                var foodCategory = await _client.UpdateAsync(model.FoodCategory.Id, model.FoodCategory.Name);

                result = CreateListResult(foodCategory, $"{foodCategory.Name} successfully updated");
            }
            else
            {
                LogModelState();
                result = View(model);
            }

            return result;
        }

        /// <summary>
        /// Handle POST events to delete an existing food category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Delete the item
            _logger.LogDebug($"Deleting food category: ID = {id}");
            await _client.DeleteAsync(id);

            // Return the list view with an empty list of items
            var message = $"Food category with ID {id} successfully deleted";
            var model = await CreateFoodCategoryListViewModel(message);
            return View("Index", model);
        }

        /// <summary>
        /// Build the food category list view model
        /// </summary>
        /// <returns></returns>
        private async Task<FoodCategoryListViewModel> CreateFoodCategoryListViewModel(string message)
        {
            // Get the list of current food categories
            var categories = await _client.ListAsync(1, _settings.ResultsPageSize);
            var plural = categories.Count == 1 ? "category" : "categories";
            _logger.LogDebug($"{categories.Count} food {plural} loaded via the service");

            // Construct the view model and serve the page
            var model = new FoodCategoryListViewModel()
            {
                Message = message
            };
            model.SetEntities(categories, 1, _settings.ResultsPageSize);
            return model;
        }
    }
}
