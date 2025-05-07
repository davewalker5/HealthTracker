using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace HealthTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class WeightMeasurementController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";

        private readonly IHealthTrackerFactory _factory;

        public WeightMeasurementController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return a list of all weight measurements for a person
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{personId}/{from}/{to}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<WeightMeasurement>>> ListWeightMeasurementsForPersonAsync(int personId, string from, string to, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime fromDate = DateTime.ParseExact(HttpUtility.UrlDecode(from), DateTimeFormat, null);
            DateTime toDate = DateTime.ParseExact(HttpUtility.UrlDecode(to), DateTimeFormat, null);

            // Retrieve matching results
            var measurements = await _factory.WeightMeasurements.ListAsync(
                x => (x.PersonId == personId) && (x.Date >= fromDate) && (x.Date <= toDate),
                pageNumber,
                pageSize);
            if (measurements == null)
            {
                return NoContent();
            }

            // Calculate the BMI for each measurement
            await _factory.WeightCalculator.CalculateRelatedProperties(measurements);

            return measurements;
        }

        /// <summary>
        /// Add a weight measurement from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<WeightMeasurement>> AddWeightMeasurementAsync([FromBody] WeightMeasurement template)
        {
            // Add the measurement
            var measurement = await _factory.WeightMeasurements.AddAsync(
                template.PersonId,
                template.Date,
                template.Weight
            );

            // Calculate the BMI for the measurement
            await _factory.WeightCalculator.CalculateRelatedProperties([measurement]);
            return measurement;
        }

        /// <summary>
        /// Update a weight measurement from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<WeightMeasurement>> UpdatePersonAsync([FromBody] WeightMeasurement template)
        {
            // Update the measurement
            var measurement = await _factory.WeightMeasurements.UpdateAsync(
                template.Id, 
                template.PersonId,
                template.Date,
                template.Weight
            );

            // Calculate the BMI for the measurement
            await _factory.WeightCalculator.CalculateRelatedProperties([measurement]);
            return measurement;
        }

        /// <summary>
        /// Delete a weight measurement
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteWeightMeasurements(int id)
        {
            // Check the measurement exists, first
            var measurements = await _factory.WeightMeasurements.ListAsync(x => x.Id == id, 1, int.MaxValue);
            if (!measurements.Any())
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.WeightMeasurements.DeleteAsync(id);
            return Ok();
        }

        /// <summary>
        /// Calculate and return the average measurement for a person and a date range
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("average/{personId}/{from}/{to}")]
        public async Task<ActionResult<WeightMeasurement>> CalculateAverageAsync(int personId, string from, string to)
        {
            // Decode the start and end date and convert them to dates
            DateTime fromDate = DateTime.ParseExact(HttpUtility.UrlDecode(from), DateTimeFormat, null);
            DateTime toDate = DateTime.ParseExact(HttpUtility.UrlDecode(to), DateTimeFormat, null);

            // Calculate the average measurement. If there are none in the specified date range, the result is null
            var average = await _factory.WeightCalculator.AverageAsync(personId, fromDate, toDate);
            if (average == null)
            {
                return NotFound();
            }

            // Calculate the BMI for the average measurement
            await _factory.WeightCalculator.CalculateRelatedProperties([average]);
            return average;
        }
    }
}
