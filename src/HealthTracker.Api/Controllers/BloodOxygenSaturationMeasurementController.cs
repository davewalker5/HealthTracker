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
    public class BloodOxygenSaturationMeasurementController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";

        private readonly IHealthTrackerFactory _factory;

        public BloodOxygenSaturationMeasurementController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return a single measurement given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<BloodOxygenSaturationMeasurement>> Get(int id)
        {
            var measurements = await _factory.BloodOxygenSaturationMeasurements.ListAsync(x => x.Id == id, 1, int.MaxValue);
            await _factory.BloodOxygenSaturationAssessor.Assess(measurements);
            return measurements.First();
        }
    
        /// <summary>
        /// Return a list of all % SPO2 measurements for a person
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{personId}/{from}/{to}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<BloodOxygenSaturationMeasurement>>> ListBloodOxygenSaturationMeasurementsForPersonAsync(int personId, string from, string to, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime fromDate = DateTime.ParseExact(HttpUtility.UrlDecode(from), DateTimeFormat, null);
            DateTime toDate = DateTime.ParseExact(HttpUtility.UrlDecode(to), DateTimeFormat, null);

            // Retrieve matching results
            var measurements = await _factory.BloodOxygenSaturationMeasurements.ListAsync(
                x => (x.PersonId == personId) && (x.Date >= fromDate) && (x.Date <= toDate),
                pageNumber,
                pageSize);

            if (measurements == null)
            {
                return NoContent();
            }

            // Assess each measurement so the assessment is returned with each one
            await _factory.BloodOxygenSaturationAssessor.Assess(measurements);

            return measurements;
        }

        /// <summary>
        /// Add a % SPO2 measurement from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<BloodOxygenSaturationMeasurement>> AddBloodOxygenSaturationMeasurementAsync([FromBody] BloodOxygenSaturationMeasurement template)
        {
            // Add the measurement
            var measurement = await _factory.BloodOxygenSaturationMeasurements.AddAsync(
                template.PersonId,
                template.Date,
                template.Percentage
            );

            // Assess the measurement so the assessment is returned with it
            await _factory.BloodOxygenSaturationAssessor.Assess([measurement]);
            return measurement;
        }

        /// <summary>
        /// Update a % SPO2 measurement from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<BloodOxygenSaturationMeasurement>> UpdateBloodOxygenSaturationMeasurementAsync([FromBody] BloodOxygenSaturationMeasurement template)
        {
            // Update the measurement
            var measurement = await _factory.BloodOxygenSaturationMeasurements.UpdateAsync(
                template.Id, 
                template.PersonId,
                template.Date,
                template.Percentage
            );

            // Assess the measurement so the assessment is returned with it
            await _factory.BloodOxygenSaturationAssessor.Assess([measurement]);
            return measurement;
        }

        /// <summary>
        /// Delete a % SPO2 measurement
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteBloodOxygenSaturationMeasurements(int id)
        {
            // Check the measurement exists, first
            var measurements = await _factory.BloodOxygenSaturationMeasurements.ListAsync(x => x.Id == id, 1, int.MaxValue);
            if (!measurements.Any())
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.BloodOxygenSaturationMeasurements.DeleteAsync(id);
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
        public async Task<ActionResult<BloodOxygenSaturationMeasurement>> CalculateAverageAsync(int personId, string from, string to)
        {
            // Decode the start and end date and convert them to dates
            DateTime fromDate = DateTime.ParseExact(HttpUtility.UrlDecode(from), DateTimeFormat, null);
            DateTime toDate = DateTime.ParseExact(HttpUtility.UrlDecode(to), DateTimeFormat, null);

            // Calculate the average measurement. If there are none in the specified date range, the result is null
            var average = await _factory.BloodOxygenSaturationCalculator.AverageAsync(personId, fromDate, toDate);
            if (average == null)
            {
                return NotFound();
            }

            // Assess the average measurement so the assessment is returned with it
            await _factory.BloodOxygenSaturationAssessor.Assess([average]);
            return average;
        }

        /// <summary>
        /// Return the daily average blood pressure reading for each date in a date range for a person
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("dailyaverage/{personId}/{from}/{to}")]
        public async Task<ActionResult<List<BloodOxygenSaturationMeasurement>>> CalculateDailyAverageAsync(int personId, string from, string to)
        {
            // Decode the start and end date and convert them to dates
            DateTime fromDate = DateTime.ParseExact(HttpUtility.UrlDecode(from), DateTimeFormat, null);
            DateTime toDate = DateTime.ParseExact(HttpUtility.UrlDecode(to), DateTimeFormat, null);

            // Calculate the daily average measurements. If there are none in the specified date range, the result is null
            var averages = await _factory.BloodOxygenSaturationCalculator.DailyAverageAsync(personId, fromDate, toDate);
            if (averages == null)
            {
                return NotFound();
            }

            // Assess each average measurement so the assessment is returned with each one
            await _factory.BloodOxygenSaturationAssessor.Assess(averages);
            return averages;
        }
    }
}
