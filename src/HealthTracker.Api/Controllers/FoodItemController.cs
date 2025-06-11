using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace HealthTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class FoodItemController : Controller
    {
        private readonly IHealthTrackerFactory _factory;

        public FoodItemController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return food item details given an ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<FoodItem>> GetFoodItemByIdAsync(int id)
        {
            var foodItems = await _factory.FoodItems.ListAsync(x => x.Id == id, 1, int.MaxValue);

            if (foodItems.Count == 0)
            {
                return NotFound();
            }

            return foodItems.First();
        }

        /// <summary>
        /// Return a list of all food items in a specified category
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{foodCategoryId}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<FoodItem>>> ListFoodItemsAsync(int foodCategoryId, int pageNumber, int pageSize)
        {
            // If a food category is provided, return only items in that category. Otherwise, return all items.
            Expression<Func<FoodItem, bool>> predicate = foodCategoryId > 0 ? x => x.FoodCategoryId == foodCategoryId : x => true;
            var foodItems = await _factory.FoodItems.ListAsync(predicate, pageNumber, pageSize);

            if (foodItems == null)
            {
                return NoContent();
            }

            return foodItems;
        }

        /// <summary>
        /// Add a food item from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<FoodItem>> AddFoodItemAsync([FromBody] FoodItem template)
        {
            var foodItem = await _factory.FoodItems.AddAsync(template.Name, template.Portion, template.FoodCategoryId, template.NutritionalValueId);
            return foodItem;
        }

        /// <summary>
        /// Update a food item from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<FoodItem>> UpdateFoodItemAsync([FromBody] FoodItem template)
        {
            var foodItem = await _factory.FoodItems.UpdateAsync(template.Id, template.Name, template.Portion, template.FoodCategoryId, template.NutritionalValueId);
            return foodItem;
        }

        /// <summary>
        /// Delete a food item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteFoodItem(int id)
        {
            // Check the item exists, first
            var foodItems = await _factory.FoodItems.ListAsync(x => x.Id == id, 1, int.MaxValue);
            if (foodItems.Count == 0)
            {
                return NotFound();
            }

            // It does, so delete any related nutritional value record first
            var nutritionalValueId = foodItems.First().NutritionalValueId ?? 0;
            if (nutritionalValueId > 0)
            {
                await _factory.NutritionalValues.DeleteAsync(nutritionalValueId);
            }

            // Delete the food item itself
            await _factory.FoodItems.DeleteAsync(id);
            return Ok();
        }
    }
}
