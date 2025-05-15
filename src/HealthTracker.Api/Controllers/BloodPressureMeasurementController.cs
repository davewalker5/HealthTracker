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
    public class BloodPressureMeasurementController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";

        private readonly IHealthTrackerFactory _factory;

        public BloodPressureMeasurementController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<BloodPressureMeasurement>> Get(int id)
        {
            var measurements = await _factory.BloodPressureMeasurements.ListAsync(x => x.Id == id, 1, int.MaxValue);
            await _factory.BloodPressureAssessor.Assess(measurements);
            return measurements.First();
        }

        /// <summary>
        /// Return a list of all blood pressure measurements for a person
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{personId}/{from}/{to}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<BloodPressureMeasurement>>> ListBloodPressureMeasurementsForPersonAsync(int personId, string from, string to, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime fromDate = DateTime.ParseExact(HttpUtility.UrlDecode(from), DateTimeFormat, null);
            DateTime toDate = DateTime.ParseExact(HttpUtility.UrlDecode(to), DateTimeFormat, null);

            // Retrieve matching results
            var measurements = await _factory.BloodPressureMeasurements.ListAsync(
                x => (x.PersonId == personId) && (x.Date >= fromDate) && (x.Date <= toDate),
                pageNumber,
                pageSize);

            if (measurements == null)
            {
                return NoContent();
            }

            // Assess each measurement so the assessment is returned with each one
            await _factory.BloodPressureAssessor.Assess(measurements);

            return measurements;
        }

        /// <summary>
        /// Add a blood pressure measurement from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<BloodPressureMeasurement>> AddBloodPressureMeasurementAsync([FromBody] BloodPressureMeasurement template)
        {
            // Add the measurement
            var measurement = await _factory.BloodPressureMeasurements.AddAsync(
                template.PersonId,
                template.Date,
                template.Systolic,
                template.Diastolic
            );

            // Assess the measurement so the assessment is returned with it
            await _factory.BloodPressureAssessor.Assess([measurement]);
            return measurement;
        }

        /// <summary>
        /// Update a blood pressure measurement from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<BloodPressureMeasurement>> UpdateBloodPressureMeasurementAsync([FromBody] BloodPressureMeasurement template)
        {
            // Update the measurement
            var measurement = await _factory.BloodPressureMeasurements.UpdateAsync(
                template.Id, 
                template.PersonId,
                template.Date,
                template.Systolic,
                template.Diastolic
            );

            // Assess the measurement so the assessment is returned with it
            await _factory.BloodPressureAssessor.Assess([measurement]);
            return measurement;
        }

        /// <summary>
        /// Delete a blood pressure measurement
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteBloodPressureMeasurements(int id)
        {
            // Check the measurement exists, first
            var measurements = await _factory.BloodPressureMeasurements.ListAsync(x => x.Id == id, 1, int.MaxValue);
            if (!measurements.Any())
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.BloodPressureMeasurements.DeleteAsync(id);
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
        public async Task<ActionResult<BloodPressureMeasurement>> CalculateAverageAsync(int personId, string from, string to)
        {
            // Decode the start and end date and convert them to dates
            DateTime fromDate = DateTime.ParseExact(HttpUtility.UrlDecode(from), DateTimeFormat, null);
            DateTime toDate = DateTime.ParseExact(HttpUtility.UrlDecode(to), DateTimeFormat, null);

            // Calculate the average measurement. If there are none in the specified date range, the result is null
            var average = await _factory.BloodPressureCalculator.AverageAsync(personId, fromDate, toDate);
            if (average == null)
            {
                return NotFound();
            }

            // Assess the average measurement so the assessment is returned with it
            await _factory.BloodPressureAssessor.Assess([average]);
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
        public async Task<ActionResult<List<BloodPressureMeasurement>>> CalculateDailyAverageAsync(int personId, string from, string to)
        {
            // Decode the start and end date and convert them to dates
            DateTime fromDate = DateTime.ParseExact(HttpUtility.UrlDecode(from), DateTimeFormat, null);
            DateTime toDate = DateTime.ParseExact(HttpUtility.UrlDecode(to), DateTimeFormat, null);

            // Calculate the daily average measurements. If there are none in the specified date range, the result is null
            var averages = await _factory.BloodPressureCalculator.DailyAverageAsync(personId, fromDate, toDate);
            if (averages == null)
            {
                return NotFound();
            }

            // Assess each average measurement so the assessment is returned with each one
            await _factory.BloodPressureAssessor.Assess(averages);
            return averages;
        }
    }
}
