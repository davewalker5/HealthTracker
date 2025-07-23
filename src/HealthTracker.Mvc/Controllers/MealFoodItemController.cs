using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthTracker.Client.Interfaces;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class MealFoodItemController : HealthTrackerControllerBase
    {
        private readonly ILogger<MealFoodItemController> _logger;
        private readonly IMealHelper _mealHelper;
        private readonly IMealFoodItemClient _client;

        public MealFoodItemController(
            IMealHelper helper,
            IMealFoodItemClient client,
            ILogger<MealFoodItemController> logger)
        {
            _mealHelper = helper;
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Serve the meal list page
        /// </summary>
        /// <param name="mealId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int mealId)
        {
            _logger.LogDebug($"Rendering index view: Meal ID = {mealId}");

            var model = new MealFoodItemListViewModel
            {
                Meal = await _mealHelper.GetAsync(mealId)
            };

            _logger.LogDebug($"Retrieved meal: {model.Meal}");

            return View(model);
        }

        /// <summary>
        /// Handle POST events to delete an existing meal/food item relationship
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Retrieve the relationship and capture the meal ID
            var relationship = await _client.GetAsync(id);
            var mealId = relationship.MealId;
    
            // Delete the item
            _logger.LogDebug($"Deleting meal/food item relationship: ID = {id}");
            await _client.DeleteAsync(id);

            // Return the list view with an empty list of items
            _logger.LogDebug($"Rendering index view: Meal ID = {mealId}");

            var model = new MealFoodItemListViewModel
            {
                Meal = await _mealHelper.GetAsync(mealId),
                Message = "Food item successfully removed from the meal"
            };

            return View("Index", model);
        }
    }
}