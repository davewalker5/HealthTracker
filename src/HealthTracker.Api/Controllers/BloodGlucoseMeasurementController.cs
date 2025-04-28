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
    public class BloodGlucoseMeasurementController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";

        private readonly IHealthTrackerFactory _factory;

        public BloodGlucoseMeasurementController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return a list of all % SPO2 measurements for a person
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{personId}/{from}/{to}")]
        public async Task<ActionResult<IEnumerable<BloodGlucoseMeasurement>>> ListBloodGlucoseMeasurementsForPersonAsync(int personId, string from, string to)
        {
            // Decode the start and end date and convert them to dates
            DateTime fromDate = DateTime.ParseExact(HttpUtility.UrlDecode(from), DateTimeFormat, null);
            DateTime toDate = DateTime.ParseExact(HttpUtility.UrlDecode(to), DateTimeFormat, null);

            // Retrieve matching results
            var measurements = await _factory.BloodGlucoseMeasurements.ListAsync(x => (x.PersonId == personId) && (x.Date >= fromDate) && (x.Date <= toDate));

            if (measurements == null)
            {
                return NoContent();
            }

            return measurements;
        }

        /// <summary>
        /// Add a % SPO2 measurement from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<BloodGlucoseMeasurement>> AddBloodGlucoseMeasurementAsync([FromBody] BloodGlucoseMeasurement template)
        {
            // Add the measurement
            var measurement = await _factory.BloodGlucoseMeasurements.AddAsync(
                template.PersonId,
                template.Date,
                template.Level
            );

            return measurement;
        }

        /// <summary>
        /// Update a % SPO2 measurement from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<BloodGlucoseMeasurement>> UpdateBloodGlucoseMeasurementAsync([FromBody] BloodGlucoseMeasurement template)
        {
            // Update the measurement
            var measurement = await _factory.BloodGlucoseMeasurements.UpdateAsync(
                template.Id, 
                template.PersonId,
                template.Date,
                template.Level
            );

            return measurement;
        }

        /// <summary>
        /// Delete a % SPO2 measurement
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteBloodGlucoseMeasurements(int id)
        {
            // Check the measurement exists, first
            var measurements = await _factory.BloodGlucoseMeasurements.ListAsync(x => x.Id == id);
            if (!measurements.Any())
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.BloodGlucoseMeasurements.DeleteAsync(id);
            return Ok();
        }
    }
}
