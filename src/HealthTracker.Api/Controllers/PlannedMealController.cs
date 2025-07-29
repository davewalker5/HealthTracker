using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using HealthTracker.Api.Interfaces;

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
        private readonly IBackgroundQueue<PurgePlannedMealsWorkItem> _queue;

        public PlannedMealController(IHealthTrackerFactory factory, IBackgroundQueue<PurgePlannedMealsWorkItem> queue)
        {
            _factory = factory;
            _queue = queue;
        }

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
        [Route("{personId}/{from}/{to}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<PlannedMeal>>> ListPlannedMealsAsync(int personId, string from, string to, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime fromDate = DateTime.ParseExact(HttpUtility.UrlDecode(from), DateTimeFormat, null);
            DateTime toDate = DateTime.ParseExact(HttpUtility.UrlDecode(to), DateTimeFormat, null);

            // Retrieve planned meals in the specified date range
            var plannedMeals = await _factory.PlannedMeals.ListAsync(
                x => (x.PersonId == personId) && (x.Date >= fromDate) && (x.Date <= toDate),
                pageNumber,
                pageSize);

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
            var plannedMeal = await _factory.PlannedMeals.AddAsync(template.PersonId, template.MealType, template.Date, template.MealId);
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
            var plannedMeal = await _factory.PlannedMeals.UpdateAsync(template.Id, template.PersonId, template.MealType, template.Date, template.MealId);
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
        /// Purge all planned meals per the specification in the work item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("purge")]
        public IActionResult PurgePlannedMeals([FromBody] PurgePlannedMealsWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Planned Meal Purge";

            // Queue the work item
            _queue.Enqueue(item);
            return Accepted();
        }
    }
}
