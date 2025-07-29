using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace HealthTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class MealConsumptionMeasurementController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";
        private readonly IHealthTrackerFactory _factory;
        private readonly IBackgroundQueue<RecalculateMealConsumptionWorkItem> _queue;

        public MealConsumptionMeasurementController(
            IHealthTrackerFactory factory,
            IBackgroundQueue<RecalculateMealConsumptionWorkItem> queue)
        {
            _factory = factory;
            _queue = queue;
        }

        /// <summary>
        /// Return a single measurement given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<MealConsumptionMeasurement>> Get(int id)
        {
            var measurements = await _factory.MealConsumptionMeasurements.ListAsync(x => x.Id == id, 1, int.MaxValue);
            return measurements.First();
        }

        /// <summary>
        /// Return a list of all measurements for a person
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{personId}/{from}/{to}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<MealConsumptionMeasurement>>> ListMealConsumptionMeasurementsForPersonAsync(int personId, string from, string to, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime fromDate = DateTime.ParseExact(HttpUtility.UrlDecode(from), DateTimeFormat, null);
            DateTime toDate = DateTime.ParseExact(HttpUtility.UrlDecode(to), DateTimeFormat, null);

            // Retrieve matching results
            var measurements = await _factory.MealConsumptionMeasurements.ListAsync(
                x => (x.PersonId == personId) && (x.Date >= fromDate) && (x.Date <= toDate),
                pageNumber,
                pageSize);

            if (measurements == null)
            {
                return NoContent();
            }

            return measurements;
        }

        /// <summary>
        /// Add a measurement from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<MealConsumptionMeasurement>> AddMealConsumptionMeasurementAsync([FromBody] MealConsumptionMeasurement template)
        {
            // Add the measurement
            var measurement = await _factory.MealConsumptionMeasurements.AddAsync(
                template.PersonId,
                template.MealId,
                template.Date,
                template.Quantity
            );

            return measurement;
        }

        /// <summary>
        /// Update a measurement from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<MealConsumptionMeasurement>> UpdateMealConsumptionMeasurementAsync([FromBody] MealConsumptionMeasurement template)
        {
            // Update the measurement
            var measurement = await _factory.MealConsumptionMeasurements.UpdateAsync(
                template.Id,
                template.PersonId,
                template.MealId,
                template.Date,
                template.Quantity
            );

            return measurement;
        }

        /// <summary>
        /// Queue a request to recalculate the nutritional values for all meal consumption records
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("recalculate")]
        public IActionResult RecalculateNutritionalValues([FromBody] RecalculateMealConsumptionWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Meal Consumption Nutritional Value Recalculation";

            // Queue the work item
            _queue.Enqueue(item);
            return Accepted();
        }

        /// <summary>
        /// Delete a measurement
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteMealConsumptionMeasurements(int id)
        {
            // Check the measurement exists, first
            var measurements = await _factory.MealConsumptionMeasurements.ListAsync(x => x.Id == id, 1, int.MaxValue);
            if (!measurements.Any())
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.MealConsumptionMeasurements.DeleteAsync(id);
            return Ok();
        }
    }
}
