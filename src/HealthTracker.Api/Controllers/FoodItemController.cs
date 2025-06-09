using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<IEnumerable<FoodItem>>> ListAllFoodItemsAsync(int foodCategoryId, int pageNumber, int pageSize)
        {
            var foodItems = await _factory.FoodItems.ListAsync(x => x.FoodCategoryId == foodCategoryId, pageNumber, pageSize);

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

            // It does, so delete any related nutritional value record
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
