using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Interfaces;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class MealFoodItemController : HealthTrackerControllerBase
    {
        private readonly ILogger<MealFoodItemController> _logger;
        private readonly IMealHelper _mealHelper;
        private readonly IFoodCategoryListGenerator _foodCategoryListGenerator;
        private readonly IFoodItemListGenerator _foodItemListGenerator;
        private readonly IFoodItemClient _foodItemClient;
        private readonly IMealFoodItemClient _client;

        public MealFoodItemController(
            IMealHelper helper,
            IFoodCategoryListGenerator foodCategoryListGenerator,
            IFoodItemListGenerator foodItemListGenerator,
            IFoodItemClient foodItemClient,
            IMealFoodItemClient client,
            ILogger<MealFoodItemController> logger)
        {
            _mealHelper = helper;
            _foodCategoryListGenerator = foodCategoryListGenerator;
            _foodItemListGenerator = foodItemListGenerator;
            _foodItemClient = foodItemClient;
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
        /// Serve the page to add a new meal/food item relationship
        /// </summary>
        /// <param name="mealId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add(int mealId)
        {
            _logger.LogDebug($"Rendering add view: Meal ID = {mealId}");

            var meal = await _mealHelper.GetAsync(mealId);
            _logger.LogDebug($"Retrieved meal: {meal}");

            var model = new AddMealFoodItemViewModel()
            {
                FoodCategories = await _foodCategoryListGenerator.Create(),
                Meal = meal.Name
            };
            model.CreateRelationship(mealId);

            _logger.LogDebug($"View model = {model}");

            return View(model);
        }

        /// <summary>
        /// Serve the page to edit an existing meal/food item relationship
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogDebug($"Rendering edit view: Meal/Food Item Relationship ID = {id}");

            var relationship = await _client.GetAsync(id);

            var model = new EditMealFoodItemViewModel()
            {
                Relationship = relationship,
                Meal = (await _mealHelper.GetAsync(relationship.MealId)).Name,
                FoodCategories = await _foodCategoryListGenerator.Create()
            };

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

        /// <summary>
        /// Return the HTML markup for a drop-down list of food items, given a food category
        /// </summary>
        /// <param name="foodCategoryId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> FoodItems(int foodCategoryId)
        {
            var model = new FoodItemsDropDownViewModel
            {
                Items = await _foodItemListGenerator.Create(foodCategoryId)
            };

            return PartialView(model);
        }

        /// <summary>
        /// Return the HTML markup to tabulate the nutritional values for a food item
        /// </summary>
        /// <param name="foodItemId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> NutritionalValue(int foodItemId)
        {
            var foodItem = await _foodItemClient.GetAsync(foodItemId);
            var model = new NutritionalValueTableViewModel()
            {
                Portion = foodItem.Portion,
                Values = foodItem.NutritionalValue
            };

            return PartialView(model);
        }
    }
}