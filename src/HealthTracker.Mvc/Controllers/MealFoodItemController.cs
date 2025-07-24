using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Entities;

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
        /// Handle POST events to save new meal/food item relationships
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddMealFoodItemViewModel model)
        {
            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index", new { mealId = model.Relationship.MealId });
            }

            _logger.LogDebug($"Adding meal/food item relationship: {model.Relationship}");

            // The "FoodItem" entity is part of the hierarchy of the view model, allowing access to its properties in the
            // view without having to duplicate code, but this means validation of its properties will occur and result
            // in an invalid model state - remove errors for the properties we're not interested in
            ModelState.Remove("Relationship.FoodItem.Name");
            ModelState.Remove("Relationship.FoodItem.Portion");

            if (ModelState.IsValid)
            {
                // Add the meal/food item relationship
                var item = await _client.AddAsync(
                    model.Relationship.MealId,
                    model.Relationship.FoodItemId,
                    model.Relationship.Quantity);

                // Return the meal composition view containing only the new item and a confirmation message  
                var compositionModel = new MealFoodItemListViewModel
                {
                    Meal = await _mealHelper.GetAsync(model.Relationship.MealId),
                    Message = "Meal/food item relationship successfully added"
                };
                return View("Index", compositionModel);
            }
            else
            {
                LogModelState(_logger);
            }

            // Populate the meal name and food category list and render the add view
            model.Meal = (await _mealHelper.GetAsync(model.Relationship.MealId)).Name;
            model.FoodCategories = await _foodCategoryListGenerator.Create();
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
        /// Handle POST events to update meal/food item relationships
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditMealFoodItemViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index", new { mealId = model.Relationship.MealId });
            }

            _logger.LogDebug($"Updating meal/food item relationship: {model.Relationship}");

            // The "FoodItem" entity is part of the hierarchy of the view model, allowing access to its properties in the
            // view without having to duplicate code, but this means validation of its properties will occur and result
            // in an invalid model state - remove errors for the properties we're not interested in
            ModelState.Remove("Relationship.FoodItem.Name");
            ModelState.Remove("Relationship.FoodItem.Portion");

            if (ModelState.IsValid)
            {
                // Update the meal/food item relationship
                var item = await _client.UpdateAsync(
                    model.Relationship.Id,
                    model.Relationship.MealId,
                    model.Relationship.FoodItemId,
                    model.Relationship.Quantity);

                // Return the meal composition view containing only the new item and a confirmation message  
                var compositionModel = new MealFoodItemListViewModel
                {
                    Meal = await _mealHelper.GetAsync(model.Relationship.MealId),
                    Message = "Meal/food item relationship successfully updated"
                };
                return View("Index", compositionModel);
            }
            else
            {
                LogModelState(_logger);
                result = View(model);
            }

            // Populate the meal name and food category list and render the edit view
            model.Meal = (await _mealHelper.GetAsync(model.Relationship.MealId)).Name;
            model.FoodCategories = await _foodCategoryListGenerator.Create();
            return result;
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