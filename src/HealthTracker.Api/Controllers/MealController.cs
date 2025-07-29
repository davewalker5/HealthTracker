using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace HealthTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class MealController : Controller
    {
        private readonly IHealthTrackerFactory _factory;
        private readonly IBackgroundQueue<RecalculateMealNutritionWorkItem> _queue;

        public MealController(
            IHealthTrackerFactory factory,
            IBackgroundQueue<RecalculateMealNutritionWorkItem> queue)
        {
            _factory = factory;
            _queue = queue;
        }

        /// <summary>
        /// Return meal details given an ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Meal>> GetMealByIdAsync(int id)
        {
            var meals = await _factory.Meals.ListAsync(x => x.Id == id, 1, int.MaxValue);

            if (meals.Count == 0)
            {
                return NotFound();
            }

            return meals.First();
        }

        /// <summary>
        /// Return a list of all meals
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{foodSourceId}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<Meal>>> ListMealsAsync(int foodSourceId, int pageNumber, int pageSize)
        {
            // If a food source is provided, return only meals from that source. Otherwise, return all meals.
            Expression<Func<Meal, bool>> predicate = foodSourceId > 0 ? x => x.FoodSourceId == foodSourceId : x => true;
            var meals = await _factory.Meals.ListAsync(predicate, pageNumber, pageSize);

            if (meals == null)
            {
                return NoContent();
            }

            return meals;
        }

        /// <summary>
        /// Add a meal from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Meal>> AddMealAsync([FromBody] Meal template)
        {
            var meal = await _factory.Meals.AddAsync(
                template.Name, template.Portions, template.FoodSourceId, template.Reference, template.NutritionalValueId);
            return meal;
        }

        /// <summary>
        /// Update a meal from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Meal>> UpdateMealAsync([FromBody] Meal template)
        {
            var meal = await _factory.Meals.UpdateAsync(
                template.Id, template.Name, template.Portions, template.FoodSourceId, template.Reference, template.NutritionalValueId);
            return meal;
        }

        /// <summary>
        /// Queue a request to recalculate the nutritional values for all meals
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("recalculate")]
        public IActionResult RecalculateNutritionalValues([FromBody] RecalculateMealNutritionWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Meal Nutritional Value Recalculation";

            // Queue the work item
            _queue.Enqueue(item);
            return Accepted();
        }

        /// <summary>
        /// Delete a meal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteMeal(int id)
        {
            // Check the meal exists, first
            var meals = await _factory.Meals.ListAsync(x => x.Id == id, 1, int.MaxValue);
            if (meals.Count == 0)
            {
                return NotFound();
            }

            // It does, so delete any related nutritional value record first
            var nutritionalValueId = meals.First().NutritionalValueId ?? 0;
            if (nutritionalValueId > 0)
            {
                await _factory.NutritionalValues.DeleteAsync(nutritionalValueId);
            }

            // Delete the meal itself
            await _factory.Meals.DeleteAsync(id);
            return Ok();
        }
    }
}
