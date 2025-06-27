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
    public class MealController : Controller
    {
        private readonly IHealthTrackerFactory _factory;

        public MealController(IHealthTrackerFactory factory)
            => _factory = factory;

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
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<Meal>>> ListMealsAsync(int pageNumber, int pageSize)
        {
            var meals = await _factory.Meals.ListAsync(x => true, pageNumber, pageSize);

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
            var meal = await _factory.Meals.AddAsync(template.Name, template.Portions, template.NutritionalValueId);
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
            var meal = await _factory.Meals.UpdateAsync(template.Id, template.Name, template.Portions, template.NutritionalValueId);
            return meal;
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
