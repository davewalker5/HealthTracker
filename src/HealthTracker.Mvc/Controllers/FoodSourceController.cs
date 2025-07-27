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
    public class FoodSourceController : ReferenceDataControllerBase<FoodSourceListViewModel, FoodSource>
    {
        private readonly IFoodSourceClient _client;

        public FoodSourceController(
            IFoodSourceClient client,
            IHealthTrackerApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<FoodSourceController> logger) : base(settings, renderer, logger)
        {
            _client = client;
        }

        /// <summary>
        /// Serve the current list of food sources
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await CreateFoodSourceListViewModel("");
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(FoodSourceListViewModel model)
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
                var foodSources = await _client.ListAsync(page, _settings.ResultsPageSize);
                model.SetEntities(foodSources, page, _settings.ResultsPageSize);
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }
        
        /// <summary>
        /// Serve the page to add a new food source
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            var model = new AddFoodSourceViewModel();
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new food sources
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddFoodSourceViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Adding food source: Name = {model.FoodSource.Name}");
                var foodSource = await _client.AddAsync(model.FoodSource.Name);

                result = CreateListResult(foodSource, $"{foodSource.Name} successfully added");
            }
            else
            {
                LogModelState();
                result = View(model);
            }

            return result;
        }
        
        /// <summary>
        /// Serve the page to edit an existing food source
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var foodSources = await _client.ListAsync(1, int.MaxValue);
            var plural = foodSources.Count == 1 ? "" : "s";
            _logger.LogDebug($"{foodSources.Count} food ource{plural} loaded via the service");

            var foodSource = foodSources.First(x => x.Id == id);
            _logger.LogDebug($"Food source with ID {id} identified for editing");

            var model = new EditFoodSourceViewModel();
            model.FoodSource = foodSource;
            return View(model);
        }

        /// <summary>
        /// Handle POST events to update an existing food source
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditFoodSourceViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Updating food source: Id = {model.FoodSource.Id}, Name = {model.FoodSource.Name}");
                var foodSource = await _client.UpdateAsync(model.FoodSource.Id, model.FoodSource.Name);

                result = CreateListResult(foodSource, $"{foodSource.Name} successfully updated");
            }
            else
            {
                LogModelState();
                result = View(model);
            }

            return result;
        }

        /// <summary>
        /// Handle POST events to delete an existing food source
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Delete the item
            _logger.LogDebug($"Deleting food source: ID = {id}");
            await _client.DeleteAsync(id);

            // Return the list view with an empty list of items
            var message = $"Food source with ID {id} successfully deleted";
            var model = await CreateFoodSourceListViewModel(message);
            return View("Index", model);
        }

        /// <summary>
        /// Build the food source list view model
        /// </summary>
        /// <returns></returns>
        private async Task<FoodSourceListViewModel> CreateFoodSourceListViewModel(string message)
        {
            // Get the list of current foodSources
            var foodSources = await _client.ListAsync(1, _settings.ResultsPageSize);
            var plural = foodSources.Count == 1 ? "" : "s";
            _logger.LogDebug($"{foodSources.Count} food source{plural} loaded via the service");

            // Construct the view model and serve the page
            var model = new FoodSourceListViewModel()
            {
                Message = message
            };
            model.SetEntities(foodSources, 1, _settings.ResultsPageSize);
            return model;
        }
    }
}
