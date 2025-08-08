using HealthTracker.Configuration.Interfaces;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Entities;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class MealSearchController : HealthTrackerControllerBase
    {
        private readonly IMealClient _client;
        private readonly IHealthTrackerApplicationSettings _settings;
        private readonly IFoodSourceListGenerator _sourceListGenerator;
        private readonly IFoodCategoryListGenerator _categoryListGenerator;

        public MealSearchController(
            IMealClient client,
            IHealthTrackerApplicationSettings settings,
            IFoodSourceListGenerator sourceListGenerator,
            IFoodCategoryListGenerator categoryListGenerator,
            IPartialViewToStringRenderer renderer,
            ILogger<MealController> logger) : base(renderer, logger)
        {
            _client = client;
            _settings = settings;
            _sourceListGenerator = sourceListGenerator;
            _categoryListGenerator = categoryListGenerator;
        }

        /// <summary>
        /// Serve the meal search page
        /// </summary>
        /// <param name="foodSourceId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int foodSourceId = 0)
        {
            _logger.LogDebug($"Rendering index view: Food Source ID = {foodSourceId}");

            var model = new MealSearchViewModel
            {
                PageNumber = 1,
                Sources = await _sourceListGenerator.Create(),
                Categories = await _categoryListGenerator.Create()
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
        public async Task<IActionResult> Index(MealSearchViewModel model)
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
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                // Retrieve the matching records
                _logger.LogDebug($"Retrieving page {page} of meals for criteria; {model.Criteria}");

                var meals = await _client.SearchAsync(
                    model.Criteria.FoodSourceId,
                    model.Criteria.FoodCategoryId,
                    model.Criteria.MealName,
                    model.Criteria.FoodItemName,
                    page,
                    _settings.ResultsPageSize);
                model.SetEntities(meals, page, _settings.ResultsPageSize);

                _logger.LogDebug($"{meals.Count} matching meals retrieved");
            }
            else
            {
                LogModelState();
            }

            // Re-populate the list of meal sources and food item categories
            model.Sources = await _sourceListGenerator.Create();
            model.Categories = await _categoryListGenerator.Create();

            return View(model);
        }
    }
}