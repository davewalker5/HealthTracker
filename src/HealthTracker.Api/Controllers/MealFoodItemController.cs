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
    public class MealFoodItemController : Controller
    {
        private readonly IHealthTrackerFactory _factory;

        public MealFoodItemController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return a meal/food item relationship given an ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<MealFoodItem>> GetMealFoodItemByIdAsync(int id)
        {
            var relationship = await _factory.MealFoodItems.GetAsync(x => x.Id == id);

            if (relationship == null)
            {
                return NotFound();
            }

            return relationship;
        }

        /// <summary>
        /// Return a list of meal/food item relationships for a specified meal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("list/{mealId}")]
        public async Task<ActionResult<IEnumerable<MealFoodItem>>> ListMealFoodItemsAsync(int mealId)
        {
            var relationships = await _factory.MealFoodItems.ListAsync(x => x.MealId == mealId);

            if (relationships == null)
            {
                return NoContent();
            }

            return relationships;
        }

        /// <summary>
        /// Add a meal/food item relationship from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<MealFoodItem>> AddMealFoodItemAsync([FromBody] MealFoodItem template)
        {
            var meal = await _factory.MealFoodItems.AddAsync(template.MealId, template.FoodItemId);
            return meal;
        }

        /// <summary>
        /// Update a meal/food item relationship from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<MealFoodItem>> UpdateMealFoodItemAsync([FromBody] MealFoodItem template)
        {
            var meal = await _factory.MealFoodItems.UpdateAsync(template.Id, template.MealId, template.FoodItemId);
            return meal;
        }

        /// <summary>
        /// Delete a meal/food item relationship
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteMealFoodItem(int id)
        {
            // Check the meal exists, first
            var relationship = await _factory.MealFoodItems.GetAsync(x => x.Id == id);
            if (relationship == null)
            {
                return NotFound();
            }

            // Delete the relationship
            await _factory.MealFoodItems.DeleteAsync(id);
            return Ok();
        }
    }
}
