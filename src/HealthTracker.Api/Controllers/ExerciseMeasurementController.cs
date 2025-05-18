using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace HealthTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class ExerciseMeasurementController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";

        private readonly IHealthTrackerFactory _factory;

        public ExerciseMeasurementController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return a single measurement given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ExerciseMeasurement>> Get(int id)
        {
            var measurements = await _factory.ExerciseMeasurements.ListAsync(x => x.Id == id, 1, int.MaxValue);
            await PopulateAncillaryProperties(measurements);
            return measurements.First();
        }

        /// <summary>
        /// Return a list of all exercise measurements for a person
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{personId}/{from}/{to}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<ExerciseMeasurement>>> ListExerciseMeasurementsForPersonAsync(int personId, string from, string to, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime fromDate = DateTime.ParseExact(HttpUtility.UrlDecode(from), DateTimeFormat, null);
            DateTime toDate = DateTime.ParseExact(HttpUtility.UrlDecode(to), DateTimeFormat, null);

            // Retrieve matching results
            var measurements = await _factory.ExerciseMeasurements.ListAsync(
                x => (x.PersonId == personId) && (x.Date >= fromDate) && (x.Date <= toDate),
                pageNumber,
                pageSize);

            if (measurements == null)
            {
                return NoContent();
            }

            // Populate ancillary properties
            await PopulateAncillaryProperties(measurements);

            return measurements;
        }

        /// <summary>
        /// Summarise exercise for a person, activity type and date range
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("summarise/{personId}/{activityTypeId}/{from}/{to}")]
        public async Task<IEnumerable<ExerciseSummary>> SummariseExerciseForPerson(int personId, int activityTypeId, string from, string to)
        {
            // Decode the start and end date and convert them to dates
            DateTime fromDate = DateTime.ParseExact(HttpUtility.UrlDecode(from), DateTimeFormat, null);
            DateTime toDate = DateTime.ParseExact(HttpUtility.UrlDecode(to), DateTimeFormat, null);

            // Generate the summary
            IEnumerable<ExerciseSummary> summaries;
            if (activityTypeId > 0)
            {
                summaries = await _factory.ExerciseCalculator.SummariseAsync(personId, activityTypeId, fromDate, toDate);
            }
            else
            {
                summaries = await _factory.ExerciseCalculator.SummariseAsync(personId, fromDate, toDate);
            }

            return summaries;
        }

        /// <summary>
        /// Add an exercise measurement from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<ExerciseMeasurement>> AddExerciseMeasurementAsync([FromBody] ExerciseMeasurement template)
        {
            var measurement = await _factory.ExerciseMeasurements.AddAsync(
                template.PersonId,
                template.ActivityTypeId,
                template.Date,
                template.Duration,
                template.Distance,
                template.Calories,
                template.MinimumHeartRate,
                template.MaximumHeartRate
            );
            return measurement;
        }

        /// <summary>
        /// Update an exercise measurement from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<ExerciseMeasurement>> UpdatePersonAsync([FromBody] ExerciseMeasurement template)
        {
            var measurement = await _factory.ExerciseMeasurements.UpdateAsync(
                template.Id,
                template.PersonId,
                template.ActivityTypeId,
                template.Date,
                template.Duration,
                template.Distance,
                template.Calories,
                template.MinimumHeartRate,
                template.MaximumHeartRate
            );
            return measurement;
        }

        /// <summary>
        /// Delete an exercise measurement
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteExerciseMeasurements(int id)
        {
            // Check the measurement exists, first
            var measurements = await _factory.ExerciseMeasurements.ListAsync(x => x.Id == id, 1, int.MaxValue);
            if (!measurements.Any())
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.ExerciseMeasurements.DeleteAsync(id);
            return Ok();
        }

        /// <summary>
        /// Populate ancillary exercise measurement properties
        /// </summary>
        /// <param name="measurements"></param>
        /// <returns></returns>
        private async Task PopulateAncillaryProperties(IEnumerable<ExerciseMeasurement> measurements)
        {
            var activityTypes = await _factory.ActivityTypes.ListAsync(x => true, 1, int.MaxValue);
            foreach (var measurement in measurements)
            {
                measurement.ActivityType = activityTypes.First(x => x.Id == measurement.ActivityTypeId).Description;
                measurement.FormattedDuration = measurement.Duration.ToFormattedDuration();
            }
        }
    }
}
