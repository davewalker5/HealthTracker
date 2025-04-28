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
    public class CholesterolMeasurementController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";

        private readonly IHealthTrackerFactory _factory;

        public CholesterolMeasurementController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return a list of all cholesterol measurements for a person
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{personId}/{from}/{to}")]
        public async Task<ActionResult<IEnumerable<CholesterolMeasurement>>> ListCholesterolMeasurementsForPersonAsync(int personId, string from, string to)
        {
            // Decode the start and end date and convert them to dates
            DateTime fromDate = DateTime.ParseExact(HttpUtility.UrlDecode(from), DateTimeFormat, null);
            DateTime toDate = DateTime.ParseExact(HttpUtility.UrlDecode(to), DateTimeFormat, null);

            // Retrieve matching results
            var measurements = await _factory.CholesterolMeasurements.ListAsync(x => (x.PersonId == personId) && (x.Date >= fromDate) && (x.Date <= toDate));

            if (measurements == null)
            {
                return NoContent();
            }

            return measurements;
        }

        /// <summary>
        /// Add a cholesterol measurement from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<CholesterolMeasurement>> AddCholesterolMeasurementAsync([FromBody] CholesterolMeasurement template)
        {
            var measurement = await _factory.CholesterolMeasurements.AddAsync(
                template.PersonId,
                template.Date,
                template.Total,
                template.HDL,
                template.LDL,
                template.Triglycerides
            );
            return measurement;
        }

        /// <summary>
        /// Update a cholesterol measurement from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<CholesterolMeasurement>> UpdatePersonAsync([FromBody] CholesterolMeasurement template)
        {
            var measurement = await _factory.CholesterolMeasurements.UpdateAsync(
                template.Id, 
                template.PersonId,
                template.Date,
                template.Total,
                template.HDL,
                template.LDL,
                template.Triglycerides
            );
            return measurement;
        }

        /// <summary>
        /// Delete a cholesterol measurement
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCholesterolMeasurements(int id)
        {
            // Check the measurement exists, first
            var measurements = await _factory.CholesterolMeasurements.ListAsync(x => x.Id == id);
            if (!measurements.Any())
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.CholesterolMeasurements.DeleteAsync(id);
            return Ok();
        }
    }
}
