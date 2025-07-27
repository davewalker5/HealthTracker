using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace HealthTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class PlannedMealController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";

        private readonly IHealthTrackerFactory _factory;

        public PlannedMealController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return planned meal details given an ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<PlannedMeal>> GetPlannedMealByIdAsync(int id)
        {
            var plannedMeals = await _factory.PlannedMeals.ListAsync(x => x.Id == id, 1, int.MaxValue);

            if (plannedMeals.Count == 0)
            {
                return NotFound();
            }

            return plannedMeals.First();
        }

        /// <summary>
        /// Return a list of all planned meals
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<PlannedMeal>>> ListPlannedMealsAsync(int pageNumber, int pageSize)
        {
            var plannedMeals = await _factory.PlannedMeals.ListAsync(x => true, pageNumber, pageSize);

            if (plannedMeals == null)
            {
                return NoContent();
            }

            return plannedMeals;
        }

        /// <summary>
        /// Add a planned meal from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<PlannedMeal>> AddPlannedMealAsync([FromBody] PlannedMeal template)
        {
            var plannedMeal = await _factory.PlannedMeals.AddAsync(template.MealType, template.Date, template.MealId);
            return plannedMeal;
        }

        /// <summary>
        /// Update a planned meal from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<PlannedMeal>> UpdatePlannedMealAsync([FromBody] PlannedMeal template)
        {
            var plannedMeal = await _factory.PlannedMeals.UpdateAsync(template.Id, template.MealType, template.Date, template.MealId);
            return plannedMeal;
        }

        /// <summary>
        /// Delete a planned meal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeletePlannedMeal(int id)
        {
            // Check the meal exists, first
            var plannedMeal = await _factory.PlannedMeals.GetAsync(x => x.Id == id);
            if (plannedMeal == null)
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.PlannedMeals.DeleteAsync(id);
            return Ok();
        }

        /// <summary>
        /// Purge all planned meals earlier than a specified cutoff date
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("purge")]
        public async Task<IActionResult> PurgePlannedMeals([FromBody] PurgePlannedMealsModel model)
        {
            await _factory.PlannedMeals.Purge(model.Cutoff);
            return Ok();
        }
    }
}
